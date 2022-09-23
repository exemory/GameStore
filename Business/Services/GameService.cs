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

    public async Task<GameDto> CreateAsync(GameCreationDto gameCreationDto)
    {
        await CheckIfGameWithKeyAlreadyExists(gameCreationDto.Key);

        var newGame = _mapper.Map<Game>(gameCreationDto);

        _unitOfWork.GameRepository.Add(newGame);
        await _unitOfWork.SaveAsync();

        return _mapper.Map<GameDto>(newGame);
    }

    public async Task UpdateAsync(Guid gameId, GameUpdateDto gameUpdateDto)
    {
        await CheckIfGameWithKeyAlreadyExists(gameUpdateDto.Key, gameId);

        var gameToUpdate = await GetGameById(gameId);

        _mapper.Map(gameUpdateDto, gameToUpdate);

        _unitOfWork.GameRepository.Update(gameToUpdate);
        await _unitOfWork.SaveAsync();
    }

    public async Task<GameWithDetailsDto> GetByKeyWithDetailsAsync(string gameKey)
    {
        var game = await GetGameByKeyWithDetails(gameKey);
        return _mapper.Map<GameWithDetailsDto>(game);
    }

    public async Task<IEnumerable<GameDto>> GetAllAsync()
    {
        var games = await _unitOfWork.GameRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<GameDto>>(games);
    }

    public async Task DeleteAsync(Guid gameId)
    {
        var gameToDelete = await GetGameById(gameId);

        _unitOfWork.GameRepository.Delete(gameToDelete);
        await _unitOfWork.SaveAsync();
    }

    public async Task<IEnumerable<GameDto>> GetAllByGenreAsync(string genre)
    {
        var games = await _unitOfWork.GameRepository.GetAllByGenreAsync(genre);
        return _mapper.Map<IEnumerable<GameDto>>(games);
    }

    public async Task<IEnumerable<GameDto>> GetAllByPlatformTypesAsync(IEnumerable<string> platformTypes)
    {
        var games = await _unitOfWork.GameRepository.GetAllByPlatformTypesAsync(platformTypes);
        return _mapper.Map<IEnumerable<GameDto>>(games);
    }

    public async Task<Stream> DownloadAsync(string gameKey)
    {
        await CheckIfGameExists(gameKey);

        return new MemoryStream(1024 * 128); // 128kb file stub
    }

    private async Task CheckIfGameWithKeyAlreadyExists(string gameKey, Guid? exceptGameId = null)
    {
        var game = await _unitOfWork.GameRepository.GetByKeyAsync(gameKey);

        if (game != null && (exceptGameId == null || exceptGameId != game.Id))
        {
            throw new GameStoreException($"Game with key '{gameKey}' already exists.");
        }
    }

    private async Task CheckIfGameExists(string gameKey)
    {
        var game = await _unitOfWork.GameRepository.GetByKeyAsync(gameKey);

        if (game == null)
        {
            ThrowGameNotFound(gameKey);
        }
    }

    private async Task<Game> GetGameById(Guid gameId)
    {
        var game = await _unitOfWork.GameRepository.GetByIdAsync(gameId);

        if (game == null)
        {
            throw new NotFoundException($"Game with id '{gameId}' not found.");
        }

        return game;
    }

    private async Task<Game> GetGameByKeyWithDetails(string gameKey)
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
}