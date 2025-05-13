using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Hybriona
{
    public static class HttpContentType
    {
        public const string Json = "application/json";
        public const string Xml = "application/xml";
        public const string Html = "text/html";
        public const string PlainText = "text/plain";
        public const string FormUrlEncoded = "application/x-www-form-urlencoded";
        public const string MultipartFormData = "multipart/form-data";
        public const string OctetStream = "application/octet-stream";
        public const string Pdf = "application/pdf";
        public const string Csv = "text/csv";
        public const string JavaScript = "application/javascript";
        public const string Css = "text/css";
        public const string Svg = "image/svg+xml";
        public const string Png = "image/png";
        public const string Jpeg = "image/jpeg";
        public const string Gif = "image/gif";
        public const string Webp = "image/webp";
    }

    public class HttpServer
    {
        private TcpListener _listener;
        private Thread _listenerThread;
        private int listenClientsDelayMS;
        // each route holds: the original pattern, the compiled regex, the param names, and the handler
        private class Route
        {
            public Regex Regex;
            public string[] ParamNames;
            public Action<HttpContext, Dictionary<string, string>> Handler;
        }

        private List<Route> _getRoutes = new List<Route>();
        private List<Route> _postRoutes = new List<Route>();

        public HttpServer(int port,int listenClientsDelayMS = 500)
        {
            this.listenClientsDelayMS = listenClientsDelayMS;
            HttpServerMainThreadDispatcher.Instance.Init();
            _listener = new TcpListener(IPAddress.Any, port);
           
        }

        public static Task RunInMainThread(System.Action action)
        {
            return HttpServerMainThreadDispatcher.Dispatch(action);
        }

        public void Start()
        {
           

            _listener.Start();
            _listener.Server.NoDelay = true;
            _listenerThread = new Thread(ListenForClients) { IsBackground = true };
            _listenerThread.Start();
        }

        public void Stop()
        {
            _listener.Stop();
            _listenerThread?.Abort();
        }

        private void ListenForClients()
        {
            while (true)
            {
                var client = _listener.AcceptTcpClient();
                new Thread(() => HandleClient(client)) { IsBackground = true }.Start();
                Thread.Sleep(this.listenClientsDelayMS);
            }
        }

        private void HandleClient(TcpClient client)
        {
            
            client.ReceiveTimeout = 5000;
            client.NoDelay = true;

            using (client)
            using (var stream = client.GetStream())
               
            using (var writer = new StreamWriter(stream) { NewLine = "\r\n", AutoFlush = true })
            {

                    // --- parse request lrine ---
                var requestLine = ReadLineFromStream(stream);
                if (string.IsNullOrEmpty(requestLine)) return;

                var parts = requestLine.Split(' ');
                var method = parts[0];
                var fullUrl = parts[1];                 // e.g. /user/123/profile/settings?foo=bar
                var urlParts = fullUrl.Split('?', 2);
                var path = urlParts[0];                 // e.g. /user/123/profile/settings
                var queryString = urlParts.Length > 1 ? urlParts[1] : "";

                // --- parse headers ---
                var headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                string line;
                while (!string.IsNullOrEmpty(line = ReadLineFromStream(stream))) 
                {
                    var kv = line.Split(new[] { ':' }, 2);
                    if (kv.Length == 2) headers[kv[0].Trim()] = kv[1].Trim();
                }

                // --- read body if present ---

                int contentLength = 0;

                // --- build context ---
                var context = new HttpContext
                {
                    Request = new HttpRequest
                    {
                        Method = method,
                        Path = path,
                        Query = ParseQueryString(queryString),
                        Headers = headers

                    },
                    Response = new HttpResponse(writer)
                };

                  
                if (headers.TryGetValue("Content-Length", out var lenStr) && int.TryParse(lenStr, out contentLength) && contentLength > 0)
                {

                    var bodyBuffer = new byte[contentLength];
                    int totalRead = 0;
                    while (totalRead < contentLength)
                    {
                            
                        int readedCount = stream.Read(bodyBuffer, totalRead, contentLength - totalRead);
                        if (readedCount <= 0)
                        {
                            break;
                        }
                        totalRead += readedCount;
                    }

                       
                    context.Request.Body = bodyBuffer;
                }



                // --- route matching ---
                var routes = method == "GET" ? _getRoutes : method == "POST" ? _postRoutes : null;
                if (routes != null)
                {
                    foreach (var rt in routes)
                    {
                        var m = rt.Regex.Match(path);
                        if (!m.Success) continue;

                        // extract named params
                        var paramValues = new Dictionary<string, string>();
                        for (int i = 0; i < rt.ParamNames.Length; i++)
                            paramValues[rt.ParamNames[i]] = Uri.UnescapeDataString(m.Groups[i + 1].Value);

                        rt.Handler(context, paramValues);
                        return;
                    }
                }

                // no route matched
                context.Response.SendResponse("404 Not Found", "text/html", HttpStatusCode.NotFound);
            }
            
        }

        // public API for registering routes
        public void Get(string pattern, Action<HttpContext, Dictionary<string, string>> handler)
            => _getRoutes.Add(BuildRoute(pattern, handler));

        public void Post(string pattern, Action<HttpContext, Dictionary<string, string>> handler)
            => _postRoutes.Add(BuildRoute(pattern, handler));

        // build regex and param-name list from a pattern like "/user/:id/profile/:section"
        private Route BuildRoute(string pattern, Action<HttpContext, Dictionary<string, string>> handler)
        {
            // find :paramName tokens
            var paramNames = Regex.Matches(pattern, @":(\w+)")
                                  .Cast<Match>()
                                  .Select(m => m.Groups[1].Value)
                                  .ToArray();

            // escape slashes, then replace ":param" with "([^/]+)"
            var regexPattern = "^" +
                Regex.Escape(pattern)
                     .Replace(@"\:", ":")
                     .Replace(@":\w+", "___PARAM___") // temporary
                + "$";

            // now swap placeholders with actual capture groups
            foreach (var name in paramNames)
                regexPattern = regexPattern.Replace($":{name}", "([^/]+)");

            var regex = new Regex(regexPattern, RegexOptions.Compiled);
            return new Route { Regex = regex, ParamNames = paramNames, Handler = handler };
        }

        private Dictionary<string, string> ParseQueryString(string qs)
        {
            var d = new Dictionary<string, string>();
            foreach (var part in qs.Split('&', StringSplitOptions.RemoveEmptyEntries))
            {
                var kv = part.Split('=', 2);
                var k = Uri.UnescapeDataString(kv[0]);
                var v = kv.Length > 1 ? Uri.UnescapeDataString(kv[1]) : "";
                d[k] = v;
            }
            return d;
        }

        public static string ReadLineFromStream(NetworkStream stream)
        {
            

            using var memoryStream = new MemoryStream();
            int readByte;

            while ((readByte = stream.ReadByte()) != -1)
            {
                // '\n' signifies the end of a line
                if (readByte == '\n')
                    break;

                // Avoid including '\r' if the line ends with "\r\n"
                if (readByte != '\r')
                    memoryStream.WriteByte((byte)readByte);
            }

            // End of stream reached with no data
            if (memoryStream.Length == 0 && readByte == -1)
                return null;

            return Encoding.UTF8.GetString(memoryStream.ToArray());
        }
    }

    // simple wrapper types
    public class HttpRequest
    {
        public string Method;
        public string Path;
        public Dictionary<string, string> Query;
        public Dictionary<string, string> Headers;
        public byte [] Body;

        public bool IsContentTypeForm()
        {
            return Headers.ContainsKey("Content-Type") && Headers["Content-Type"].Contains("multipart/form-data");
        }

        public string ContentType
        {
            get
            {
                return Headers.ContainsKey("Content-Type") ? Headers["Content-Type"] : null;
            }
        }
       
    }

    public class HttpContext : IDisposable
    {
        public HttpRequest Request;
        public HttpResponse Response;

        public void Dispose()
        {
            Request.Body = null;
            Request.Headers.Clear();
            Request.Headers = null;

            Request.Query.Clear();
            Request.Query = null;

            Response = null;
            Request = null;
            
            
        }
    }

    public class HttpResponse : IDisposable
    {
        private StreamWriter _writer;

        public void Dispose()
        {
            _writer.Close();
            _writer = null;
        }
        public HttpResponse(StreamWriter writer)
        {
            _writer = writer;
            // ensure CRLF line endings and auto‐flush
            _writer.NewLine = "\r\n";
           // _writer.AutoFlush = true;
        }

        /// <summary>
        /// Sends a text-based HTTP response (HTML, JSON, plain text, etc.).
        /// </summary>
        /// <param name="content">The body payload as a string.</param>
        /// <param name="contentType">MIME type (e.g. "text/html", "application/json").</param>
        /// <param name="statusCode">Custom HttpStatusCode enum.</param>
        public void SendResponse(
            string content,
            string contentType = "text/html",
            HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            byte[] bodyBytes = Encoding.UTF8.GetBytes(content);
            int code = (int)statusCode;
            string statusTxt = statusCode.ToString().Replace('_', ' ');

            // --- Status line ---
            _writer.WriteLine($"HTTP/1.1 {code} {statusTxt}");
            // --- Headers ---
            _writer.WriteLine($"Content-Type: {contentType}; charset=UTF-8");
            _writer.WriteLine($"Content-Length: {bodyBytes.Length}");
            _writer.WriteLine("Connection: close");
            _writer.WriteLine();  // blank line to signal end of headers

            // --- Body ---
            //_writer.Flush();       // ensure headers are sent immediately
            _writer.BaseStream.Write(bodyBytes, 0, bodyBytes.Length);
        }

        /// <summary>
        /// Sends a binary file (images, PDFs, etc.) as the HTTP response.
        /// </summary>
        /// <param name="filePath">Local filesystem path to the file.</param>
        /// <param name="contentType">MIME type (e.g. "image/png", "application/pdf").</param>
        /// <param name="statusCode">Custom HttpStatusCode enum.</param>
        public void SendFile(
            string filePath,
            string contentType = "application/octet-stream",
            HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            if (!File.Exists(filePath))
            {
                SendResponse("404 File Not Found", "text/html", HttpStatusCode.NotFound);
                return;
            }

            byte[] fileBytes = File.ReadAllBytes(filePath);
            int code = (int)statusCode;
            string statusTxt = statusCode.ToString().Replace('_', ' ');

            // --- Status line ---
            _writer.WriteLine($"HTTP/1.1 {code} {statusTxt}");
            // --- Headers ---
            _writer.WriteLine($"Content-Type: {contentType}");
            _writer.WriteLine($"Content-Length: {fileBytes.Length}");
            _writer.WriteLine("Connection: close");
            _writer.WriteLine();  // blank line

            // --- Body ---
            _writer.Flush();  // push headers
            _writer.BaseStream.Write(fileBytes, 0, fileBytes.Length);
        }

        /// <summary>
        /// Sends a binary file (images, PDFs, etc.) as the HTTP response.
        /// </summary>
        /// <param name="data">Byte Array.</param>
        /// <param name="contentType">MIME type (e.g. "image/png", "application/pdf").</param>
        /// <param name="statusCode">Custom HttpStatusCode enum.</param>
        public void SendBytes(
            byte [] data,
            string contentType = "application/octet-stream",
            HttpStatusCode statusCode = HttpStatusCode.OK)
        {
           
            int code = (int)statusCode;
            string statusTxt = statusCode.ToString().Replace('_', ' ');

            // --- Status line ---
            _writer.WriteLine($"HTTP/1.1 {code} {statusTxt}");
            // --- Headers ---
            _writer.WriteLine($"Content-Type: {contentType}");
            _writer.WriteLine($"Content-Length: {data.Length}");
            _writer.WriteLine("Connection: close");
            _writer.WriteLine();  // blank line

            // --- Body ---
            _writer.Flush();  // push headers
            _writer.BaseStream.Write(data, 0, data.Length);
        }
    }

    // your custom status codes
    public enum HttpStatusCode
    {
        // 1xx Informational
        Continue = 100,
        SwitchingProtocols = 101,
        Processing = 102,
        EarlyHints = 103,

        // 2xx Success
        OK = 200,
        Created = 201,
        Accepted = 202,
        NonAuthoritativeInformation = 203,
        NoContent = 204,
        ResetContent = 205,
        PartialContent = 206,
        MultiStatus = 207,
        AlreadyReported = 208,
        IMUsed = 226,

        // 3xx Redirection
        MultipleChoices = 300,
        MovedPermanently = 301,
        Found = 302,
        SeeOther = 303,
        NotModified = 304,
        UseProxy = 305,
        TemporaryRedirect = 307,
        PermanentRedirect = 308,

        // 4xx Client Errors
        BadRequest = 400,
        Unauthorized = 401,
        PaymentRequired = 402,
        Forbidden = 403,
        NotFound = 404,
        MethodNotAllowed = 405,
        NotAcceptable = 406,
        ProxyAuthenticationRequired = 407,
        RequestTimeout = 408,
        Conflict = 409,
        Gone = 410,
        LengthRequired = 411,
        PreconditionFailed = 412,
        PayloadTooLarge = 413,
        URITooLong = 414,
        UnsupportedMediaType = 415,
        RangeNotSatisfiable = 416,
        ExpectationFailed = 417,
        ImATeapot = 418,
        MisdirectedRequest = 421,
        UnprocessableEntity = 422,
        Locked = 423,
        FailedDependency = 424,
        TooEarly = 425,
        UpgradeRequired = 426,
        PreconditionRequired = 428,
        TooManyRequests = 429,
        RequestHeaderFieldsTooLarge = 431,
        UnavailableForLegalReasons = 451,

        // 5xx Server Errors
        InternalServerError = 500,
        NotImplemented = 501,
        BadGateway = 502,
        ServiceUnavailable = 503,
        GatewayTimeout = 504,
        HTTPVersionNotSupported = 505,
        VariantAlsoNegotiates = 506,
        InsufficientStorage = 507,
        LoopDetected = 508,
        NotExtended = 510,
        NetworkAuthenticationRequired = 511
    }
}
