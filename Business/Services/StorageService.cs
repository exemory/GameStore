using Business.Exceptions;
using Business.Interfaces;
using Business.Options;
using Microsoft.Extensions.Options;

namespace Business.Services;

/// <inheritdoc />
public class StorageService : IStorageService
{
    private readonly StorageOptions _storageOptions;

    /// <summary>
    /// Constructor for initializing a <see cref="StorageService"/> class instance
    /// </summary>
    /// <param name="options">Storage options</param>
    public StorageService(IOptions<StorageOptions> options)
    {
        _storageOptions = options.Value;
    }

    public void CheckIfGameImageExists(string gameImageName)
    {
        var exists = DoesFileExist(gameImageName, _storageOptions.GameImagesFolderPath);

        if (!exists)
        {
            throw new NotFoundException($"Game image with name '{gameImageName}' not found.");
        }
    }

    public async Task<string> StoreGameImageAsync(Stream fileStream, string originalFileName)
    {
        return await StoreImage(fileStream, originalFileName, _storageOptions.GameImagesFolderPath);
    }

    public Stream GetGameImage(string gameImageName)
    {
        CheckIfGameImageExists(gameImageName);
        return GetFile(gameImageName, _storageOptions.GameImagesFolderPath);
    }

    public void CheckIfUserAvatarExists(string userAvatarName)
    {
        var exists = DoesFileExist(userAvatarName, _storageOptions.UserAvatarsFolderPath);

        if (!exists)
        {
            throw new NotFoundException($"User avatar image with name '{userAvatarName}' not found.");
        }
    }

    public async Task<string> StoreUserAvatarAsync(Stream fileStream, string originalFileName)
    {
        return await StoreImage(fileStream, originalFileName, _storageOptions.UserAvatarsFolderPath);
    }

    private static bool DoesFileExist(string fileName, string folderPath)
    {
        var filePath = Path.Combine(folderPath, fileName);
        return File.Exists(filePath);
    }

    public Stream GetUserAvatar(string userAvatarName)
    {
        CheckIfUserAvatarExists(userAvatarName);
        return GetFile(userAvatarName, _storageOptions.UserAvatarsFolderPath);
    }

    private static FileStream GetFile(string fileName, string folderPath)
    {
        var filePath = Path.Combine(folderPath, fileName);
        return new FileStream(filePath, FileMode.Open);
    }

    private async Task<string> StoreImage(Stream fileStream, string originalFileName, string folderPath)
    {
        ValidateImageFileExtension(originalFileName);

        var fileId = Guid.NewGuid();
        var fileExtension = GetFileExtension(originalFileName);
        var fileName = $"{fileId}{fileExtension}";

        var filePath = Path.Combine(folderPath, fileName);
        await using var imageFileStream = new FileStream(filePath, FileMode.Create);

        await fileStream.CopyToAsync(imageFileStream);
        return fileName;
    }

    private static string GetFileExtension(string fileName)
    {
        return Path.GetExtension(fileName);
    }

    private void ValidateImageFileExtension(string fileName)
    {
        var extension = GetFileExtension(fileName);

        var isSupported = _storageOptions.SupportedImageExtensions.Contains(extension);

        if (!isSupported)
        {
            throw new GameStoreException($"Image file extension '{extension}' is not supported.");
        }
    }
}