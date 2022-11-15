using Data.Entities;

namespace Data.Interfaces;

/// <summary>
/// Repository of <see cref="Order"/> entities
/// </summary>
public interface IOrderRepository : IRepository<Order>
{
}