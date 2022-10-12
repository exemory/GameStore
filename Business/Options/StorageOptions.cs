namespace Business.Options;

public class StorageOptions
{
    private static readonly string AppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
    
    public string AppFolderName { get; set; } = default!;
    public string GameImagesFolderName { get; set; } = default!;

    public string[] SupportedImageExtensions { get; set; } = default!;


    private string? _gameImagesFolderPath;
    public string GameImagesFolderPath => _gameImagesFolderPath ??= Path.Combine(AppDataPath, AppFolderName, GameImagesFolderName);
}