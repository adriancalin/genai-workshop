using Authentication.Data.DTOs;
using Authentication.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Authentication.Data.Repositories;

/// <summary>
/// Query service implementation for reading address data (CQRS read side)
/// Uses AsNoTracking for optimized read-only queries with DTO projection
/// </summary>
public class AddressesQuery : IAddressesQuery
{
    private readonly ApplicationDbContext _context;

    public AddressesQuery(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<AddressDto>> GetAddressesByCompanyIdAsync(Guid companyId, string userId)
    {
        return await _context.Addresses
            .AsNoTracking()
            .Where(a => a.CompanyId == companyId && a.Company!.UserId == userId && !a.Company.Deleted)
            .Select(a => new AddressDto
            {
                Id = a.Id,
                CompanyId = a.CompanyId,
                Street = a.Street,
                Suite = a.Suite,
                City = a.City,
                State = a.State,
                PostalCode = a.PostalCode,
                Country = a.Country
            })
            .ToListAsync();
    }

    public async Task<AddressDto?> GetAddressByIdAsync(Guid id, string userId)
    {
        return await _context.Addresses
            .AsNoTracking()
            .Where(a => a.Id == id && a.Company!.UserId == userId && !a.Company.Deleted)
            .Select(a => new AddressDto
            {
                Id = a.Id,
                CompanyId = a.CompanyId,
                Street = a.Street,
                Suite = a.Suite,
                City = a.City,
                State = a.State,
                PostalCode = a.PostalCode,
                Country = a.Country
            })
            .FirstOrDefaultAsync();
    }

    public async Task<bool> ExistsAsync(Guid id, string userId)
    {
        return await _context.Addresses
            .AsNoTracking()
            .AnyAsync(a => a.Id == id && a.Company!.UserId == userId && !a.Company.Deleted);
    }

    public async Task<int> CountAsync(Guid companyId, string userId)
    {
        return await _context.Addresses
            .AsNoTracking()
            .CountAsync(a => a.CompanyId == companyId && a.Company!.UserId == userId && !a.Company.Deleted);
    }
}
