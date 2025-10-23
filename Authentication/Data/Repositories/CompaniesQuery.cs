using Authentication.Data.DTOs;
using Authentication.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Authentication.Data.Repositories;

/// <summary>
/// Query service implementation for reading company data (CQRS read side)
/// Uses AsNoTracking for optimized read-only queries with DTO projection
/// </summary>
public class CompaniesQuery : ICompaniesQuery
{
    private readonly ApplicationDbContext _context;

    public CompaniesQuery(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<CompanyDto>> GetCompaniesByUserIdAsync(string userId)
    {
        return await _context.Companies
            .AsNoTracking()
            .Where(c => c.UserId == userId && !c.Deleted)
            .Select(c => new CompanyDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                UserId = c.UserId,
                UserName = c.User != null ? c.User.UserName : null,
                AddressCount = c.Addresses.Count
            })
            .ToListAsync();
    }

    public async Task<PaginatedResult<CompanyDto>> GetCompaniesByUserIdAsync(string userId, int pageNumber, int pageSize)
    {
        var query = _context.Companies
            .AsNoTracking()
            .Where(c => c.UserId == userId && !c.Deleted);

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderBy(c => c.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new CompanyDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                UserId = c.UserId,
                UserName = c.User != null ? c.User.UserName : null,
                AddressCount = c.Addresses.Count
            })
            .ToListAsync();

        return new PaginatedResult<CompanyDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<CompanyDto?> GetCompanyByIdAsync(Guid id, string userId)
    {
        return await _context.Companies
            .AsNoTracking()
            .Where(c => c.Id == id && c.UserId == userId && !c.Deleted)
            .Select(c => new CompanyDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                UserId = c.UserId,
                UserName = c.User != null ? c.User.UserName : null,
                AddressCount = c.Addresses.Count
            })
            .FirstOrDefaultAsync();
    }

    public async Task<CompanyDetailsDto?> GetCompanyWithDetailsAsync(Guid id, string userId)
    {
        return await _context.Companies
            .AsNoTracking()
            .Where(c => c.Id == id && c.UserId == userId && !c.Deleted)
            .Select(c => new CompanyDetailsDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                UserId = c.UserId,
                UserName = c.User != null ? c.User.UserName : null,
                AddressCount = c.Addresses.Count,
                Addresses = c.Addresses.Select(a => new AddressDto
                {
                    Id = a.Id,
                    CompanyId = a.CompanyId,
                    Street = a.Street,
                    Suite = a.Suite,
                    City = a.City,
                    State = a.State,
                    PostalCode = a.PostalCode,
                    Country = a.Country
                }).ToList()
            })
            .FirstOrDefaultAsync();
    }

    public async Task<bool> ExistsAsync(Guid id, string userId)
    {
        return await _context.Companies
            .AsNoTracking()
            .AnyAsync(c => c.Id == id && c.UserId == userId && !c.Deleted);
    }

    public async Task<int> CountAsync(string userId)
    {
        return await _context.Companies
            .AsNoTracking()
            .CountAsync(c => c.UserId == userId && !c.Deleted);
    }
}