using AutoMapper;
using Business.DataTransferObjects;
using Business.Exceptions;
using Business.Interfaces;
using Data.Entities;
using Data.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Business.Services;

/// <inheritdoc />
public class UserService : IUserService
{
    public const string ManagerRole = "Manager";

    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly UserManager<User> _userManager;

    /// <summary>
    /// Constructor for initializing a <see cref="GameService"/> class instance
    /// </summary>
    /// <param name="unitOfWork">Unit of work</param>
    /// <param name="mapper">Mapper</param>
    /// <param name="userManager">Identity user manager</param>
    public UserService(IUnitOfWork unitOfWork, IMapper mapper, UserManager<User> userManager)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _userManager = userManager;
    }

    public async Task<IEnumerable<UserInfoDto>> GetAllAsync()
    {
        var users = await _unitOfWork.UserRepository.GetAllAsync();

        return _mapper.Map<IEnumerable<UserInfoDto>>(users);
    }

    public async Task UpdateRolesAsync(Guid userId, UserRolesUpdateDto userRolesUpdateDto)
    {
        var user = await GetUserAsync(userId);

        if (userRolesUpdateDto.Manager)
        {
            await _userManager.AddToRoleAsync(user, ManagerRole);
        }
        else
        {
            await _userManager.RemoveFromRoleAsync(user, ManagerRole);
        }
    }

    private async Task<User> GetUserAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user == null)
        {
            throw new NotFoundException($"User with id '{userId}' not found");
        }

        return user;
    }
}