namespace Business.DataTransferObjects;

public class GameCreationDto
{
    public string Key { get; set; } = default!;
    public string? Name { get; set; }
    public string? Description { get; set; }
}