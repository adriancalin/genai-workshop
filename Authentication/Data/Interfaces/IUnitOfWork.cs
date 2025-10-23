namespace Authentication.Data.Interfaces;

/// <summary>
/// Unit of Work interface for coordinating repository operations and transactions
/// </summary>
public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// Get repository for a specific entity type
    /// </summary>
    IRepository<T> Repository<T>() where T : class;

    /// <summary>
    /// Get query builder for a specific entity type
    /// </summary>
    IQuery<T> Query<T>() where T : class;

    /// <summary>
    /// Save all changes to the database
    /// </summary>
    Task<int> SaveChangesAsync();

    /// <summary>
    /// Begin a database transaction
    /// </summary>
    Task BeginTransactionAsync();

    /// <summary>
    /// Commit the current transaction
    /// </summary>
    Task CommitTransactionAsync();

    /// <summary>
    /// Rollback the current transaction
    /// </summary>
    Task RollbackTransactionAsync();
}