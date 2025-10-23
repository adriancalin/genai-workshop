using Authentication.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Authentication.Data.Repositories;

/// <summary>
/// Company repository for write operations (CQRS write side)
/// For read operations, use CompaniesQuery
/// Uses change tracking for updates
/// </summary>
public class CompanyRepository : Repository<Company>, ICompanyRepository
{
    public CompanyRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Company?> GetForUpdateAsync(int id, string userId)
    {
        return await _dbSet
            .Include(c => c.Address)
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId && !c.Deleted);
    }

    public async Task<Company> CreateCompanyAsync(Company company)
    {
        await _dbSet.AddAsync(company);
        return company;
    }

    public async Task UpdateCompanyAsync(Company company)
    {
        _dbSet.Update(company);
        await Task.CompletedTask;
    }

    public async Task<bool> DeleteCompanyAsync(int id, string userId)
    {
        var company = await _dbSet
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId && !c.Deleted);

        if (company == null)
            return false;

        // Soft delete: set Deleted flag instead of removing
        company.Deleted = true;
        _dbSet.Update(company);
        return true;
    }

    public async Task<bool> IsOwnedByUserAsync(int id, string userId)
    {
        return await _dbSet.AnyAsync(c => c.Id == id && c.UserId == userId && !c.Deleted);
    }
}