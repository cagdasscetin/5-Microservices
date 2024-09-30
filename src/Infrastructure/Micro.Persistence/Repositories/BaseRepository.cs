using System.Linq.Expressions;
using Micro.Application.Interfaces.Repositories;
using Micro.Domain.Entities;
using Micro.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Micro.Persistence.Repositories;

public class BaseRepository<T, TKey> : IRepository<T, TKey> where T : Entity<TKey>
{
    private readonly ApplicationDbContext _context;

    public BaseRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<T> AddAsync(T entity)
    {
        await _context.Set<T>().AddAsync(entity);
        return entity;
    }
    public async Task<IList<T>> AddInBulkAsync(IList<T> entities)
    {
        await _context.Set<T>().AddRangeAsync(entities);
        return entities;
    }
    public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
    {
        return await _context.Set<T>().AnyAsync(predicate);
    }

    public async Task<int> CountAsync(Expression<Func<T, bool>> predicate = null)
    {
        return await (predicate == null ? _context.Set<T>().CountAsync() : _context.Set<T>().CountAsync(predicate));
    }

    public async Task DeleteAsync(T entity)
    {
        await Task.Run(() => { _context.Set<T>().Remove(entity); });
    }
    public async Task<IList<T>> ExecuteSqlQueryAsync(string sql, params object[] parameters)
    {
        return await _context.Set<T>().FromSqlRaw(sql, parameters).ToListAsync();
    }
    public async Task<IList<T>> GetAllAsync(Expression<Func<T, bool>> predicate = null, params Expression<Func<T, object>>[] includeProperties)
    {
        IQueryable<T> query = _context.Set<T>();

        if (predicate != null)
        {
            query = query.Where(predicate);
        }

        if (includeProperties.Any())
        {
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
        }

        return await query.AsNoTracking().ToListAsync();
    }

    public async Task<IList<T>> GetAllAsyncV2(IList<Expression<Func<T, bool>>> predicates, IList<Expression<Func<T, object>>> includeProperties)
    {

        IQueryable<T> query = _context.Set<T>();

        if (predicates != null && predicates.Any())
        {
            foreach (var predicate in predicates)
            {
                query = query.Where(predicate);
            }
        }

        if (includeProperties != null && includeProperties.Any())
        {
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
        }

        return await query.AsNoTracking().ToListAsync();
    }

    public IQueryable<T> GetAsQueryable()
    {
        return _context.Set<T>().AsQueryable();
    }

    public async Task<T> GetAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties)
    {
        IQueryable<T> query = _context.Set<T>();

        query = query.Where(predicate);

        if (includeProperties.Any())
        {
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
        }

        return await query.AsNoTracking().FirstOrDefaultAsync();
    }

    public async Task<T> GetAsyncV2(IList<Expression<Func<T, bool>>> predicates, IList<Expression<Func<T, object>>> includeProperties)
    {
        IQueryable<T> query = _context.Set<T>();

        if (predicates != null && predicates.Any())
        {
            foreach (var predicate in predicates)
            {
                query = query.Where(predicate);
            }
        }

        if (includeProperties != null && includeProperties.Any())
        {
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
        }

        return await query.AsNoTracking().SingleOrDefaultAsync();
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task<T> UpdateAsync(T entity)
    {
        await Task.Run(() => { _context.Set<T>().Update(entity); });
        return entity;
    }
}