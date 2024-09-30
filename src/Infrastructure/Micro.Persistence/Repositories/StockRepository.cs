using Micro.Application.Interfaces.Repositories;
using Micro.Domain.Entities;
using Micro.Persistence.Context;

namespace Micro.Persistence.Repositories;

public class StockRepository : BaseRepository<Stock, int>, IStockRepository
{
    public StockRepository(ApplicationDbContext context) : base(context)
    {
    }
}