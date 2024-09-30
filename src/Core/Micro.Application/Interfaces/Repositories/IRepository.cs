using System.Linq.Expressions;
using Micro.Domain.Entities;

namespace Micro.Application.Interfaces.Repositories;

public interface IRepository <T, TKey> where T : Entity<TKey>
{
    Task<T> GetAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties);

    Task<T> GetAsyncV2(IList<Expression<Func<T, bool>>> predicates, IList<Expression<Func<T, object>>> includeProperties);

    Task<IList<T>> GetAllAsync(Expression<Func<T, bool>> predicate = null, params Expression<Func<T, object>>[] includeProperties);

    Task<IList<T>> GetAllAsyncV2(IList<Expression<Func<T, bool>>> predicates, IList<Expression<Func<T, object>>> includeProperties);

    Task<T> AddAsync(T entity);

    Task<IList<T>> AddInBulkAsync(IList<T> entities);

    Task<T> UpdateAsync(T entity);

    Task DeleteAsync(T entity);

    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);

    Task<int> CountAsync(Expression<Func<T, bool>> predicate = null);

    IQueryable<T> GetAsQueryable();
    Task<IList<T>> ExecuteSqlQueryAsync(string sql, params object[] parameters);
    
    Task<int> SaveChangesAsync();
    
}