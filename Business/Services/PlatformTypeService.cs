using AutoMapper;
using Business.DataTransferObjects;
using Business.Interfaces;
using Data.Interfaces;

namespace Business.Services;

/// <inheritdoc />
public class PlatformTypeService : IPlatformTypeService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    /// <summary>
    /// Constructor for initializing a <see cref="PlatformTypeService"/> class instance
    /// </summary>
    /// <param name="unitOfWork">Unit of work</param>
    /// <param name="mapper">Mapper</param>
    public PlatformTypeService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<PlatformTypeDto>> GetAllAsync()
    {
        var platformTypes = await _unitOfWork.PlatformTypeRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<PlatformTypeDto>>(platformTypes);
    }
}