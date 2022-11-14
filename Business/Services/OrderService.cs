using AutoMapper;
using Business.DataTransferObjects;
using Business.Exceptions;
using Business.Interfaces;
using Data.Entities;
using Data.Interfaces;

namespace Business.Services;

/// <inheritdoc />
public class OrderService : IOrderService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    /// <summary>
    /// Constructor for initializing a <see cref="OrderService"/> class instance
    /// </summary>
    /// <param name="unitOfWork">Unit of work</param>
    /// <param name="mapper">Mapper</param>
    public OrderService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task CreateAsync(OrderCreationDto orderCreationDto)
    {
        var gameIds = orderCreationDto.Items.Select(i => i.GameId);
        await CheckIfGamesExist(gameIds);

        var order = _mapper.Map<Order>(orderCreationDto);

        _unitOfWork.OrderRepository.Add(order);
        await _unitOfWork.SaveAsync();
    }

    private async Task CheckIfGamesExist(IEnumerable<Guid> gameIds)
    {
        var distinctGameIds = gameIds.Distinct().ToList();

        var games = await _unitOfWork.GameRepository.GetByIds(distinctGameIds);

        if (games.Count != distinctGameIds.Count)
        {
            var existingGameIds = games.Select(g => g.Id);
            var nonexistentGameIds = distinctGameIds.Except(existingGameIds);

            throw new NotFoundException($"Games with ids '{string.Join("', '", nonexistentGameIds)}' not found.");
        }
    }
}