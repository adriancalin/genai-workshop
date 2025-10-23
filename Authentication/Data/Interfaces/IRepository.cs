using System.Linq.Expressions;

namespace Authentication.Data.Interfaces;

/// <summary>
/// Generic repository interface for CRUD operations
/// </summary>
/// <typeparam name="T">Entity type</typeparam>
public interface IRepository<T> where T : class
{
    /// <summary>
    /// Get entity by ID
    /// </summary>
    Task<T?> GetByIdAsync(object id);

    /// <summary>
    /// Get all entities
    /// </summary>
    Task<IEnumerable<T>> GetAllAsync();

    /// <summary>
    /// Find entities matching a predicate
    /// </summary>
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);

    /// <summary>
    /// Get first entity matching a predicate
    /// </summary>
    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);

    /// <summary>
    /// Add a new entity
    /// </summary>
    Task AddAsync(T entity);

    /// <summary>
    /// Add multiple entities
    /// </summary>
    Task AddRangeAsync(IEnumerable<T> entities);

    /// <summary>
    /// Update an existing entity
    /// </summary>
    void Update(T entity);

    /// <summary>
    /// Update multiple entities
    /// </summary>
    void UpdateRange(IEnumerable<T> entities);

    /// <summary>
    /// Remove an entity
    /// </summary>
    void Remove(T entity);

    /// <summary>
    /// Remove multiple entities
    /// </summary>
    void RemoveRange(IEnumerable<T> entities);

    /// <summary>
    /// Check if any entity matches the predicate
    /// </summary>
    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);

    /// <summary>
    /// Count entities matching the predicate
    /// </summary>
    Task<int> CountAsync(Expression<Func<T, bool>> predicate);
}