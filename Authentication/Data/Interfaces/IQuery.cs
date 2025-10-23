using System.Linq.Expressions;

namespace Authentication.Data.Interfaces;

/// <summary>
/// Query interface for building and executing read-only queries
/// </summary>
/// <typeparam name="T">Entity type</typeparam>
public interface IQuery<T> where T : class
{
    /// <summary>
    /// Apply a filter to the query
    /// </summary>
    IQuery<T> Where(Expression<Func<T, bool>> predicate);

    /// <summary>
    /// Include related entities
    /// </summary>
    IQuery<T> Include(Expression<Func<T, object>> navigationProperty);

    /// <summary>
    /// Order by ascending
    /// </summary>
    IQuery<T> OrderBy<TKey>(Expression<Func<T, TKey>> keySelector);

    /// <summary>
    /// Order by descending
    /// </summary>
    IQuery<T> OrderByDescending<TKey>(Expression<Func<T, TKey>> keySelector);

    /// <summary>
    /// Take a specific number of entities
    /// </summary>
    IQuery<T> Take(int count);

    /// <summary>
    /// Skip a specific number of entities
    /// </summary>
    IQuery<T> Skip(int count);

    /// <summary>
    /// Execute the query and return all results
    /// </summary>
    Task<IEnumerable<T>> ToListAsync();

    /// <summary>
    /// Execute the query and return first result or null
    /// </summary>
    Task<T?> FirstOrDefaultAsync();

    /// <summary>
    /// Execute the query and return single result or null
    /// </summary>
    Task<T?> SingleOrDefaultAsync();

    /// <summary>
    /// Execute the query and check if any results exist
    /// </summary>
    Task<bool> AnyAsync();

    /// <summary>
    /// Execute the query and count results
    /// </summary>
    Task<int> CountAsync();

    /// <summary>
    /// Get the underlying IQueryable for advanced scenarios
    /// </summary>
    IQueryable<T> AsQueryable();
}