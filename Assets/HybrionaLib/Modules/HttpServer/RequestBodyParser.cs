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
        public static FormData Parse(byte[] rawBody, string contentTypeHeader)
        {
            var form = new FormData();

            if (string.IsNullOrEmpty(contentTypeHeader))
                return form;

            var ct = contentTypeHeader.Split(';')[0].Trim();

            if (ct.Equals("application/x-www-form-urlencoded", StringComparison.OrdinalIgnoreCase))
            {
                ParseUrlEncoded(System.Text.Encoding.UTF8.GetString( rawBody ), form);
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

        private static byte[] CRLF = new byte[] { 13, 10 }; // \r\n
        private static byte[] HEADER_BODY_SEPARATOR = new byte[] { 13, 10, 13, 10 };

        public static void ParseMultipart(byte[] bodyBytes, string boundary, FormData form, Encoding encoding = null)
        {
            encoding ??= Encoding.UTF8;
            // the actual boundary marker in the body is prefixed with "--"
            var boundaryBytes = encoding.GetBytes($"--{boundary}");
            var parts = SplitOnSequence(bodyBytes, boundaryBytes)
                        // the first and last entries around the preamble/epilogue can be empty
                        .Where(p => p.Length > 0 && !IsFinalBoundary(p, boundaryBytes))
                        .ToList();

            foreach (var part in parts)
            {
                // each part begins with CRLF, trim it
                var trimmedPart = TrimStart(part, CRLF);

                // find header/body separator (\r\n\r\n)
                int sepIndex = IndexOfSequence(trimmedPart, HEADER_BODY_SEPARATOR, 0);
                if (sepIndex < 0) continue;

                // header bytes and body bytes
                var headerBytes = trimmedPart.Take(sepIndex).ToArray();
                var contentBytes = trimmedPart
                    .Skip(sepIndex + HEADER_BODY_SEPARATOR.Length)
                    .ToArray();

                // parse headers (ASCII)
                var headerText = encoding.GetString(headerBytes);
                var headers = headerText
                    .Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(line => line.Split(new[] { ':' }, 2))
                    .Where(kv => kv.Length == 2)
                    .ToDictionary(kv => kv[0].Trim(), kv => kv[1].Trim(), StringComparer.OrdinalIgnoreCase);

                if (!headers.TryGetValue("Content-Disposition", out var disp))
                    continue;

                // dissect content-disposition
                var dispParts = disp.Split(';').Select(s => s.Trim()).ToArray();
                var namePart = dispParts.FirstOrDefault(p => p.StartsWith("name=", StringComparison.OrdinalIgnoreCase));
                var filePart = dispParts.FirstOrDefault(p => p.StartsWith("filename=", StringComparison.OrdinalIgnoreCase));

                var fieldName = Unquote(namePart?.Split('=', 2)[1] ?? "");

                if (filePart == null)
                {
                    // text field: decode contentBytes
                    var value = encoding.GetString(contentBytes).TrimEnd('\r', '\n');
                    form.Fields[fieldName] = value;
                }
                else
                {
                    // file field
                    var fileName = Unquote(filePart.Split('=', 2)[1]);
                    headers.TryGetValue("Content-Type", out var fileCt);

                    form.Files.Add(new ParsedFile
                    {
                        FieldName = fieldName,
                        FileName = fileName,
                        ContentType = fileCt ?? "application/octet-stream",
                        Data = contentBytes
                    });
                }
            }
        }

        // Helpers

        // Split the byte[] on a delimiter sequence (non-overlapping)
        private static List<byte[]> SplitOnSequence(byte[] data, byte[] delimiter)
        {
            var parts = new List<byte[]>();
            int start = 0;
            while (start < data.Length)
            {
                int idx = IndexOfSequence(data, delimiter, start);
                if (idx < 0)
                {
                    // last part
                    var tail = new byte[data.Length - start];
                    Array.Copy(data, start, tail, 0, tail.Length);
                    parts.Add(tail);
                    break;
                }

                // slice from start to idx
                var segment = new byte[idx - start];
                Array.Copy(data, start, segment, 0, segment.Length);
                parts.Add(segment);
                // move past delimiter
                start = idx + delimiter.Length;
            }
            return parts;
        }

        // Find the first occurrence of pattern in data starting at offset
        private static int IndexOfSequence(byte[] data, byte[] pattern, int offset)
        {
            for (int i = offset; i <= data.Length - pattern.Length; i++)
            {
                bool match = true;
                for (int j = 0; j < pattern.Length; j++)
                {
                    if (data[i + j] != pattern[j])
                    {
                        match = false;
                        break;
                    }
                }
                if (match) return i;
            }
            return -1;
        }

        // Trim a prefix sequence from the byte[] if present
        private static byte[] TrimStart(byte[] data, byte[] prefix)
        {
            if (IndexOfSequence(data, prefix, 0) == 0)
                return data.Skip(prefix.Length).ToArray();
            return data;
        }

        // Check if this part is the closing boundary (i.e. just "--boundary--")
        private static bool IsFinalBoundary(byte[] part, byte[] boundaryBytes)
        {
            // final boundary part will start with boundaryBytes + "--"
            if (part.Length < boundaryBytes.Length + 2) return false;
            if (!boundaryBytes.SequenceEqual(part.Take(boundaryBytes.Length))) return false;
            return part[boundaryBytes.Length] == (byte)'-'
                && part[boundaryBytes.Length + 1] == (byte)'-';
        }

        private static string Unquote(string s)
        {
            if (s.StartsWith("\"") && s.EndsWith("\""))
                return s.Substring(1, s.Length - 2);
            return s;
        }

    }
}
