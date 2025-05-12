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

        // Handle POST for file upload
        //_server.Post("/upload_file", FileUploadHandler.HandleFileUpload);

        // Start the server
        _server.Start();
        Debug.Log("Server started");
    }

    void OnApplicationQuit()
    {
        // Optional: Add stop logic to your HttpServer class if needed
         _server.Stop();
    }
}
