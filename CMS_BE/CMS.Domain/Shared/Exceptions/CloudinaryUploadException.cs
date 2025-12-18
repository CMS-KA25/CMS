namespace CMS.Domain.Shared.Exceptions
{
    public class CloudinaryUploadException : Exception
    {
        public CloudinaryUploadException(string message) : base(message) { }
        public CloudinaryUploadException(string message, Exception innerException) : base(message, innerException) { }
    }
}