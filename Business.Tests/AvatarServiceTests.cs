using AutoFixture;
using Business.Exceptions;
using Business.Interfaces;
using Business.Services;
using Data.Entities;
using ExpectedObjects;
using FluentAssertions;
using Force.DeepCloner;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NSubstitute.ReturnsExtensions;
using Xunit;

namespace Business.Tests;

public class AvatarServiceTests
{
    private readonly AvatarService _sut;

    private readonly IStorageService _storageService = Substitute.For<IStorageService>();

    private readonly UserManager<User> _userManager =
        Substitute.For<UserManager<User>>(Substitute.For<IUserStore<User>>(), null, null, null, null, null, null, null,
            null);

    private readonly ISession _session = Substitute.For<ISession>();
    private readonly ILogger<AvatarService> _logger = Substitute.For<ILogger<AvatarService>>();

    private readonly Fixture _fixture = new();

    public AvatarServiceTests()
    {
        _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _sut = new AvatarService(_storageService, _userManager, _session, _logger);
    }

    [Theory]
    [InlineData("1.png")]
    [InlineData("2.jpg")]
    [InlineData("3.jpeg")]
    public async Task UploadAvatarImageAsync_ShouldUploadImage(string fileName)
    {
        // Arrange
        var fileStream = new MemoryStream();
        var user = _fixture.Create<User>();
        var storedFileName = _fixture.Create<string>();

        var expectedUser = user.DeepClone();
        expectedUser.Avatar = storedFileName;
        var expectedUserToUpdate = expectedUser.ToExpectedObject();

        _session.UserId.Returns(user.Id);
        _userManager.FindByIdAsync(user.Id.ToString()).Returns(user);
        _storageService.StoreUserAvatarAsync(fileStream, fileName).Returns(storedFileName);

        // Act
        await _sut.UploadAvatarImageAsync(fileStream, fileName);

        // Assert
        await _userManager.Received(1).UpdateAsync(Arg.Is<User>(u => expectedUserToUpdate.Equals(u)));
    }

    [Fact]
    public async Task UploadAvatarImageAsync_ShouldFail_WhenUserDoesNotExist()
    {
        // Arrange
        var fileStream = new MemoryStream();
        var fileName = _fixture.Create<string>();
        var nonexistentUserId = _fixture.Create<Guid>();

        _session.UserId.Returns(nonexistentUserId);
        _userManager.FindByIdAsync(nonexistentUserId.ToString()).ReturnsNull();

        // Act
        Func<Task> result = async () => await _sut.UploadAvatarImageAsync(fileStream, fileName);

        // Assert
        await result.Should().ThrowExactlyAsync<AccessDeniedException>();
        await _storageService.DidNotReceive()
            .StoreUserAvatarAsync(Arg.Any<Stream>(), Arg.Any<string>());
    }

    [Fact]
    public async Task UploadAvatarImageAsync_ShouldFail_WhenImageExtensionIsNotSupported()
    {
        // Arrange
        var fileStream = new MemoryStream();
        var fileName = _fixture.Create<string>();
        var user = _fixture.Create<User>();

        _session.UserId.Returns(user.Id);
        _userManager.FindByIdAsync(user.Id.ToString()).Returns(user);
        _storageService.StoreUserAvatarAsync(fileStream, fileName).ThrowsAsync<GameStoreException>();

        // Act
        Func<Task> result = async () => await _sut.UploadAvatarImageAsync(fileStream, fileName);

        // Assert
        await result.Should().ThrowExactlyAsync<GameStoreException>();
        await _storageService.Received(1).StoreUserAvatarAsync(fileStream, fileName);
    }

    [Fact]
    public async Task GetAvatarImageAsync_ShouldReturnAvatarImage()
    {
        // Arrange
        var user = _fixture.Create<User>();
        var fileStream = new MemoryStream();

        _session.UserId.Returns(user.Id);
        _userManager.FindByIdAsync(user.Id.ToString()).Returns(user);
        _storageService.GetUserAvatar(user.Avatar!).Returns(fileStream);

        // Act
        var result = await _sut.GetAvatarImageAsync();

        // Assert
        result.FileName.Should().Be(user.Avatar);
        result.FileStream.Should().BeSameAs(fileStream);

        _storageService.Received(1).GetUserAvatar(user.Avatar!);
    }

    [Fact]
    public async Task GetAvatarImageAsync_ShouldFail_WhenUserDoesNotExist()
    {
        // Arrange
        var nonexistentUserId = _fixture.Create<Guid>();

        _session.UserId.Returns(nonexistentUserId);
        _userManager.FindByIdAsync(nonexistentUserId.ToString()).ReturnsNull();

        // Act
        Func<Task> result = async () => await _sut.GetAvatarImageAsync();

        // Assert
        await result.Should().ThrowExactlyAsync<AccessDeniedException>();
    }

    [Fact]
    public async Task GetAvatarImageAsync_ShouldFail_WhenUserHasNotAvatar()
    {
        // Arrange
        var user = _fixture.Build<User>()
            .Without(u => u.Avatar)
            .Create();
        const string expectedExceptionMessage = "User has not avatar.";

        _session.UserId.Returns(user.Id);
        _userManager.FindByIdAsync(user.Id.ToString()).Returns(user);

        // Act
        Func<Task> result = async () => await _sut.GetAvatarImageAsync();

        // Assert
        await result.Should().ThrowExactlyAsync<NotFoundException>()
            .WithMessage(expectedExceptionMessage);
    }

    [Fact]
    public async Task GetAvatarImage_ShouldFail_UserAvatarImageNotFound()
    {
        // Arrange
        var user = _fixture.Create<User>();
        const string expectedExceptionMessage = "User's avatar not found.";

        _session.UserId.Returns(user.Id);
        _userManager.FindByIdAsync(user.Id.ToString()).Returns(user);
        _storageService.GetUserAvatar(user.Avatar!).Throws<NotFoundException>();

        // Act
        Func<Task> result = async () => await _sut.GetAvatarImageAsync();

        // Assert
        await result.Should().ThrowExactlyAsync<NotFoundException>()
            .WithMessage(expectedExceptionMessage);
    }
}