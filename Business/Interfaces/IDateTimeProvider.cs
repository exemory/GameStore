namespace Business.Interfaces;

public interface IDateTimeProvider
{
    public DateTimeOffset UtcNow { get; }
}