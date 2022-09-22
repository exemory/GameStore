using System.Runtime.Serialization;

namespace Business.Exceptions;

/// <summary>
/// Represents the general application error
/// </summary>
[Serializable]
public class GameStoreException : Exception
{
    public GameStoreException()
    {
    }

    protected GameStoreException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public GameStoreException(string? message) : base(message)
    {
    }

    public GameStoreException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}