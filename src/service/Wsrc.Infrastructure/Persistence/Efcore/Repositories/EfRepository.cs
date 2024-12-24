using System.Linq.Expressions;

using Microsoft.EntityFrameworkCore;

using Wsrc.Core.Interfaces.Repositories;
using Wsrc.Domain.Entities;

namespace Wsrc.Infrastructure.Persistence.Efcore.Repositories;

public class EfRepository<T>(WsrcContext context)
    : IAsyncRepository<T>
    where T
    : EntityBase
{
    protected readonly WsrcContext Context = context;

    public async ValueTask<T?> GetByIdAsync(int id)
    {
        var entity = await Context.Set<T>().FindAsync(id);

        if (entity is not null)
        {
            Context.Entry(entity).State = EntityState.Detached;
        }

        return entity;
    }

    public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
    {
        var entity = await Context.Set<T>().FirstOrDefaultAsync(predicate);

        if (entity is not null)
        {
            Context.Entry(entity).State = EntityState.Detached;
        }

        return entity;
    }

    public async Task AddAsync(T entity)
    {
        await Context.Set<T>().AddAsync(entity);
        await Context.SaveChangesAsync();
    }

    public async Task AddRangeAsync(IEnumerable<T> entities)
    {
        await Context.Set<T>().AddRangeAsync(entities);
        await Context.SaveChangesAsync();
    }

    public Task UpdateAsync(T entity)
    {
        Context.Entry(entity).State = EntityState.Modified;
        return Context.SaveChangesAsync();
    }

    public Task RemoveAsync(T entity)
    {
        Context.Set<T>().Remove(entity);
        return Context.SaveChangesAsync();
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await Context.Set<T>().ToListAsync();
    }

    public async Task<IEnumerable<T>> GetWhereAsync(Expression<Func<T, bool>> predicate)
    {
        return await Context.Set<T>().Where(predicate).ToListAsync();
    }

    public Task<int> CountAllAsync() => Context.Set<T>().CountAsync();

    public Task<int> CountWhereAsync(Expression<Func<T, bool>> predicate)
        => Context.Set<T>().CountAsync(predicate);

    public async Task<int> SaveChangesAsync()
    {
        await using var transaction = await Context.Database.BeginTransactionAsync();

        try
        {
            var result = await Context.SaveChangesAsync();

            await transaction.CommitAsync();

            return result;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();

            throw new Exception("Failed to save changes to the database", ex);
        }
    }
}