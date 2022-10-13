using System.Runtime.Serialization;

namespace Business.Exceptions
{
    /// <summary>
    /// The exception that is thrown when registration failed
    /// </summary>
    [Serializable]
    public class RegistrationException : GameStoreException
    {
        public RegistrationException()
        {
        }

        protected RegistrationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public RegistrationException(string message) : base(message)
        {
        }

        public RegistrationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}