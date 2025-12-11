namespace CMS.Domain.Shared.Exceptions
{
    public abstract class BaseException : Exception
    {
        public string ErrorCode { get; }

        protected BaseException(string message, string errorCode) : base(message)
        {
            ErrorCode = errorCode;
        }

        protected BaseException(string message, string errorCode, Exception innerException) 
            : base(message, innerException)
        {
            ErrorCode = errorCode;
        }
    }
}