﻿using Business.Exceptions;

namespace Business.Interfaces;

/// <summary>
/// Service for storage and retrieving files
/// </summary>
public interface IStorageService
{
    /// <summary>
    /// Check and throw <see cref="NotFoundException"/> if game image does not exist
    /// </summary>
    /// <param name="gameImageName">File name of the game image which need to be checked</param>
    /// <exception cref="NotFoundException">
    /// Thrown when the game image specified by <paramref name="gameImageName"/> does not exist
    /// </exception>
    public void CheckIfGameImageExists(string gameImageName);

    /// <summary>
    /// Store the game image
    /// </summary>
    /// <param name="fileStream">File stream of the game image</param>
    /// <param name="originalFileName">User-specified image file name</param>
    /// <returns>File name the stored image</returns>
    /// <exception cref="GameStoreException">
    /// Thrown when the image file extension is not supported
    /// </exception>
    public Task<string> StoreGameImageAsync(Stream fileStream, string originalFileName);

    /// <summary>
    /// Get the game image specified by <paramref name="gameImageName"/>
    /// </summary>
    /// <param name="gameImageName">File name the game image which need to be retrieved</param>
    /// <returns>File stream of the game image</returns>
    /// <exception cref="NotFoundException">
    /// Thrown when the game image specified by <paramref name="gameImageName"/> does not exist
    /// </exception>
    public Stream GetGameImage(string gameImageName);
}