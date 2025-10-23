using Authentication.Data.DTOs;

namespace Authentication.Data.Interfaces;

/// <summary>
/// Query service for reading company data (CQRS read side)
/// </summary>
public interface ICompaniesQuery
{
    /// <summary>
    /// Get all companies for a specific user
    /// </summary>
    Task<IEnumerable<CompanyDto>> GetCompaniesByUserIdAsync(string userId);

    /// <summary>
    /// Get paginated companies for a specific user
    /// </summary>
    Task<PaginatedResult<CompanyDto>> GetCompaniesByUserIdAsync(string userId, int pageNumber, int pageSize);

    /// <summary>
    /// Get a company by ID (basic info)
    /// </summary>
    Task<CompanyDto?> GetCompanyByIdAsync(int id, string userId);

    /// <summary>
    /// Get a company with full details including address
    /// </summary>
    Task<CompanyDetailsDto?> GetCompanyWithDetailsAsync(int id, string userId);

    /// <summary>
    /// Check if a company exists and belongs to the user
    /// </summary>
    Task<bool> ExistsAsync(int id, string userId);

    /// <summary>
    /// Get total count of companies for a user
    /// </summary>
    Task<int> CountAsync(string userId);
}