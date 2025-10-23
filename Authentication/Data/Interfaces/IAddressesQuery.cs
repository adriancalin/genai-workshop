using Authentication.Data.DTOs;

namespace Authentication.Data.Interfaces;

/// <summary>
/// Query service for reading address data (CQRS read side)
/// </summary>
public interface IAddressesQuery
{
    /// <summary>
    /// Get all addresses for a specific company
    /// </summary>
    Task<IEnumerable<AddressDto>> GetAddressesByCompanyIdAsync(Guid companyId, string userId);

    /// <summary>
    /// Get an address by ID
    /// </summary>
    Task<AddressDto?> GetAddressByIdAsync(Guid id, string userId);

    /// <summary>
    /// Check if an address exists and belongs to a company owned by the user
    /// </summary>
    Task<bool> ExistsAsync(Guid id, string userId);

    /// <summary>
    /// Get total count of addresses for a company
    /// </summary>
    Task<int> CountAsync(Guid companyId, string userId);
}
