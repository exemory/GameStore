using Business.Exceptions;
using Business.Interfaces;
using Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Business.Services;

public class AvatarService : IAvatarService
{
    private readonly IStorageService _storageService;
    private readonly UserManager<User> _userManager;
    private readonly ISession _session;
    private readonly ILogger<AvatarService> _logger;

    public AvatarService(IStorageService storageService, UserManager<User> userManager, ISession session,
        ILogger<AvatarService> logger)
    {
        _storageService = storageService;
        _userManager = userManager;
        _session = session;
        _logger = logger;
    }

    public async Task UploadAvatarImageAsync(Stream fileStream, string originalFileName)
    {
        var user = await _userManager.FindByIdAsync(_session.UserId.ToString());
        if (user == null)
        {
            _logger.LogError("Authorized user with id '{UserId}' does not exist", _session.UserId);
            throw new AccessDeniedException("User is not authorized.");
        }
        
        var imageFileName = await _storageService.StoreUserAvatarAsync(fileStream, originalFileName);

        user.Avatar = imageFileName;

        await _userManager.UpdateAsync(user);
    }

    public async Task<(Stream FileStream, string FileName)> GetAvatarImageAsync()
    {
        var user = await _userManager.FindByIdAsync(_session.UserId.ToString());
        if (user == null)
        {
            _logger.LogError("Authorized user with id '{UserId}' does not exist", _session.UserId);
            throw new AccessDeniedException("User is not authorized.");
        }

        if (user.Avatar == null)
        {
            throw new NotFoundException("User has not avatar.");
        }

        Stream fileStream;

        try
        {
            fileStream = _storageService.GetUserAvatar(user.Avatar);
        }
        catch (NotFoundException)
        {
            _logger.LogError("User's avatar '{Avatar}' not found", user.Avatar);
            throw new NotFoundException("User's avatar not found.");
        }

        return (fileStream, user.Avatar);
    }
}