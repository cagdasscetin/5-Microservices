using Micro.Application.Interfaces.Repositories;
using Micro.Domain.Entities;
using Micro.Persistence.Context;

namespace Micro.Persistence.Repositories;

public class OutboxRepository : BaseRepository<OutboxMessage, int>, IOutboxRepository
{
    public OutboxRepository(ApplicationDbContext context) : base(context)
    {
    }
}