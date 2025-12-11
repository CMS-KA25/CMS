namespace CMS.Domain.Shared.Exceptions
{
    public class ForbiddenAccessException : BaseException
    {
        public ForbiddenAccessException(string message = "Access denied") : base(message, "FORBIDDEN") { }
        
        public ForbiddenAccessException(string resource, string action) 
            : base($"Access denied for {action} on {resource}", "FORBIDDEN") { }
    }
}