using CTMS.DataModel.Models;

namespace CTMS.Middlewares
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, RequestInformation requestInformation)
        {
            // Log the request path
            Console.WriteLine($"Request Path: {context.Request.Path}");
            string url = $"{context.Request.Scheme}://{context.Request.Host}";
            requestInformation.Url = url;
            // Call the next middleware in the pipeline
            await _next(context);
        }
    }
}
