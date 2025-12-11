namespace CMS.Domain.Shared.Exceptions
{
    public class NotFoundException : BaseException
    {
        public NotFoundException(string message) : base(message, "NOT_FOUND") { }
        
        public NotFoundException(string resourceName, object key) 
            : base($"{resourceName} with key '{key}' was not found", "NOT_FOUND") { }
    }
}