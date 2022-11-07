using SISGED.Shared.DTOs;
using System.Text;

namespace SISGED.Server.Helpers.Middlewares
{

    public static class HttpLoggerMiddlewareExtensions
    {
        public static IApplicationBuilder UseHttpLogger(this IApplicationBuilder app)
        {
            return app.UseMiddleware<HttpLoggerMiddleware>();
        }
    }

    public class HttpLoggerMiddleware
    {
        private readonly ILogger<HttpLoggerMiddleware> _httpLogger;
        private readonly RequestDelegate _requestDelegate;

        private List<string> httpBodyMethods = new()
        {
            "POST",
            "PUT",
            "PATCH"
        };

        private List<string> httpResponseUrls = new()
        {
            "html",
            "js",
            "css",
            "json",
            "png",
            "jpg",
            "jpeg",
            "webp"
        };

        public HttpLoggerMiddleware(ILogger<HttpLoggerMiddleware> httpLogger, RequestDelegate requestDelegate)
        {
            _httpLogger = httpLogger;
            _requestDelegate = requestDelegate;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            using var memoryStream = new MemoryStream();

            var originalResponseBody = context.Response.Body;
            context.Response.Body = memoryStream;

            if (httpBodyMethods.Contains(context.Request.Method)) await WriteHtttpRequestBodyAsync(context);

            await _requestDelegate(context);

            if (memoryStream.Length > 0) await WriteHttpResponseBodyAsync(new HttpResponseWriterDTO(originalResponseBody, context, memoryStream));

        }

        /// <summary>
        /// Gets and Writes the http request body 
        /// </summary>
        /// <param name="context">Http Context for any request</param>
        /// <returns>A task that specifies if the http request body could be written successfully</returns>
        private async Task WriteHtttpRequestBodyAsync(HttpContext context)
        {
            var request = context.Request;

            request.EnableBuffering();

            var requestBuffer = new byte[Convert.ToInt32(request.ContentLength)];

            await request.Body.ReadAsync(requestBuffer);

            var requestBody = Encoding.UTF8.GetString(requestBuffer);

            request.Body.Position = 0;

            _httpLogger.LogInformation("Http Request Body: {requestBody}", requestBody);

        }

        /// <summary>
        /// Gets and Writes the http response body 
        /// </summary>
        /// <param name="httpResponseWriterDTO">Contains all information related to the httpContext for the response</param>
        /// <returns>A task that specifies if the http response body could be written successfully</returns>
        private async Task WriteHttpResponseBodyAsync(HttpResponseWriterDTO httpResponseWriterDTO)

        {
            httpResponseWriterDTO.HttpMemoryStream.Seek(0, SeekOrigin.Begin);

            var responseBody = new StreamReader(httpResponseWriterDTO.HttpMemoryStream).ReadToEnd();

            httpResponseWriterDTO.HttpMemoryStream.Seek(0, SeekOrigin.Begin);

            await httpResponseWriterDTO.HttpMemoryStream.CopyToAsync(httpResponseWriterDTO.OriginalStream);

            httpResponseWriterDTO.Context.Response.Body = httpResponseWriterDTO.OriginalStream;

            if (httpResponseUrls.Contains(httpResponseWriterDTO.Context.Request.Path.Value!)
                || httpResponseWriterDTO.Context.Request.Path.Value!.Contains("framework")) return;

            if (httpResponseWriterDTO.Context.Response.StatusCode.ToString().StartsWith("2")) _httpLogger.LogInformation("Http Response Body: {responseBody}", responseBody);
            else _httpLogger.LogError("Http Response Exception Message: {responseBody}", responseBody);
        }

    }
}
