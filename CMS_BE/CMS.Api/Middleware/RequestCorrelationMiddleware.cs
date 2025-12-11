using Serilog.Context;

namespace CMS.Api.Middleware
{
    public class RequestCorrelationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestCorrelationMiddleware> _logger;

        public RequestCorrelationMiddleware(RequestDelegate next, ILogger<RequestCorrelationMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var headerValue = context.Request.Headers["X-Correlation-Id"].FirstOrDefault();
            var correlationId = Guid.TryParse(headerValue, out var parsedId) ? parsedId : Guid.NewGuid();

            context.Items["CorrelationId"] = correlationId;
            context.Response.Headers.Append("X-Correlation-Id", correlationId.ToString());

            using (LogContext.PushProperty("CorrelationId", correlationId))
            {
                _logger.LogInformation("Request started: {Method} {Path}", 
                    context.Request.Method, context.Request.Path);

                try
                {
                    await _next(context);
                    
                    _logger.LogInformation("Request completed: {Method} {Path} {StatusCode}", 
                        context.Request.Method, context.Request.Path, context.Response.StatusCode);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Request failed: {Method} {Path}", 
                        context.Request.Method, context.Request.Path);
                    throw;
                }
            }
        }
    }
}