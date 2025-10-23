using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Authentication.Data.Interfaces;
using Authentication.Data.DTOs;
using System.Security.Claims;

namespace Web.Pages;

public class CompaniesModel : PageModel
{
    private readonly ICompaniesQuery _companiesQuery;
    private readonly ICompanyRepository _companyRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CompaniesModel> _logger;

    public CompaniesModel(
        ICompaniesQuery companiesQuery,
        ICompanyRepository companyRepository,
        IUnitOfWork unitOfWork,
        ILogger<CompaniesModel> logger)
    {
        _companiesQuery = companiesQuery;
        _companyRepository = companyRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public IList<CompanyDto> Companies { get; set; } = default!;
    public string? CurrentUserId { get; set; }

    public async Task OnGetAsync()
    {
        CurrentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (!string.IsNullOrEmpty(CurrentUserId))
        {
            Companies = (await _companiesQuery.GetCompaniesByUserIdAsync(CurrentUserId)).ToList();
        }
        else
        {
            Companies = new List<CompanyDto>();
        }
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var deleted = await _companyRepository.DeleteCompanyAsync(id, userId);
        
        if (!deleted)
        {
            _logger.LogWarning("User {UserId} attempted to delete company {CompanyId} but it was not found or not owned", userId, id);
            return NotFound();
        }

        await _unitOfWork.SaveChangesAsync();
        _logger.LogInformation("User {UserId} deleted company {CompanyId}", userId, id);

        return RedirectToPage();
    }
}