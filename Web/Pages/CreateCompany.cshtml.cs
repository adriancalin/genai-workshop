using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Authentication;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations;

namespace Web.Pages;

[Authorize]
public class CreateCompanyModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<CreateCompanyModel> _logger;

    public CreateCompanyModel(ApplicationDbContext context, ILogger<CreateCompanyModel> logger)
    {
        _context = context;
        _logger = logger;
    }

    [BindProperty]
    public CompanyInput Input { get; set; } = default!;

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

    public void OnGet()
    {
        Input = new CompanyInput();
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
            ModelState.AddModelError(string.Empty, "Unable to determine user identity.");
            return Page();
        }

        var company = new Company
        {
            Name = Input.Name,
            Description = Input.Description,
            UserId = userId
        };

        // Create address if any address field is provided
        if (!string.IsNullOrWhiteSpace(Input.Street) || 
            !string.IsNullOrWhiteSpace(Input.City) || 
            !string.IsNullOrWhiteSpace(Input.PostalCode))
        {
            company.Address = new Address
            {
                Street = Input.Street ?? string.Empty,
                Suite = Input.Suite,
                City = Input.City ?? string.Empty,
                State = Input.State,
                PostalCode = Input.PostalCode ?? string.Empty,
                Country = Input.Country
            };
        }

        _context.Companies.Add(company);
        await _context.SaveChangesAsync();

        _logger.LogInformation("User {UserId} created company {CompanyId}", userId, company.Id);

        return RedirectToPage("./Companies");
    }
}