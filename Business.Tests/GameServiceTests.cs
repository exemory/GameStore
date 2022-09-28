using AutoFixture;
using AutoMapper;
using Business.DataTransferObjects;
using Business.Exceptions;
using Business.Services;
using Data.Entities;
using Data.Interfaces;
using ExpectedObjects;
using FluentAssertions;
using Moq;
using Xunit;

namespace Business.Tests;

public class GameServiceTests
{
    private readonly GameService _sut;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

    private readonly Fixture _fixture = new();
    private readonly IMapper _mapper = UnitTestHelper.CreateMapper();

    public GameServiceTests()
    {
        _sut = new GameService(_unitOfWorkMock.Object, _mapper);
        _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateGame()
    {
        // Arrange
        var gameCreationDto = _fixture.Create<GameCreationDto>();
        var mappedGame = _mapper.Map<Game>(gameCreationDto);
        var expectedToCreate = mappedGame.ToExpectedObject();
        var expected = _mapper.Map<GameWithGenresDto>(mappedGame);

        _unitOfWorkMock.Setup(u => u.GameRepository.GetByKeyAsync(gameCreationDto.Key))
            .ReturnsAsync((Game?) null);

        // Act
        var result = await _sut.CreateAsync(gameCreationDto);

        // Assert
        result.Should().BeEquivalentTo(expected);

        _unitOfWorkMock.Verify(u => u.GameRepository.Add(It.Is<Game>(g => expectedToCreate.Equals(g))), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ShouldFail_WhenGameWithKeyAlreadyExists()
    {
        // Arrange
        var gameCreationDto = _fixture.Build<GameCreationDto>().Create();

        _unitOfWorkMock.Setup(u => u.GameRepository.GetByKeyAsync(gameCreationDto.Key))
            .ReturnsAsync(_fixture.Create<Game>());

        // Act
        Func<Task> result = async () => await _sut.CreateAsync(gameCreationDto);

        // Assert
        await result.Should().ThrowAsync<GameStoreException>();

        _unitOfWorkMock.Verify(u => u.GameRepository.Add(It.IsAny<Game>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.SaveAsync(), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateGame()
    {
        // Arrange
        var game = _fixture.Build<Game>()
            .Without(g => g.Comments)
            .Without(g => g.Genres)
            .Without(g => g.PlatformTypes)
            .Create();
        var gameUpdateDto = _fixture.Create<GameUpdateDto>();
        var expectedGameToUpdate = _mapper.Map<Game>(gameUpdateDto, o =>
                o.AfterMap((d, g) => g.Id = game.Id))
            .ToExpectedObject();

        _unitOfWorkMock.Setup(u => u.GameRepository.GetByKeyAsync(gameUpdateDto.Key))
            .ReturnsAsync((Game?) null);
        _unitOfWorkMock.Setup(u => u.GameRepository.GetByIdAsync(game.Id))
            .ReturnsAsync(game);

        // Act
        await _sut.UpdateAsync(game.Id, gameUpdateDto);

        // Assert
        _unitOfWorkMock.Verify(u =>
            u.GameRepository.Update(It.Is<Game>(g => expectedGameToUpdate.Equals(g))), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateGame_WhenKeysAreEqual()
    {
        // Arrange
        var game = _fixture.Build<Game>()
            .Without(g => g.Comments)
            .Without(g => g.Genres)
            .Without(g => g.PlatformTypes)
            .Create();
        var gameUpdateDto = _fixture.Build<GameUpdateDto>()
            .With(g => g.Key, game.Key)
            .Create();
        var expectedGameToUpdate = _mapper.Map<Game>(gameUpdateDto, o =>
                o.AfterMap((d, g) => g.Id = game.Id))
            .ToExpectedObject();

        _unitOfWorkMock.Setup(u => u.GameRepository.GetByKeyAsync(gameUpdateDto.Key))
            .ReturnsAsync(game);
        _unitOfWorkMock.Setup(u => u.GameRepository.GetByIdAsync(game.Id))
            .ReturnsAsync(game);

        // Act
        await _sut.UpdateAsync(game.Id, gameUpdateDto);

        // Assert
        _unitOfWorkMock.Verify(u =>
            u.GameRepository.Update(It.Is<Game>(g => expectedGameToUpdate.Equals(g))), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldFail_WhenGameWithNewKeyAlreadyExists()
    {
        // Arrange
        var gameUpdateDto = _fixture.Create<GameUpdateDto>();
        var gameIdToUpdate = Guid.NewGuid();

        _unitOfWorkMock.Setup(u => u.GameRepository.GetByKeyAsync(gameUpdateDto.Key))
            .ReturnsAsync(_fixture.Create<Game>());

        // Act
        Func<Task> result = async () => await _sut.UpdateAsync(gameIdToUpdate, gameUpdateDto);

        // Assert
        await result.Should().ThrowAsync<GameStoreException>();

        _unitOfWorkMock.Verify(u => u.GameRepository.Update(It.IsAny<Game>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.SaveAsync(), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_ShouldFail_WhenGameDoesNotExist()
    {
        // Arrange
        var nonexistentGameId = Guid.NewGuid();
        var gameUpdateDto = _fixture.Create<GameUpdateDto>();

        _unitOfWorkMock.Setup(u => u.GameRepository.GetByKeyAsync(gameUpdateDto.Key))
            .ReturnsAsync((Game?) null);
        _unitOfWorkMock.Setup(u => u.GameRepository.GetByIdAsync(nonexistentGameId))
            .ReturnsAsync((Game?) null);

        // Act
        Func<Task> result = async () => await _sut.UpdateAsync(nonexistentGameId, gameUpdateDto);

        // Assert
        await result.Should().ThrowAsync<NotFoundException>();

        _unitOfWorkMock.Verify(u => u.GameRepository.Update(It.IsAny<Game>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.SaveAsync(), Times.Never);
    }

    [Fact]
    public async Task GetByKeyWithDetailsAsync_ShouldReturnGame()
    {
        // Arrange
        var game = _fixture.Create<Game>();
        var expected = _mapper.Map<GameWithDetailsDto>(game);

        _unitOfWorkMock.Setup(u => u.GameRepository.GetByKeyWithDetailsAsync(game.Key))
            .ReturnsAsync(game);

        // Act
        var result = await _sut.GetByKeyWithDetailsAsync(game.Key);

        // Assert
        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetByKeyWithDetailsAsync_ShouldFail_WhenGameDoesNotExist()
    {
        // Arrange
        var nonexistentGameKey = _fixture.Create<string>();

        _unitOfWorkMock.Setup(u => u.GameRepository.GetByKeyWithDetailsAsync(nonexistentGameKey))
            .ReturnsAsync((Game?) null);

        // Act
        Func<Task> result = async () => await _sut.GetByKeyWithDetailsAsync(nonexistentGameKey);

        // Assert
        await result.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllGames()
    {
        // Arrange
        var games = _fixture.Build<Game>()
            .Without(g => g.Comments)
            .Without(g => g.PlatformTypes)
            .CreateMany();
        var expected = _mapper.Map<IEnumerable<GameWithGenresDto>>(games);

        _unitOfWorkMock.Setup(u => u.GameRepository.GetAllWithGenresAsync())
            .ReturnsAsync(games);

        // Act
        var result = await _sut.GetAllAsync();

        // Assert
        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetAllByGenreAsync_ShouldReturnGenreGames()
    {
        // Arrange
        var genre = _fixture.Create<string>();
        var games = _fixture.Build<Game>()
            .Without(g => g.Comments)
            .Without(g => g.PlatformTypes)
            .CreateMany();
        var expected = _mapper.Map<IEnumerable<GameWithGenresDto>>(games);

        _unitOfWorkMock.Setup(u => u.GameRepository.GetAllByGenreWithGenresAsync(genre))
            .ReturnsAsync(games);

        // Act
        var result = await _sut.GetAllByGenreAsync(genre);

        // Assert
        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetAllByGenreAsync_GetAllByPlatformTypesAsync()
    {
        // Arrange
        var platformTypes = _fixture.CreateMany<string>();
        var expectedPlatformTypes = platformTypes.ToExpectedObject();
        var games = _fixture.Build<Game>()
            .Without(g => g.Comments)
            .Without(g => g.PlatformTypes)
            .CreateMany();
        var expected = _mapper.Map<IEnumerable<GameWithGenresDto>>(games);

        _unitOfWorkMock.Setup(u => u.GameRepository.GetAllByPlatformTypesWithGenresAsync(
                It.Is<IEnumerable<string>>(pts => expectedPlatformTypes.Equals(platformTypes))))
            .ReturnsAsync(games);

        // Act
        var result = await _sut.GetAllByPlatformTypesAsync(platformTypes);

        // Assert
        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeleteGame()
    {
        // Arrange
        var game = _fixture.Create<Game>();
        var expectedGameToDelete = game.ToExpectedObject();

        _unitOfWorkMock.Setup(u => u.GameRepository.GetByIdAsync(game.Id))
            .ReturnsAsync(game);

        // Act
        await _sut.DeleteAsync(game.Id);

        // Assert
        _unitOfWorkMock.Verify(u =>
            u.GameRepository.Delete(It.Is<Game>(g => expectedGameToDelete.Equals(g))), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldFail_WhenGameDoesNotExist()
    {
        // Arrange
        var nonexistentGameId = Guid.NewGuid();

        _unitOfWorkMock.Setup(u => u.GameRepository.GetByIdAsync(nonexistentGameId))
            .ReturnsAsync((Game?) null);

        // Act
        Func<Task> result = async () => await _sut.DeleteAsync(nonexistentGameId);

        // Assert
        await result.Should().ThrowAsync<NotFoundException>();

        _unitOfWorkMock.Verify(u => u.GameRepository.Delete(It.IsAny<Game>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.SaveAsync(), Times.Never);
    }

    [Fact]
    public async Task DownloadAsync_ShouldReturnFileStream()
    {
        // Arrange
        var game = _fixture.Create<Game>();

        _unitOfWorkMock.Setup(u => u.GameRepository.GetByKeyAsync(game.Key))
            .ReturnsAsync(game);

        // Act
        Func<Task> result = async () => await _sut.DownloadAsync(game.Key);

        // Assert
        await result.Should().NotThrowAsync();
    }

    [Fact]
    public async Task DownloadAsync_ShouldFail_WhenGameDoesNotExist()
    {
        // Arrange
        var nonexistentGameKey = _fixture.Create<string>();

        _unitOfWorkMock.Setup(u => u.GameRepository.GetByKeyAsync(nonexistentGameKey))
            .ReturnsAsync((Game?) null);

        // Act
        Func<Task> result = async () => await _sut.DownloadAsync(nonexistentGameKey);

        // Assert
        await result.Should().ThrowAsync<NotFoundException>();
    }
}