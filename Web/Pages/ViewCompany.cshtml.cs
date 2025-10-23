using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Authentication.Data.Interfaces;
using Authentication.Data.DTOs;
using System.Security.Claims;

namespace Web.Pages;

[Authorize]
public class ViewCompanyModel : PageModel
{
    private readonly ICompaniesQuery _companiesQuery;
    private readonly ILogger<ViewCompanyModel> _logger;

    public ViewCompanyModel(ICompaniesQuery companiesQuery, ILogger<ViewCompanyModel> logger)
    {
        _companiesQuery = companiesQuery;
        _logger = logger;
    }

    public CompanyDetailsDto Company { get; set; } = default!;

    [BindProperty(SupportsGet = true)]
    public Guid Id { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var company = await _companiesQuery.GetCompanyWithDetailsAsync(Id, userId);
        if (company == null)
        {
            return NotFound();
        }

        Company = company;
        return Page();
    }
}
