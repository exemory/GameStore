using AutoFixture;
using AutoMapper;
using Business.DataTransferObjects;
using Business.Exceptions;
using Business.Services;
using Data.Entities;
using Data.Interfaces;
using ExpectedObjects;
using FluentAssertions;
using Force.DeepCloner;
using Microsoft.AspNetCore.Identity;
using NSubstitute;
using Xunit;

namespace Business.Tests.ServiceTests;

public class UserServiceTests : TestsBase
{
    private readonly UserService _sut;

    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IMapper _mapper = UnitTestHelper.CreateMapper();

    private readonly UserManager<User> _userManager =
        Substitute.For<UserManager<User>>(Substitute.For<IUserStore<User>>(), null, null, null, null, null, null, null,
            null);

    public UserServiceTests()
    {
        _sut = new UserService(_unitOfWork, _mapper, _userManager);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllUsers()
    {
        // Arrange
        var users = Fixture.Build<User>()
            .Without(u => u.Comments)
            .CreateMany();
        var mappedUsers = _mapper.Map<IEnumerable<UserInfoDto>>(users);

        _unitOfWork.UserRepository.GetAllAsync().Returns(users);

        // Act
        var result = await _sut.GetAllAsync();

        // Assert
        result.Should().BeEquivalentTo(mappedUsers);
    }

    [Fact]
    public async Task ModifyUserRoles_ShouldAddManagerRoleToUser()
    {
        // Arrange
        var userRolesUpdateDto = Fixture.Build<UserRolesUpdateDto>()
            .With(r => r.Manager, true)
            .Create();
        var user = Fixture.Build<User>()
            .Without(u => u.Comments)
            .Create();
        var expectedUser = user.ToExpectedObject();

        _userManager.FindByIdAsync(user.Id.ToString()).Returns(user.DeepClone());

        // Act
        await _sut.UpdateRolesAsync(user.Id, userRolesUpdateDto);

        // Assert
        await _userManager.Received()
            .AddToRoleAsync(Arg.Is<User>(u => expectedUser.Equals(u)), UserService.ManagerRole);
    }

    [Fact]
    public async Task ModifyUserRoles_ShouldRemoveManagerRoleFromUser()
    {
        // Arrange
        var userRolesUpdateDto = Fixture.Build<UserRolesUpdateDto>()
            .With(r => r.Manager, false)
            .Create();
        var user = Fixture.Build<User>()
            .Without(u => u.Comments)
            .Create();
        var expectedUser = user.ToExpectedObject();

        _userManager.FindByIdAsync(user.Id.ToString()).Returns(user.DeepClone());

        // Act
        await _sut.UpdateRolesAsync(user.Id, userRolesUpdateDto);

        // Assert
        await _userManager.Received()
            .RemoveFromRoleAsync(Arg.Is<User>(u => expectedUser.Equals(u)), UserService.ManagerRole);
    }

    [Fact]
    public async Task ModifyUserRoles_ShouldFail_WhenUserDoesNotExist()
    {
        // Arrange
        var userRolesUpdateDto = Fixture.Create<UserRolesUpdateDto>();
        var nonexistentUserId = Fixture.Create<Guid>();

        // Act
        var result = () => _sut.UpdateRolesAsync(nonexistentUserId, userRolesUpdateDto);

        // Assert
        await result.Should().ThrowExactlyAsync<NotFoundException>();

        await _userManager.DidNotReceive().AddToRoleAsync(Arg.Any<User>(), Arg.Any<string>());
        await _userManager.DidNotReceive().RemoveFromRoleAsync(Arg.Any<User>(), Arg.Any<string>());
    }
}