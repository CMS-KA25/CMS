namespace CMS.Domain.Shared.Exceptions
{
    public class ValidationException : BaseException
    {
        public ValidationException(string message) : base(message, "VALIDATION_ERROR") { }
        
        public ValidationException(string field, string message) 
            : base($"Validation failed for {field}: {message}", "VALIDATION_ERROR") { }
    }
}