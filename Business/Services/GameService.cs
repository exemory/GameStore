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
        await GetGameByKey(gameCreationDto.Key);

        var newGame = _mapper.Map<Game>(gameCreationDto);

        _unitOfWork.GameRepository.Add(newGame);
        await _unitOfWork.SaveAsync();

        return _mapper.Map<GameDto>(newGame);
    }

    public async Task UpdateAsync(Guid gameId, GameUpdateDto gameUpdateDto)
    {
        await GetGameByKey(gameUpdateDto.Key);

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
        await GetGameByKey(gameKey);

        return new MemoryStream(1024 * 128); // 128kb file stub
    }

    private async Task<Game> GetGameByKey(string gameKey)
    {
        var game = await _unitOfWork.GameRepository.GetByKeyAsync(gameKey);

        if (game == null)
        {
            ThrowGameNotFound(gameKey);
        }

        return game!;
    }

    private async Task<Game> GetGameById(Guid gameId)
    {
        var game = await _unitOfWork.GameRepository.GetByIdAsync(gameId);

        if (game == null)
        {
            ThrowGameNotFound(gameId);
        }

        return game!;
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

    private static void ThrowGameNotFound(Guid gameId)
    {
        throw new NotFoundException($"Game with gameId '{gameId}' not found.");
    }

    private static void ThrowGameNotFound(string gameKey)
    {
        throw new NotFoundException($"Game with gameKey '{gameKey}' not found.");
    }
}