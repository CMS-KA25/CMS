using CMS.Application.Shared.DTOs;
using CMS.Domain.Shared.Exceptions;
using System.Text.Json;

namespace CMS.Api.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            if (context.Response.HasStarted)
                return;

            context.Response.ContentType = "application/json";

            var correlationId = context.Items["CorrelationId"] switch
            {
                Guid guid => guid,
                string s when Guid.TryParse(s, out var parsedGuid) => parsedGuid,
                _ => Guid.NewGuid()
            };

            var response = new ApiResponse<object>
            {
                Success = false,
                Data = null,
                CorrelationId = correlationId,
                Timestamp = DateTime.UtcNow
            };

            switch (exception)
            {
                case NotFoundException:
                    context.Response.StatusCode = 404;
                    response.Message = exception.Message;
                    response.ErrorCode = "NOT_FOUND";
                    break;

                case ForbiddenAccessException:
                    context.Response.StatusCode = 403;
                    response.Message = exception.Message;
                    response.ErrorCode = "FORBIDDEN";
                    break;

                case ValidationException:
                    context.Response.StatusCode = 400;
                    response.Message = exception.Message;
                    response.ErrorCode = "VALIDATION_ERROR";
                    break;

                default:
                    context.Response.StatusCode = 500;
                    response.Message = "Internal server error";
                    response.ErrorCode = "INTERNAL_ERROR";
                    break;
            }

            var json = JsonSerializer.Serialize(response, JsonOptions);
            await context.Response.WriteAsync(json);
        }
    }
}