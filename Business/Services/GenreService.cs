using AutoMapper;
using Business.DataTransferObjects;
using Business.Interfaces;
using Data.Interfaces;

namespace Business.Services;

/// <inheritdoc />
public class GenreService : IGenreService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    /// <summary>
    /// Constructor for initializing a <see cref="GenreService"/> class instance
    /// </summary>
    /// <param name="unitOfWork">Unit of work</param>
    /// <param name="mapper">Mapper</param>
    public GenreService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<GenreDto>> GetAllAsync()
    {
        var genres = await _unitOfWork.GenreRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<GenreDto>>(genres);
    }
}