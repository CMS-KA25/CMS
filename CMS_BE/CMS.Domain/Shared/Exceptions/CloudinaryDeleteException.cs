namespace CMS.Domain.Shared.Exceptions
{
    public class CloudinaryDeleteException : Exception
    {
        public CloudinaryDeleteException(string message) : base(message) { }
        public CloudinaryDeleteException(string message, Exception innerException) : base(message, innerException) { }
    }
}