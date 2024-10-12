using System.Linq.Expressions;
using Wsrc.Domain.Entities;

namespace Wsrc.Core.Interfaces.Repositories;

public interface IAsyncRepository<T> where T : EntityBase
{
    ValueTask<T?> GetByIdAsync(int id);
    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);

    Task AddAsync(T entity);
    Task AddRangeAsync(IEnumerable<T> entities);
    Task UpdateAsync(T entity);
    Task RemoveAsync(T entity);

    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> GetWhereAsync(Expression<Func<T, bool>> predicate);

    Task<int> CountAllAsync();
    Task<int> CountWhereAsync(Expression<Func<T, bool>> predicate);
}