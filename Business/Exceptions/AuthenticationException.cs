using System.Runtime.Serialization;

namespace Business.Exceptions
{
    /// <summary>
    /// The exception that is thrown when authentication failed
    /// </summary>
    [Serializable]
    public class AuthenticationException : GameStoreException
    {
        public AuthenticationException()
        {
        }

        protected AuthenticationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public AuthenticationException(string message) : base(message)
        {
        }

        public AuthenticationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}