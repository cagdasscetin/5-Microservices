using Micro.Application.Interfaces.Repositories;
using Micro.Domain.Entities;
using Micro.Persistence.Context;

namespace Micro.Persistence.Repositories;

public class OrderItemRepository : BaseRepository<OrderItem, int>, IOrderItemRepository
{
    public OrderItemRepository(ApplicationDbContext context) : base(context)
    {
    }
}