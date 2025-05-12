using UnityEngine;
using Hybriona;

public class HttpServerExample : MonoBehaviour
{
    private HttpServer _server;

    void Start()
    {
        _server = new HttpServer("127.0.0.1", 8081);

        // Handle GET requests
        _server.Get("/home/:userid/profile", (context, routeParams) =>
        {
            context.Response.SendResponse($"<html><body><h1>Welcome  {routeParams["userid"]} to Home!</h1></body></html>", "text/html", HttpStatusCode.OK);
        });

        _server.Get("/home", (context, routeParams) =>
        {
           
            context.Response.SendResponse("<html><body><h1>Welcome to Home!</h1></body></html>", "text/html", HttpStatusCode.OK);
        });

        _server.Get("/upload", (ctx, routeParams) =>
        {
            // Build a multipart/form-data upload form
            string html = @"
            <html>
              <body>
                <h1>Upload Form</h1>
                <form action=""/handle_upload"" method=""post"" enctype=""multipart/form-data"">
                  <!-- Two text fields -->
                  <label for=""text1"">Text Field 1:</label>
                  <input type=""text"" name=""text1"" id=""text1"" /><br/><br/>
          
                  <label for=""text2"">Text Field 2:</label>
                  <input type=""text"" name=""text2"" id=""text2"" /><br/><br/>
          
                  <!-- Two file inputs -->
                  <label for=""file1"">File 1:</label>
                  <input type=""file"" name=""file1"" id=""file1"" /><br/><br/>
          
                  <label for=""file2"">File 2:</label>
                  <input type=""file"" name=""file2"" id=""file2"" /><br/><br/>
          
                  <button type=""submit"">Upload</button>
                </form>
              </body>
            </html>";

            ctx.Response.SendResponse(html, "text/html", HttpStatusCode.OK);
        });


        _server.Post("/handle_upload", (context, routeParams) =>
        {
            if(context.Request.IsContentTypeForm())
            {
                Debug.Log("handle_upload");
                var formData =RequestBodyParser.Parse(context.Request.Body, context.Request.ContentType);

                string fileData = "";
                if(formData.Files.Count > 0)
                {
                    var file = formData.Files[0];
                    fileData += file.FileName + " " + file.Data.Length + " bytes";

                }
                context.Response.SendResponse($"<html><body><pre>{formData.Fields["text1"]}\n{formData.Fields["text2"]}\n\n{fileData}</pre></body></html>", "text/html", HttpStatusCode.OK);
            }
            else
            {
                context.Response.SendResponse($"<html><body>Not found any data {context.Request.ContentType}</body></html>", "text/html", HttpStatusCode.OK);
            }
            
        });

        // Handle POST for file upload
        //_server.Post("/upload_file", FileUploadHandler.HandleFileUpload);

        // Start the server
        _server.Start();
        Debug.Log("Server started");
    }

    void OnDisable()
    {
        // Optional: Add stop logic to your HttpServer class if needed
         _server.Stop();
    }
}
