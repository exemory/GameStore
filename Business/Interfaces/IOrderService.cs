using Business.DataTransferObjects;
using Business.Exceptions;

namespace Business.Interfaces;

/// <summary>
/// Service for orders
/// </summary>
public interface IOrderService
{
    /// <summary>
    /// Create an order
    /// </summary>
    /// <param name="orderCreationDto">Order creation data</param>
    /// <exception cref="NotFoundException">
    /// Thrown when some games do not exist
    /// </exception>
    public Task CreateAsync(OrderCreationDto orderCreationDto);
}