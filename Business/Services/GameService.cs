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
        if (gameCreationDto.Key != null)
        {
            var existedGame = await _unitOfWork.GameRepository.GetByKeyAsync(gameCreationDto.Key);

            if (existedGame != null)
            {
                throw new GameStoreException($"Game with key '{gameCreationDto.Key}' already exists.");
            }
        }

        var newGame = _mapper.Map<Game>(gameCreationDto);

        _unitOfWork.GameRepository.Add(newGame);
        await _unitOfWork.SaveAsync();

        return _mapper.Map<GameDto>(newGame);
    }

    public async Task UpdateAsync(Guid id, GameUpdateDto gameUpdateDto)
    {
        if (gameUpdateDto.Key != null)
        {
            var existedGame = await _unitOfWork.GameRepository.GetByKeyAsync(gameUpdateDto.Key);

            if (existedGame != null && existedGame.Id != id)
            {
                throw new GameStoreException($"Game with key '{gameUpdateDto.Key}' already exists.");
            }
        }

        var gameToUpdate = await _unitOfWork.GameRepository.GetByIdAsync(id);

        if (gameToUpdate == null)
        {
            throw new NotFoundException($"Game with id '{id}' not found.");
        }

        _mapper.Map(gameUpdateDto, gameToUpdate);

        _unitOfWork.GameRepository.Update(gameToUpdate);
        await _unitOfWork.SaveAsync();
    }

    public async Task<GameWithDetailsDto> GetByKeyWithDetailsAsync(string key)
    {
        var game = await _unitOfWork.GameRepository.GetByKeyWithDetailsAsync(key);

        if (game == null)
        {
            throw new NotFoundException($"Game with key '{key}' not found.");
        }

        return _mapper.Map<GameWithDetailsDto>(game);
    }

    public async Task<IEnumerable<GameDto>> GetAllAsync()
    {
        var games = await _unitOfWork.GameRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<GameDto>>(games);
    }

    public async Task DeleteAsync(Guid id)
    {
        var game = await _unitOfWork.GameRepository.GetByIdAsync(id);

        if (game == null)
        {
            throw new NotFoundException($"Game with id '{id}' not found.");
        }

        _unitOfWork.GameRepository.Delete(game);
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

    public async Task<Stream> DownloadAsync(string key)
    {
        var game = await _unitOfWork.GameRepository.GetByKeyWithDetailsAsync(key);

        if (game == null)
        {
            throw new NotFoundException($"Game with key '{key}' not found.");
        }

        return new MemoryStream(1024 * 128); // 128kb file stub
    }
}