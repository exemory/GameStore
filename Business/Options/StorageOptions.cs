namespace Business.Options;

public class StorageOptions
{
    private static readonly string AppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

    private string? _gameImagesFolderPath;
    private string? _userAvatarsFolderPath;

    public string AppFolderName { get; set; } = default!;
    public string GameImagesFolderName { get; set; } = default!;
    public string UserAvatarsFolderName { get; set; } = default!;
    public string[] SupportedImageExtensions { get; set; } = default!;

    public string GameImagesFolderPath =>
        _gameImagesFolderPath ??= Path.Combine(AppDataPath, AppFolderName, GameImagesFolderName);

    public string UserAvatarsFolderPath =>
        _userAvatarsFolderPath ??= Path.Combine(AppDataPath, AppFolderName, UserAvatarsFolderName);
}