using Business.Exceptions;
using Business.Interfaces;
using Business.Options;
using Microsoft.Extensions.Options;

namespace Business.Services;

/// <inheritdoc />
public class StorageService : IStorageService
{
    private readonly StorageOptions _options;

    /// <summary>
    /// Constructor for initializing a <see cref="StorageService"/> class instance
    /// </summary>
    /// <param name="options">Storage options</param>
    public StorageService(IOptions<StorageOptions> options)
    {
        _options = options.Value;
    }

    public void CheckIfGameImageExists(string gameImageName)
    {
        var gameImagePath = Path.Combine(_options.GameImagesFolderPath, gameImageName);
        var imageExists = File.Exists(gameImagePath);

        if (!imageExists)
        {
            throw new NotFoundException($"Game image with name '{gameImageName}' not found.");
        }
    }

    public async Task<string> StoreGameImageAsync(Stream fileStream, string originalFileName)
    {
        ValidateImageFileExtension(originalFileName);

        var imageId = Guid.NewGuid();
        var fileExtension = GetFileExtension(originalFileName);
        var gameImageName = $"{imageId}{fileExtension}";

        var imageFilePath = Path.Combine(_options.GameImagesFolderPath, gameImageName);
        await using var imageFileStream = new FileStream(imageFilePath, FileMode.Create);

        await fileStream.CopyToAsync(imageFileStream);

        return gameImageName;
    }

    public Stream GetGameImage(string gameImageName)
    {
        CheckIfGameImageExists(gameImageName);

        var imageFilePath = Path.Combine(_options.GameImagesFolderPath, gameImageName);
        var fileStream = new FileStream(imageFilePath, FileMode.Open);

        return fileStream;
    }

    private static string GetFileExtension(string fileName)
    {
        return Path.GetExtension(fileName);
    }

    private void ValidateImageFileExtension(string fileName)
    {
        var extension = GetFileExtension(fileName);

        var isSupported = _options.SupportedImageExtensions.Contains(extension);

        if (!isSupported)
        {
            throw new GameStoreException($"Image file extension '{extension}' is not supported.");
        }
    }
}