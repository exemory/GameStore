using Data.Entities;
using Data.Interfaces;

namespace Data.Repositories;

/// <inheritdoc cref="IOrderRepository" />
public class OrderRepository : Repository<Order>, IOrderRepository
{
    /// <summary>
    /// Constructor for initializing a <see cref="OrderRepository"/> class instance
    /// </summary>
    /// <param name="context">Context of the database</param>
    public OrderRepository(GameStoreContext context) : base(context)
    {
    }
}