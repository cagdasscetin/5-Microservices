using Micro.Domain.Entities;

namespace Micro.Application.Interfaces.Repositories;

public interface IOutboxRepository : IRepository<OutboxMessage, int>
{
    
}