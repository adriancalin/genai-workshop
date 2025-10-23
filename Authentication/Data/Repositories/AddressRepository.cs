using Authentication.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Authentication.Data.Repositories;

/// <summary>
/// Address repository for write operations (CQRS write side)
/// For read operations, use AddressesQuery
/// Uses change tracking for updates
/// </summary>
public class AddressRepository : Repository<Address>, IAddressRepository
{
    public AddressRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Address?> GetForUpdateAsync(Guid id, string userId)
    {
        return await _dbSet
            .Include(a => a.Company)
            .FirstOrDefaultAsync(a => a.Id == id && a.Company!.UserId == userId && !a.Company.Deleted);
    }

    public async Task<Address> CreateAddressAsync(Address address, string userId)
    {
        // Verify company ownership
        var companyExists = await IsCompanyOwnedByUserAsync(address.CompanyId!.Value, userId);
        if (!companyExists)
        {
            throw new UnauthorizedAccessException("Company not found or not owned by user");
        }

        await _dbSet.AddAsync(address);
        return address;
    }

    public async Task UpdateAddressAsync(Address address)
    {
        _dbSet.Update(address);
        await Task.CompletedTask;
    }

    public async Task<bool> DeleteAddressAsync(Guid id, string userId)
    {
        var address = await _dbSet
            .Include(a => a.Company)
            .FirstOrDefaultAsync(a => a.Id == id && a.Company!.UserId == userId && !a.Company.Deleted);

        if (address == null)
            return false;

        _dbSet.Remove(address);
        return true;
    }

    public async Task<bool> IsOwnedByUserAsync(Guid id, string userId)
    {
        return await _dbSet
            .Include(a => a.Company)
            .AnyAsync(a => a.Id == id && a.Company!.UserId == userId && !a.Company.Deleted);
    }

    public async Task<bool> IsCompanyOwnedByUserAsync(Guid companyId, string userId)
    {
        return await _context.Companies
            .AnyAsync(c => c.Id == companyId && c.UserId == userId && !c.Deleted);
    }
}
