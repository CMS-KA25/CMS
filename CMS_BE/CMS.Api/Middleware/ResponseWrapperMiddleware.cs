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

            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            await _next(context);

            context.Response.Body = originalBodyStream;
            responseBody.Seek(0, SeekOrigin.Begin);

            var response = await new StreamReader(responseBody).ReadToEndAsync();
            var correlationId = (Guid)(context.Items["CorrelationId"] ?? Guid.NewGuid());

            if (context.Response.ContentType?.Contains("application/json") == true && !string.IsNullOrEmpty(response))
            {
                var wrappedResponse = new ApiResponse<object>
                {
                    Success = context.Response.StatusCode < 400,
                    Data = JsonSerializer.Deserialize<object>(response),
                    CorrelationId = correlationId,
                    Timestamp = DateTime.UtcNow
                };

                var jsonResponse = JsonSerializer.Serialize(wrappedResponse);
                await context.Response.WriteAsync(jsonResponse);
            }
            else
            {
                await responseBody.CopyToAsync(originalBodyStream);
            }
        }
    }
}