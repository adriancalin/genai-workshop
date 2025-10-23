using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Authentication;
using Authentication.Data.Interfaces;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations;

namespace Web.Pages;

[Authorize]
public class EditCompanyModel : PageModel
{
    private readonly ICompaniesQuery _companiesQuery;
    private readonly ICompanyRepository _companyRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<EditCompanyModel> _logger;

    public EditCompanyModel(
        ICompaniesQuery companiesQuery,
        ICompanyRepository companyRepository,
        IUnitOfWork unitOfWork,
        ILogger<EditCompanyModel> logger)
    {
        _companiesQuery = companiesQuery;
        _companyRepository = companyRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    [BindProperty]
    public CompanyInput Input { get; set; } = default!;

    [BindProperty(SupportsGet = true)]
    public int Id { get; set; }

    public class CompanyInput
    {
        [Required(ErrorMessage = "Company name is required")]
        [StringLength(200, ErrorMessage = "Company name cannot exceed 200 characters")]
        public string Name { get; set; } = null!;

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string? Description { get; set; }

        // Address fields (all optional)
        [StringLength(200, ErrorMessage = "Street cannot exceed 200 characters")]
        public string? Street { get; set; }

        [StringLength(100, ErrorMessage = "Suite cannot exceed 100 characters")]
        public string? Suite { get; set; }

        [StringLength(100, ErrorMessage = "City cannot exceed 100 characters")]
        public string? City { get; set; }

        [StringLength(100, ErrorMessage = "State cannot exceed 100 characters")]
        public string? State { get; set; }

        [StringLength(20, ErrorMessage = "Postal code cannot exceed 20 characters")]
        public string? PostalCode { get; set; }

        [StringLength(100, ErrorMessage = "Country cannot exceed 100 characters")]
        public string? Country { get; set; }
    }

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

        Input = new CompanyInput
        {
            Name = company.Name,
            Description = company.Description,
            Street = company.Address?.Street,
            Suite = company.Address?.Suite,
            City = company.Address?.City,
            State = company.Address?.State,
            PostalCode = company.Address?.PostalCode,
            Country = company.Address?.Country
        };

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var company = await _companyRepository.GetForUpdateAsync(Id, userId);
        if (company == null)
        {
            return NotFound();
        }

        company.Name = Input.Name;
        company.Description = Input.Description;

        // Update or create address if any address field is provided
        if (!string.IsNullOrWhiteSpace(Input.Street) ||
            !string.IsNullOrWhiteSpace(Input.City) ||
            !string.IsNullOrWhiteSpace(Input.PostalCode))
        {
            if (company.Address == null)
            {
                company.Address = new Address();
            }

            company.Address.Street = Input.Street ?? string.Empty;
            company.Address.Suite = Input.Suite;
            company.Address.City = Input.City ?? string.Empty;
            company.Address.State = Input.State;
            company.Address.PostalCode = Input.PostalCode ?? string.Empty;
            company.Address.Country = Input.Country;
        }
        else if (company.Address != null)
        {
            // Remove address if all fields are empty
            company.Address = null;
        }

        await _companyRepository.UpdateCompanyAsync(company);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("User {UserId} updated company {CompanyId}", userId, company.Id);

        return RedirectToPage("./Companies");
    }
}
