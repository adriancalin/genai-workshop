using System.Linq.Expressions;
using Authentication.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Authentication.Data.Repositories;

/// <summary>
/// Query implementation for building and executing read-only queries
/// </summary>
/// <typeparam name="T">Entity type</typeparam>
public class Query<T> : IQuery<T> where T : class
{
    private IQueryable<T> _query;

    public Query(DbSet<T> dbSet)
    {
        _query = dbSet.AsQueryable();
    }

    public IQuery<T> Where(Expression<Func<T, bool>> predicate)
    {
        _query = _query.Where(predicate);
        return this;
    }

    public IQuery<T> Include(Expression<Func<T, object>> navigationProperty)
    {
        _query = _query.Include(navigationProperty);
        return this;
    }

    public IQuery<T> OrderBy<TKey>(Expression<Func<T, TKey>> keySelector)
    {
        _query = _query.OrderBy(keySelector);
        return this;
    }

    public IQuery<T> OrderByDescending<TKey>(Expression<Func<T, TKey>> keySelector)
    {
        _query = _query.OrderByDescending(keySelector);
        return this;
    }

    public IQuery<T> Take(int count)
    {
        _query = _query.Take(count);
        return this;
    }

    public IQuery<T> Skip(int count)
    {
        _query = _query.Skip(count);
        return this;
    }

    public async Task<IEnumerable<T>> ToListAsync()
    {
        return await _query.ToListAsync();
    }

    public async Task<T?> FirstOrDefaultAsync()
    {
        return await _query.FirstOrDefaultAsync();
    }

    public async Task<T?> SingleOrDefaultAsync()
    {
        return await _query.SingleOrDefaultAsync();
    }

    public async Task<bool> AnyAsync()
    {
        return await _query.AnyAsync();
    }

    public async Task<int> CountAsync()
    {
        return await _query.CountAsync();
    }

    public IQueryable<T> AsQueryable()
    {
        return _query;
    }
}