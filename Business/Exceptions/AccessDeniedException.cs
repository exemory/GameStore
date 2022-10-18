using System.Runtime.Serialization;

namespace Business.Exceptions
{
    /// <summary>
    /// The exception that is thrown when access to operation is denied
    /// </summary>
    [Serializable]
    public class AccessDeniedException : GameStoreException
    {
        public AccessDeniedException()
        {
        }

        protected AccessDeniedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public AccessDeniedException(string message) : base(message)
        {
        }

        public AccessDeniedException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}