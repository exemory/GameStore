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
        var gameCreationDto = _fixture.Create<GameCreationDto>();
        var mappedGame = _mapper.Map<Game>(gameCreationDto);
        var expectedToCreate = mappedGame.ToExpectedObject();
        var expected = _mapper.Map<GameDto>(mappedGame);

        _unitOfWorkMock.Setup(u => u.GameRepository.GetByKeyAsync(gameCreationDto.Key))
            .ReturnsAsync((Game?) null);

        var result = await _sut.CreateAsync(gameCreationDto);

        result.Should().BeEquivalentTo(expected);

        _unitOfWorkMock.Verify(u => u.GameRepository.Add(It.Is<Game>(g => expectedToCreate.Equals(g))), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ShouldFail_WhenGameWithKeyAlreadyExists()
    {
        var gameCreationDto = _fixture.Build<GameCreationDto>().Create();

        _unitOfWorkMock.Setup(u => u.GameRepository.GetByKeyAsync(gameCreationDto.Key))
            .ReturnsAsync(_fixture.Create<Game>());

        Func<Task> result = async () => await _sut.CreateAsync(gameCreationDto);

        await result.Should().ThrowAsync<GameStoreException>();

        _unitOfWorkMock.Verify(u => u.GameRepository.Add(It.IsAny<Game>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.SaveAsync(), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateGame()
    {
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

        await _sut.UpdateAsync(game.Id, gameUpdateDto);

        _unitOfWorkMock.Verify(u =>
            u.GameRepository.Update(It.Is<Game>(g => expectedGameToUpdate.Equals(g))), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateGame_WhenKeysAreEqual()
    {
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

        await _sut.UpdateAsync(game.Id, gameUpdateDto);

        _unitOfWorkMock.Verify(u =>
            u.GameRepository.Update(It.Is<Game>(g => expectedGameToUpdate.Equals(g))), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldFail_WhenGameWithNewKeyAlreadyExists()
    {
        var gameUpdateDto = _fixture.Create<GameUpdateDto>();
        var gameIdToUpdate = Guid.NewGuid();

        _unitOfWorkMock.Setup(u => u.GameRepository.GetByKeyAsync(gameUpdateDto.Key))
            .ReturnsAsync(_fixture.Create<Game>());

        Func<Task> result = async () => await _sut.UpdateAsync(gameIdToUpdate, gameUpdateDto);

        await result.Should().ThrowAsync<GameStoreException>();

        _unitOfWorkMock.Verify(u => u.GameRepository.Update(It.IsAny<Game>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.SaveAsync(), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_ShouldFail_WhenGameDoesNotExist()
    {
        var nonexistentGameId = Guid.NewGuid();
        var gameUpdateDto = _fixture.Create<GameUpdateDto>();

        _unitOfWorkMock.Setup(u => u.GameRepository.GetByKeyAsync(gameUpdateDto.Key))
            .ReturnsAsync((Game?) null);
        _unitOfWorkMock.Setup(u => u.GameRepository.GetByIdAsync(nonexistentGameId))
            .ReturnsAsync((Game?) null);

        Func<Task> result = async () => await _sut.UpdateAsync(nonexistentGameId, gameUpdateDto);

        await result.Should().ThrowAsync<NotFoundException>();

        _unitOfWorkMock.Verify(u => u.GameRepository.Update(It.IsAny<Game>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.SaveAsync(), Times.Never);
    }

    [Fact]
    public async Task GetByKeyWithDetailsAsync_ShouldReturnGame()
    {
        var game = _fixture.Create<Game>();
        var expected = _mapper.Map<GameWithDetailsDto>(game);

        _unitOfWorkMock.Setup(u => u.GameRepository.GetByKeyWithDetailsAsync(game.Key))
            .ReturnsAsync(game);

        var result = await _sut.GetByKeyWithDetailsAsync(game.Key);

        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetByKeyWithDetailsAsync_ShouldFail_WhenGameDoesNotExist()
    {
        var nonexistentGameKey = _fixture.Create<string>();

        _unitOfWorkMock.Setup(u => u.GameRepository.GetByKeyWithDetailsAsync(nonexistentGameKey))
            .ReturnsAsync((Game?) null);

        Func<Task> result = async () => await _sut.GetByKeyWithDetailsAsync(nonexistentGameKey);

        await result.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllGames()
    {
        var games = _fixture.Build<Game>()
            .Without(g => g.Comments)
            .Without(g => g.Genres)
            .Without(g => g.PlatformTypes)
            .CreateMany();
        var expected = _mapper.Map<IEnumerable<GameDto>>(games);

        _unitOfWorkMock.Setup(u => u.GameRepository.GetAllAsync())
            .ReturnsAsync(games);

        var result = await _sut.GetAllAsync();

        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetAllByGenreAsync_ShouldReturnGenreGames()
    {
        var genre = _fixture.Create<string>();
        var games = _fixture.Build<Game>()
            .Without(g => g.Comments)
            .Without(g => g.Genres)
            .Without(g => g.PlatformTypes)
            .CreateMany();
        var expected = _mapper.Map<IEnumerable<GameDto>>(games);

        _unitOfWorkMock.Setup(u => u.GameRepository.GetAllByGenreAsync(genre))
            .ReturnsAsync(games);

        var result = await _sut.GetAllByGenreAsync(genre);

        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetAllByGenreAsync_GetAllByPlatformTypesAsync()
    {
        var platformTypes = _fixture.CreateMany<string>();
        var expectedPlatformTypes = platformTypes.ToExpectedObject();
        var games = _fixture.Build<Game>()
            .Without(g => g.Comments)
            .Without(g => g.Genres)
            .Without(g => g.PlatformTypes)
            .CreateMany();
        var expected = _mapper.Map<IEnumerable<GameDto>>(games);

        _unitOfWorkMock.Setup(u => u.GameRepository.GetAllByPlatformTypesAsync(
                It.Is<IEnumerable<string>>(pts => expectedPlatformTypes.Equals(platformTypes))))
            .ReturnsAsync(games);

        var result = await _sut.GetAllByPlatformTypesAsync(platformTypes);

        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeleteGame()
    {
        var game = _fixture.Create<Game>();
        var expectedGameToDelete = game.ToExpectedObject();

        _unitOfWorkMock.Setup(u => u.GameRepository.GetByIdAsync(game.Id))
            .ReturnsAsync(game);

        await _sut.DeleteAsync(game.Id);

        _unitOfWorkMock.Verify(u =>
            u.GameRepository.Delete(It.Is<Game>(g => expectedGameToDelete.Equals(g))), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldFail_WhenGameDoesNotExist()
    {
        var nonexistentGameId = Guid.NewGuid();

        _unitOfWorkMock.Setup(u => u.GameRepository.GetByIdAsync(nonexistentGameId))
            .ReturnsAsync((Game?) null);

        Func<Task> result = async () => await _sut.DeleteAsync(nonexistentGameId);

        await result.Should().ThrowAsync<NotFoundException>();

        _unitOfWorkMock.Verify(u => u.GameRepository.Delete(It.IsAny<Game>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.SaveAsync(), Times.Never);
    }

    [Fact]
    public async Task DownloadAsync_ShouldReturnFileStream()
    {
        var game = _fixture.Create<Game>();

        _unitOfWorkMock.Setup(u => u.GameRepository.GetByKeyAsync(game.Key))
            .ReturnsAsync(game);

        Func<Task> result = async () => await _sut.DownloadAsync(game.Key);

        await result.Should().NotThrowAsync();
    }

    [Fact]
    public async Task DownloadAsync_ShouldFail_WhenGameDoesNotExist()
    {
        var nonexistentGameKey = _fixture.Create<string>();

        _unitOfWorkMock.Setup(u => u.GameRepository.GetByKeyAsync(nonexistentGameKey))
            .ReturnsAsync((Game?) null);

        Func<Task> result = async () => await _sut.DownloadAsync(nonexistentGameKey);

        await result.Should().ThrowAsync<NotFoundException>();
    }
}