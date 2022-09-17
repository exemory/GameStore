namespace Business.DataTransferObjects;

public class GameDto
{
    public Guid Id { get; set; }
    public string Key { get; set; } = default!;
    public string Name { get; set; } = default!;
}