namespace Authentication.Data.Interfaces;

/// <summary>
/// Address repository interface for write operations (CQRS write side)
/// For read operations, use IAddressesQuery
/// </summary>
public interface IAddressRepository : IRepository<Address>
{
    /// <summary>
    /// Get an address by ID for updating (with change tracking)
    /// Verifies that the address belongs to a company owned by the user
    /// </summary>
    Task<Address?> GetForUpdateAsync(Guid id, string userId);

    /// <summary>
    /// Create a new address for a company
    /// </summary>
    Task<Address> CreateAddressAsync(Address address, string userId);

    /// <summary>
    /// Update an existing address
    /// </summary>
    Task UpdateAddressAsync(Address address);

    /// <summary>
    /// Delete an address if the company is owned by user
    /// </summary>
    Task<bool> DeleteAddressAsync(Guid id, string userId);

    /// <summary>
    /// Validate that the address belongs to a company owned by the user
    /// </summary>
    Task<bool> IsOwnedByUserAsync(Guid id, string userId);

    /// <summary>
    /// Validate that a company is owned by the user
    /// </summary>
    Task<bool> IsCompanyOwnedByUserAsync(Guid companyId, string userId);
}
