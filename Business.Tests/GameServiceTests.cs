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

    [Theory]
    [MemberData(nameof(CreateAsync_ShouldCreateGame_TestData))]
    public async Task CreateAsync_ShouldCreateGame(GameCreationDto gameCreationDto)
    {
        // Arrange
        var genres = gameCreationDto.GenreIds?
            .Select(i => _fixture.Build<Genre>().With(g => g.Id, i).Create())
            .ToList() ?? new List<Genre>();
        var platformTypes = gameCreationDto.PlatformTypeIds?
            .Select(i => _fixture.Build<PlatformType>().With(p => p.Id, i).Create())
            .ToList() ?? new List<PlatformType>();
        var mappedGame = _mapper.Map<Game>(gameCreationDto);
        mappedGame.Genres = genres;
        mappedGame.PlatformTypes = platformTypes;
        var expectedToCreate = mappedGame.ToExpectedObject();
        var expected = _mapper.Map<GameWithGenresDto>(mappedGame);

        _unitOfWorkMock.Setup(u => u.GameRepository.GetByKeyAsync(gameCreationDto.Key))
            .ReturnsAsync((Game?) null);
        _unitOfWorkMock.Setup(u =>
                u.GenreRepository.GetByIdsAsync(It.IsAny<IEnumerable<Guid>>()))
            .ReturnsAsync((IEnumerable<Guid> ids) => genres.Where(g => ids.Contains(g.Id)).ToList());
        _unitOfWorkMock.Setup(u =>
                u.PlatformTypeRepository.GetByIdsAsync(It.IsAny<IEnumerable<Guid>>()))
            .ReturnsAsync((IEnumerable<Guid> ids) => platformTypes.Where(p => ids.Contains(p.Id)).ToList());

        // Act
        var result = await _sut.CreateAsync(gameCreationDto);

        // Assert
        result.Should().BeEquivalentTo(expected);

        _unitOfWorkMock.Verify(u => u.GameRepository.Add(It.Is<Game>(g => expectedToCreate.Equals(g))), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveAsync(), Times.Once);
    }

    public static IEnumerable<object[]> CreateAsync_ShouldCreateGame_TestData()
    {
        var fixture = new Fixture();

        yield return new object[]
        {
            fixture.Create<GameCreationDto>()
        };
        yield return new object[]
        {
            fixture.Build<GameCreationDto>()
                .Without(g => g.GenreIds)
                .Create()
        };
        yield return new object[]
        {
            fixture.Build<GameCreationDto>()
                .Without(g => g.PlatformTypeIds)
                .Create()
        };
        yield return new object[]
        {
            fixture.Build<GameCreationDto>()
                .With(g => g.GenreIds, new List<Guid>())
                .Create()
        };
        yield return new object[]
        {
            fixture.Build<GameCreationDto>()
                .With(g => g.PlatformTypeIds, new List<Guid>())
                .Create()
        };
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
        await result.Should().ThrowExactlyAsync<GameStoreException>();

        _unitOfWorkMock.Verify(u => u.GameRepository.Add(It.IsAny<Game>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.SaveAsync(), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_ShouldFail_WhenGenresDoNotExist()
    {
        // Arrange
        var gameCreationDto = _fixture.Create<GameCreationDto>();
        var expectedPlatformTypeIds = gameCreationDto.PlatformTypeIds.ToExpectedObject();
        var expectedExceptionMessage = $"Genres with ids {string.Join(", ", gameCreationDto.GenreIds!)} not found.";

        _unitOfWorkMock.Setup(u => u.GameRepository.GetByKeyAsync(gameCreationDto.Key))
            .ReturnsAsync((Game?) null);
        _unitOfWorkMock.Setup(u =>
                u.GenreRepository.GetByIdsAsync(It.IsAny<IEnumerable<Guid>>()))
            .ReturnsAsync(new List<Genre>());
        _unitOfWorkMock.Setup(u =>
                u.PlatformTypeRepository.GetByIdsAsync(It.Is<IEnumerable<Guid>>(ids =>
                    expectedPlatformTypeIds.Equals(ids))))
            .ReturnsAsync(_fixture.CreateMany<PlatformType>(gameCreationDto.PlatformTypeIds!.Count).ToList());

        // Act
        Func<Task> result = async () => await _sut.CreateAsync(gameCreationDto);

        // Assert
        await result.Should().ThrowExactlyAsync<NotFoundException>()
            .WithMessage(expectedExceptionMessage);

        _unitOfWorkMock.Verify(u => u.GameRepository.Add(It.IsAny<Game>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.SaveAsync(), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_ShouldFail_WhenPlatformTypesDoNotExist()
    {
        // Arrange
        var gameCreationDto = _fixture.Create<GameCreationDto>();
        var expectedGenreIds = gameCreationDto.GenreIds.ToExpectedObject();
        var expectedExceptionMessage =
            $"Platform types with ids {string.Join(", ", gameCreationDto.PlatformTypeIds!)} not found.";

        _unitOfWorkMock.Setup(u => u.GameRepository.GetByKeyAsync(gameCreationDto.Key))
            .ReturnsAsync((Game?) null);
        _unitOfWorkMock.Setup(u =>
                u.GenreRepository.GetByIdsAsync(It.Is<IEnumerable<Guid>>(ids => expectedGenreIds.Equals(ids))))
            .ReturnsAsync(_fixture.CreateMany<Genre>(gameCreationDto.GenreIds!.Count).ToList());
        _unitOfWorkMock.Setup(u =>
                u.PlatformTypeRepository.GetByIdsAsync(It.IsAny<IEnumerable<Guid>>()))
            .ReturnsAsync(new List<PlatformType>());

        // Act
        Func<Task> result = async () => await _sut.CreateAsync(gameCreationDto);

        // Assert
        await result.Should().ThrowExactlyAsync<NotFoundException>()
            .WithMessage(expectedExceptionMessage);

        _unitOfWorkMock.Verify(u => u.GameRepository.Add(It.IsAny<Game>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.SaveAsync(), Times.Never);
    }

    [Theory]
    [MemberData(nameof(UpdateAsync_ShouldUpdateGame_TestData))]
    public async Task UpdateAsync_ShouldUpdateGame(Game game, GameUpdateDto gameUpdateDto)
    {
        // Arrange
        var genres = gameUpdateDto.GenreIds?
            .Select(i => _fixture.Build<Genre>().With(g => g.Id, i).Create())
            .ToList() ?? new List<Genre>();
        var platformTypes = gameUpdateDto.PlatformTypeIds?
            .Select(i => _fixture.Build<PlatformType>().With(p => p.Id, i).Create())
            .ToList() ?? new List<PlatformType>();

        var expectedGameToUpdate = _mapper.Map<Game>(gameUpdateDto, o =>
            o.AfterMap((_, g) =>
            {
                g.Id = game.Id;
                g.Genres = genres;
                g.PlatformTypes = platformTypes;
            })).ToExpectedObject();

        _unitOfWorkMock.Setup(u => u.GameRepository.GetByIdWithDetailsAsync(game.Id))
            .ReturnsAsync(game);
        _unitOfWorkMock.Setup(u => u.GameRepository.GetByKeyAsync(game.Key))
            .ReturnsAsync((string key) => game.Key == key ? game : null);
        _unitOfWorkMock.Setup(u =>
                u.GenreRepository.GetByIdsAsync(It.IsAny<IEnumerable<Guid>>()))
            .ReturnsAsync((IEnumerable<Guid> ids) => genres.Where(g => ids.Contains(g.Id)).ToList());
        _unitOfWorkMock.Setup(u =>
                u.PlatformTypeRepository.GetByIdsAsync(It.IsAny<IEnumerable<Guid>>()))
            .ReturnsAsync((IEnumerable<Guid> ids) => platformTypes.Where(p => ids.Contains(p.Id)).ToList());

        // Act
        await _sut.UpdateAsync(game.Id, gameUpdateDto);

        // Assert
        _unitOfWorkMock.Verify(u =>
            u.GameRepository.Update(It.Is<Game>(g => expectedGameToUpdate.Equals(g))), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveAsync(), Times.Once);
    }

    public static IEnumerable<object[]> UpdateAsync_ShouldUpdateGame_TestData()
    {
        var fixture = new Fixture();
        fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
        fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        var gameKey = fixture.Create<string>();

        yield return new object[]
        {
            fixture.Build<Game>()
                .Without(g => g.Comments)
                .Create(),
            fixture.Create<GameUpdateDto>()
        };
        yield return new object[]
        {
            fixture.Build<Game>()
                .With(g => g.Key, gameKey)
                .Without(g => g.Comments)
                .Create(),
            fixture.Build<GameUpdateDto>()
                .With(g => g.Key, gameKey)
                .Create()
        };
        yield return new object[]
        {
            fixture.Build<Game>()
                .Without(g => g.Comments)
                .Create(),
            fixture.Build<GameUpdateDto>()
                .Without(g => g.GenreIds)
                .Create()
        };
        yield return new object[]
        {
            fixture.Build<Game>()
                .Without(g => g.Comments)
                .Create(),
            fixture.Build<GameUpdateDto>()
                .Without(g => g.PlatformTypeIds)
                .Create()
        };
        yield return new object[]
        {
            fixture.Build<Game>()
                .Without(g => g.Comments)
                .Create(),
            fixture.Build<GameUpdateDto>()
                .With(g => g.GenreIds, new List<Guid>())
                .Create()
        };
        yield return new object[]
        {
            fixture.Build<Game>()
                .Without(g => g.Comments)
                .Create(),
            fixture.Build<GameUpdateDto>()
                .With(g => g.PlatformTypeIds, new List<Guid>())
                .Create()
        };
    }

    [Fact]
    public async Task UpdateAsync_ShouldFail_WhenGameDoesNotExist()
    {
        // Arrange
        var nonexistentGameId = Guid.NewGuid();
        var gameUpdateDto = _fixture.Create<GameUpdateDto>();
        var expectedExceptionMessage = $"Game with id '{nonexistentGameId}' not found.";

        _unitOfWorkMock.Setup(u => u.GameRepository.GetByIdWithDetailsAsync(nonexistentGameId))
            .ReturnsAsync((Game?) null);

        // Act
        Func<Task> result = async () => await _sut.UpdateAsync(nonexistentGameId, gameUpdateDto);

        // Assert
        await result.Should().ThrowExactlyAsync<NotFoundException>()
            .WithMessage(expectedExceptionMessage);

        _unitOfWorkMock.Verify(u => u.GameRepository.Update(It.IsAny<Game>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.SaveAsync(), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_ShouldFail_WhenGameWithNewKeyAlreadyExists()
    {
        // Arrange
        var gameUpdateDto = _fixture.Create<GameUpdateDto>();
        var gameIdToUpdate = Guid.NewGuid();

        _unitOfWorkMock.Setup(u => u.GameRepository.GetByIdWithDetailsAsync(gameIdToUpdate))
            .ReturnsAsync(_fixture.Create<Game>());
        _unitOfWorkMock.Setup(u => u.GameRepository.GetByKeyAsync(gameUpdateDto.Key))
            .ReturnsAsync(_fixture.Create<Game>());

        // Act
        Func<Task> result = async () => await _sut.UpdateAsync(gameIdToUpdate, gameUpdateDto);

        // Assert
        await result.Should().ThrowExactlyAsync<GameStoreException>();

        _unitOfWorkMock.Verify(u => u.GameRepository.Update(It.IsAny<Game>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.SaveAsync(), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_ShouldFail_WhenGenresDoNotExist()
    {
        // Arrange
        var gameIdToUpdate = Guid.NewGuid();
        var gameUpdateDto = _fixture.Create<GameUpdateDto>();
        var expectedPlatformTypeIds = gameUpdateDto.PlatformTypeIds.ToExpectedObject();
        var expectedExceptionMessage = $"Genres with ids {string.Join(", ", gameUpdateDto.GenreIds!)} not found.";

        _unitOfWorkMock.Setup(u => u.GameRepository.GetByIdWithDetailsAsync(gameIdToUpdate))
            .ReturnsAsync(_fixture.Create<Game>());
        _unitOfWorkMock.Setup(u => u.GameRepository.GetByKeyAsync(gameUpdateDto.Key))
            .ReturnsAsync((Game?) null);

        _unitOfWorkMock.Setup(u => u.GenreRepository.GetByIdsAsync(It.IsAny<IEnumerable<Guid>>()))
            .ReturnsAsync(new List<Genre>());
        _unitOfWorkMock
            .Setup(u => u.PlatformTypeRepository.GetByIdsAsync(
                It.Is<IEnumerable<Guid>>(ids => expectedPlatformTypeIds.Equals(ids))))
            .ReturnsAsync(_fixture.CreateMany<PlatformType>(gameUpdateDto.PlatformTypeIds!.Count).ToList());

        // Act
        Func<Task> result = async () => await _sut.UpdateAsync(gameIdToUpdate, gameUpdateDto);

        // Assert
        await result.Should().ThrowExactlyAsync<NotFoundException>()
            .WithMessage(expectedExceptionMessage);

        _unitOfWorkMock.Verify(u => u.GameRepository.Add(It.IsAny<Game>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.SaveAsync(), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_ShouldFail_WhenPlatformTypesDoNotExist()
    {
        // Arrange
        var gameIdToUpdate = Guid.NewGuid();
        var gameUpdateDto = _fixture.Create<GameUpdateDto>();
        var expectedGenreIds = gameUpdateDto.GenreIds.ToExpectedObject();
        var expectedExceptionMessage =
            $"Platform types with ids {string.Join(", ", gameUpdateDto.PlatformTypeIds!)} not found.";

        _unitOfWorkMock.Setup(u => u.GameRepository.GetByIdWithDetailsAsync(gameIdToUpdate))
            .ReturnsAsync(_fixture.Create<Game>());
        _unitOfWorkMock.Setup(u => u.GameRepository.GetByKeyAsync(gameUpdateDto.Key))
            .ReturnsAsync((Game?) null);

        _unitOfWorkMock
            .Setup(u => u.GenreRepository.GetByIdsAsync(
                It.Is<IEnumerable<Guid>>(ids => expectedGenreIds.Equals(ids))))
            .ReturnsAsync(_fixture.CreateMany<Genre>(gameUpdateDto.GenreIds!.Count).ToList());
        _unitOfWorkMock.Setup(u => u.PlatformTypeRepository.GetByIdsAsync(It.IsAny<IEnumerable<Guid>>()))
            .ReturnsAsync(new List<PlatformType>());

        // Act
        Func<Task> result = async () => await _sut.UpdateAsync(gameIdToUpdate, gameUpdateDto);

        // Assert
        await result.Should().ThrowExactlyAsync<NotFoundException>()
            .WithMessage(expectedExceptionMessage);

        _unitOfWorkMock.Verify(u => u.GameRepository.Add(It.IsAny<Game>()), Times.Never);
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
        await result.Should().ThrowExactlyAsync<NotFoundException>();
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
        await result.Should().ThrowExactlyAsync<NotFoundException>();

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
        await result.Should().ThrowExactlyAsync<NotFoundException>();
    }
}