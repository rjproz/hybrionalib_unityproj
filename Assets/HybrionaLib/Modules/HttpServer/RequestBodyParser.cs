using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Hybriona
{
    /// <summary>
    /// Represents a single uploaded file.
    /// </summary>
    public class ParsedFile
    {
        public string FieldName { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public byte[] Data { get; set; }
    }

    /// <summary>
    /// Holds parsed form fields and file uploads.
    /// </summary>
    public class FormData
    {
        public Dictionary<string, string> Fields { get; } = new Dictionary<string, string>();
        public List<ParsedFile> Files { get; } = new List<ParsedFile>();
    }

    /// <summary>
    /// Parses application/x-www-form-urlencoded and multipart/form-data bodies.
    /// </summary>
    public static class RequestBodyParser
    {
        /// <summary>
        /// Parse the request body into form fields and files.
        /// </summary>
        public static FormData Parse(string rawBody, string contentTypeHeader)
        {
            var form = new FormData();

            if (string.IsNullOrEmpty(contentTypeHeader))
                return form;

            var ct = contentTypeHeader.Split(';')[0].Trim();

            if (ct.Equals("application/x-www-form-urlencoded", StringComparison.OrdinalIgnoreCase))
            {
                ParseUrlEncoded(rawBody, form);
            }
            else if (ct.Equals("multipart/form-data", StringComparison.OrdinalIgnoreCase))
            {
                // find boundary
                var boundary = GetBoundary(contentTypeHeader);
                if (boundary != null)
                    ParseMultipart(rawBody, boundary, form);
            }

            return form;
        }

        private static void ParseUrlEncoded(string body, FormData form)
        {
            // key1=val1&key2=val2...
            var pairs = body.Split('&', StringSplitOptions.RemoveEmptyEntries);
            foreach (var pair in pairs)
            {
                var kv = pair.Split('=', 2);
                var key = Uri.UnescapeDataString(kv[0]);
                var val = kv.Length > 1
                    ? Uri.UnescapeDataString(kv[1])
                    : "";
                form.Fields[key] = val;
            }
        }

        private static string GetBoundary(string contentTypeHeader)
        {
            // Content-Type: multipart/form-data; boundary=----WebKitFormBoundaryXYZ
            var parts = contentTypeHeader.Split(';');
            foreach (var part in parts)
            {
                var t = part.Trim();
                if (t.StartsWith("boundary=", StringComparison.OrdinalIgnoreCase))
                    return t.Substring("boundary=".Length).Trim('"');
            }
            return null;
        }

        private static void ParseMultipart(string body, string boundary, FormData form)
        {
            var delim = $"--{boundary}";
            var sections = body.Split(new[] { delim }, StringSplitOptions.RemoveEmptyEntries)
                               .Select(s => s.Trim('\r', '\n'))
                               .Where(s => s != "--") // trailing
                               .ToArray();

            foreach (var section in sections)
            {
                // split headers / content
                var idx = section.IndexOf("\r\n\r\n");
                if (idx < 0) continue;
                var headerPart = section.Substring(0, idx);
                var contentPart = section.Substring(idx + 4);

                // parse headers
                var headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                foreach (var line in headerPart.Split("\r\n"))
                {
                    var kv = line.Split(':', 2);
                    if (kv.Length == 2)
                        headers[kv[0].Trim()] = kv[1].Trim();
                }

                // content-disposition is mandatory
                if (!headers.TryGetValue("Content-Disposition", out var disp)) continue;
                // example: form-data; name="myField"; filename="foo.txt"
                var dispParts = disp.Split(';')
                                    .Select(p => p.Trim())
                                    .ToArray();
                var namePart = dispParts.FirstOrDefault(p => p.StartsWith("name=", StringComparison.OrdinalIgnoreCase));
                var filenamePart = dispParts.FirstOrDefault(p => p.StartsWith("filename=", StringComparison.OrdinalIgnoreCase));

                var fieldName = Unquote(namePart?.Split('=', 2)[1] ?? "");
                if (filenamePart == null)
                {
                    // regular field
                    form.Fields[fieldName] = contentPart.TrimEnd('\r', '\n');
                }
                else
                {
                    // file field
                    var fileName = Unquote(filenamePart.Split('=', 2)[1]);
                    headers.TryGetValue("Content-Type", out var fileCt);

                    // raw bytes
                    var data = Encoding.UTF8.GetBytes(contentPart);

                    form.Files.Add(new ParsedFile
                    {
                        FieldName = fieldName,
                        FileName = fileName,
                        ContentType = fileCt ?? "application/octet-stream",
                        Data = data
                    });
                }
            }
        }

        private static string Unquote(string s)
        {
            if (s.StartsWith("\"") && s.EndsWith("\""))
                return s.Substring(1, s.Length - 2);
            return s;
        }
    }
}
