using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Authentication;
using Authentication.Data.Interfaces;
using Authentication.Data.DTOs;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations;

namespace Web.Pages;

[Authorize]
public class ManageAddressesModel : PageModel
{
    private readonly ICompaniesQuery _companiesQuery;
    private readonly IAddressesQuery _addressesQuery;
    private readonly IAddressRepository _addressRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ManageAddressesModel> _logger;

    public ManageAddressesModel(
        ICompaniesQuery companiesQuery,
        IAddressesQuery addressesQuery,
        IAddressRepository addressRepository,
        IUnitOfWork unitOfWork,
        ILogger<ManageAddressesModel> logger)
    {
        _companiesQuery = companiesQuery;
        _addressesQuery = addressesQuery;
        _addressRepository = addressRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    [BindProperty(SupportsGet = true)]
    public Guid Id { get; set; } // Company ID

    public CompanyDto Company { get; set; } = default!;
    public IEnumerable<AddressDto> Addresses { get; set; } = new List<AddressDto>();

    [BindProperty]
    public AddressInput Input { get; set; } = default!;

    [BindProperty]
    public Guid? EditingAddressId { get; set; }

    public class AddressInput
    {
        [Required(ErrorMessage = "Street is required")]
        [StringLength(200, ErrorMessage = "Street cannot exceed 200 characters")]
        public string Street { get; set; } = null!;

        [StringLength(100, ErrorMessage = "Suite cannot exceed 100 characters")]
        public string? Suite { get; set; }

        [Required(ErrorMessage = "City is required")]
        [StringLength(100, ErrorMessage = "City cannot exceed 100 characters")]
        public string City { get; set; } = null!;

        [StringLength(100, ErrorMessage = "State cannot exceed 100 characters")]
        public string? State { get; set; }

        [Required(ErrorMessage = "Postal code is required")]
        [StringLength(20, ErrorMessage = "Postal code cannot exceed 20 characters")]
        public string PostalCode { get; set; } = null!;

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

        var company = await _companiesQuery.GetCompanyByIdAsync(Id, userId);
        if (company == null)
        {
            return NotFound();
        }

        Company = company;
        Addresses = await _addressesQuery.GetAddressesByCompanyIdAsync(Id, userId);

        return Page();
    }

    public async Task<IActionResult> OnPostCreateAsync()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        if (!ModelState.IsValid)
        {
            await LoadPageDataAsync(userId);
            return Page();
        }

        var address = new Address
        {
            CompanyId = Id,
            Street = Input.Street,
            Suite = Input.Suite,
            City = Input.City,
            State = Input.State,
            PostalCode = Input.PostalCode,
            Country = Input.Country
        };

        try
        {
            await _addressRepository.CreateAddressAsync(address, userId);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("User {UserId} created address {AddressId} for company {CompanyId}", userId, address.Id, Id);
            TempData["SuccessMessage"] = "Address added successfully!";

            return RedirectToPage(new { Id });
        }
        catch (UnauthorizedAccessException)
        {
            ModelState.AddModelError(string.Empty, "You don't have permission to add addresses to this company.");
            await LoadPageDataAsync(userId);
            return Page();
        }
    }

    public async Task<IActionResult> OnPostEditAsync(Guid addressId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        if (!ModelState.IsValid)
        {
            EditingAddressId = addressId;
            await LoadPageDataAsync(userId);
            return Page();
        }

        var address = await _addressRepository.GetForUpdateAsync(addressId, userId);
        if (address == null)
        {
            TempData["ErrorMessage"] = "Address not found or you don't have permission to edit it.";
            return RedirectToPage(new { Id });
        }

        address.Street = Input.Street;
        address.Suite = Input.Suite;
        address.City = Input.City;
        address.State = Input.State;
        address.PostalCode = Input.PostalCode;
        address.Country = Input.Country;

        await _addressRepository.UpdateAddressAsync(address);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("User {UserId} updated address {AddressId}", userId, addressId);
        TempData["SuccessMessage"] = "Address updated successfully!";

        return RedirectToPage(new { Id });
    }

    public async Task<IActionResult> OnPostDeleteAsync(Guid addressId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var deleted = await _addressRepository.DeleteAddressAsync(addressId, userId);
        if (deleted)
        {
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("User {UserId} deleted address {AddressId}", userId, addressId);
            TempData["SuccessMessage"] = "Address deleted successfully!";
        }
        else
        {
            TempData["ErrorMessage"] = "Address not found or you don't have permission to delete it.";
        }

        return RedirectToPage(new { Id });
    }

    private async Task LoadPageDataAsync(string userId)
    {
        var company = await _companiesQuery.GetCompanyByIdAsync(Id, userId);
        if (company != null)
        {
            Company = company;
            Addresses = await _addressesQuery.GetAddressesByCompanyIdAsync(Id, userId);
        }
    }
}
