namespace Authentication.Data.Interfaces;

/// <summary>
/// Company repository interface for write operations (CQRS write side)
/// For read operations, use ICompaniesQuery
/// </summary>
public interface ICompanyRepository : IRepository<Company>
{
    /// <summary>
    /// Get a company by ID for updating (with change tracking)
    /// </summary>
    Task<Company?> GetForUpdateAsync(Guid id, string userId);

    /// <summary>
    /// Create a new company
    /// </summary>
    Task<Company> CreateCompanyAsync(Company company);

    /// <summary>
    /// Update an existing company
    /// </summary>
    Task UpdateCompanyAsync(Company company);

    /// <summary>
    /// Delete a company if owned by user
    /// </summary>
    Task<bool> DeleteCompanyAsync(Guid id, string userId);

    /// <summary>
    /// Validate company ownership (for authorization checks)
    /// </summary>
    Task<bool> IsOwnedByUserAsync(Guid id, string userId);
}