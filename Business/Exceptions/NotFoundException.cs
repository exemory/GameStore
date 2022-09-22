using System.Runtime.Serialization;

namespace Business.Exceptions;

/// <summary>
/// The exception that is thrown when something not found
/// </summary>
[Serializable]
public class NotFoundException : GameStoreException
{
    public NotFoundException()
    {
    }

    protected NotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public NotFoundException(string? message) : base(message)
    {
    }

    public NotFoundException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}