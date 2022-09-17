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
        var newGame = _mapper.Map<Game>(gameCreationDto);
        
        _unitOfWork.GameRepository.Add(newGame);
        await _unitOfWork.SaveAsync();
        
        return _mapper.Map<GameDto>(newGame);
    }

    public async Task UpdateAsync(Guid id, GameUpdateDto gameUpdateDto)
    {
        var game = await _unitOfWork.GameRepository.GetByIdAsync(id);

        if (game == null)
        {
            throw new NotFoundException($"Game with id '{id}' not found.");
        }

        _mapper.Map(gameUpdateDto, game);
        
        _unitOfWork.GameRepository.Update(game);
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
}