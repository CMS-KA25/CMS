using CMS.Application.Shared.DTOs;
using System.Text.Json;

namespace CMS.Api.Middleware
{
    public class ResponseWrapperMiddleware
    {
        private readonly RequestDelegate _next;

        public ResponseWrapperMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var originalBodyStream = context.Response.Body;
            var responseBody = new MemoryStream();

            // Replace the response body so we can inspect/transform it. Ensure original
            // body is restored even if downstream throws so exception handlers can write
            // to the real response stream.
            context.Response.Body = responseBody;
            try
            {
                await _next(context);

                // Move to start and read captured response
                responseBody.Seek(0, SeekOrigin.Begin);
                var responseText = await new StreamReader(responseBody).ReadToEndAsync();
                var correlationId = (Guid)(context.Items["CorrelationId"] ?? Guid.NewGuid());

                // Restore original body before writing the final output
                context.Response.Body = originalBodyStream;

                if (context.Response.ContentType?.Contains("application/json") == true && !string.IsNullOrEmpty(responseText))
                {
                    var wrappedResponse = new ApiResponse<object>
                    {
                        Success = context.Response.StatusCode < 400,
                        Data = JsonSerializer.Deserialize<object>(responseText),
                        CorrelationId = correlationId,
                        Timestamp = DateTime.UtcNow
                    };

                    var jsonResponse = JsonSerializer.Serialize(wrappedResponse);
                    await context.Response.WriteAsync(jsonResponse);
                }
                else
                {
                    // For non-json responses, copy through the original bytes
                    responseBody.Seek(0, SeekOrigin.Begin);
                    await responseBody.CopyToAsync(originalBodyStream);
                }
            }
            finally
            {
                // Ensure the response.Body is the original stream so further middleware
                // (e.g., exception handler) can write to it.
                context.Response.Body = originalBodyStream;
                responseBody.Dispose();
            }
        }
    }
}