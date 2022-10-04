using AutoMapper;
using Business.DataTransferObjects;
using Business.Exceptions;
using Business.Interfaces;
using Data.Entities;
using Data.Interfaces;

namespace Business.Services;

/// <inheritdoc />
public class GameService : IGameService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    /// <summary>
    /// Constructor for initializing a <see cref="GameService"/> class instance
    /// </summary>
    /// <param name="unitOfWork">Unit of work</param>
    /// <param name="mapper">Mapper</param>
    public GameService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<GameWithGenresDto> CreateAsync(GameCreationDto gameCreationDto)
    {
        await CheckIfGameWithKeyAlreadyExistsAsync(gameCreationDto.Key);

        var newGame = _mapper.Map<Game>(gameCreationDto);
        newGame.Genres = await GetGenresByIdsAsync(gameCreationDto.GenreIds);
        newGame.PlatformTypes = await GetPlatformTypesByIdsAsync(gameCreationDto.PlatformTypeIds);

        _unitOfWork.GameRepository.Add(newGame);
        await _unitOfWork.SaveAsync();

        return _mapper.Map<GameWithGenresDto>(newGame);
    }

    public async Task UpdateAsync(Guid gameId, GameUpdateDto gameUpdateDto)
    {
        var gameToUpdate = await GetGameByIdWithDetailsAsync(gameId);

        if (gameUpdateDto.Key != gameToUpdate.Key)
        {
            await CheckIfGameWithKeyAlreadyExistsAsync(gameUpdateDto.Key);
        }

        _mapper.Map(gameUpdateDto, gameToUpdate);
        gameToUpdate.Genres = await GetGenresByIdsAsync(gameUpdateDto.GenreIds);
        gameToUpdate.PlatformTypes = await GetPlatformTypesByIdsAsync(gameUpdateDto.PlatformTypeIds);

        _unitOfWork.GameRepository.Update(gameToUpdate);
        await _unitOfWork.SaveAsync();
    }

    public async Task<GameWithDetailsDto> GetByKeyWithDetailsAsync(string gameKey)
    {
        var game = await GetGameByKeyWithDetailsAsync(gameKey);
        return _mapper.Map<GameWithDetailsDto>(game);
    }

    public async Task<IEnumerable<GameWithGenresDto>> GetAllAsync()
    {
        var games = await _unitOfWork.GameRepository.GetAllWithGenresAsync();
        return _mapper.Map<IEnumerable<GameWithGenresDto>>(games);
    }

    public async Task DeleteAsync(Guid gameId)
    {
        var gameToDelete = await GetGameByIdAsync(gameId);

        _unitOfWork.GameRepository.Delete(gameToDelete);
        await _unitOfWork.SaveAsync();
    }

    public async Task<IEnumerable<GameWithGenresDto>> GetAllByGenreAsync(string genre)
    {
        var games = await _unitOfWork.GameRepository.GetAllByGenreWithGenresAsync(genre);
        return _mapper.Map<IEnumerable<GameWithGenresDto>>(games);
    }

    public async Task<IEnumerable<GameWithGenresDto>> GetAllByPlatformTypesAsync(IEnumerable<string> platformTypes)
    {
        var games = await _unitOfWork.GameRepository.GetAllByPlatformTypesWithGenresAsync(platformTypes);
        return _mapper.Map<IEnumerable<GameWithGenresDto>>(games);
    }

    public async Task<Stream> DownloadAsync(string gameKey)
    {
        await CheckIfGameExistsAsync(gameKey);

        return new MemoryStream(1024 * 128); // 128kb file stub
    }

    private async Task CheckIfGameWithKeyAlreadyExistsAsync(string gameKey)
    {
        var game = await _unitOfWork.GameRepository.GetByKeyAsync(gameKey);

        if (game != null)
        {
            throw new GameStoreException($"Game with key '{gameKey}' already exists.");
        }
    }

    private async Task CheckIfGameExistsAsync(string gameKey)
    {
        var game = await _unitOfWork.GameRepository.GetByKeyAsync(gameKey);

        if (game == null)
        {
            ThrowGameNotFound(gameKey);
        }
    }

    private async Task<Game> GetGameByIdAsync(Guid gameId)
    {
        var game = await _unitOfWork.GameRepository.GetByIdAsync(gameId);

        if (game == null)
        {
            ThrowGameNotFound(gameId);
        }

        return game!;
    }

    private async Task<Game> GetGameByIdWithDetailsAsync(Guid gameId)
    {
        var game = await _unitOfWork.GameRepository.GetByIdWithDetailsAsync(gameId);

        if (game == null)
        {
            ThrowGameNotFound(gameId);
        }

        return game!;
    }

    private async Task<Game> GetGameByKeyWithDetailsAsync(string gameKey)
    {
        var game = await _unitOfWork.GameRepository.GetByKeyWithDetailsAsync(gameKey);

        if (game == null)
        {
            ThrowGameNotFound(gameKey);
        }

        return game!;
    }

    private static void ThrowGameNotFound(string gameKey)
    {
        throw new NotFoundException($"Game with key '{gameKey}' not found.");
    }

    private static void ThrowGameNotFound(Guid gameId)
    {
        throw new NotFoundException($"Game with id '{gameId}' not found.");
    }

    private async Task<ICollection<Genre>> GetGenresByIdsAsync(ICollection<Guid>? genreIds)
    {
        if (genreIds == null || !genreIds.Any())
        {
            return new List<Genre>();
        }

        genreIds = genreIds.Distinct().ToList();

        var existedGenres = await _unitOfWork.GenreRepository.GetByIdsAsync(genreIds);

        if (existedGenres.Count == genreIds.Count)
        {
            return existedGenres;
        }

        var nonexistentGenreIds = genreIds.Except(existedGenres.Select(g => g.Id));

        throw new NotFoundException($"Genres with ids {string.Join(", ", nonexistentGenreIds)} not found.");
    }

    private async Task<ICollection<PlatformType>> GetPlatformTypesByIdsAsync(ICollection<Guid>? platformTypeIds)
    {
        if (platformTypeIds == null || !platformTypeIds.Any())
        {
            return new List<PlatformType>();
        }

        platformTypeIds = platformTypeIds.Distinct().ToList();

        var existedPlatformTypes = await _unitOfWork.PlatformTypeRepository.GetByIdsAsync(platformTypeIds);

        if (existedPlatformTypes.Count == platformTypeIds.Count)
        {
            return existedPlatformTypes;
        }

        var nonexistentPlatformTypeIds = platformTypeIds.Except(existedPlatformTypes.Select(g => g.Id));

        throw new NotFoundException(
            $"Platform types with ids {string.Join(", ", nonexistentPlatformTypeIds)} not found.");
    }
}