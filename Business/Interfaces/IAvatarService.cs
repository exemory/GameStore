using Business.Exceptions;

namespace Business.Interfaces;

public interface IAvatarService
{
    /// <summary>
    /// Store the uploaded user avatar image and bind it to the authorized user
    /// </summary>
    /// <param name="fileStream">Stream of the image file</param>
    /// <param name="originalFileName">User-specified name of the uploading image</param>
    /// <exception cref="AccessDeniedException">
    /// The user is not authorized
    /// </exception>
    /// <exception cref="GameStoreException">
    /// Thrown when the image file extension is not supported
    /// </exception>
    public Task UploadAvatarImageAsync(Stream fileStream, string originalFileName);

    /// <summary>
    /// Get user avatar image
    /// </summary>
    /// <returns>Tuple that contains the image file stream and file name</returns>
    /// <exception cref="AccessDeniedException">
    /// The user is not authorized
    /// </exception>
    /// <exception cref="NotFoundException">
    /// Thrown when:
    /// <list type="bullet">
    /// <item><description>The user has not avatar</description></item>
    /// <item><description>The user's avatar not found</description></item>
    /// </list>
    /// </exception>
    public Task<(Stream FileStream, string FileName)> GetAvatarImageAsync();
}