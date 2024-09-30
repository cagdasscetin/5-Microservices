using Micro.Application.Interfaces.Repositories;
using Micro.Domain.Entities;
using Micro.Persistence.Context;

namespace Micro.Persistence.Repositories;

public class OrderRepository : BaseRepository<Order, int>, IOrderRepository
{
    public OrderRepository(ApplicationDbContext context) : base(context)
    {
    }
}