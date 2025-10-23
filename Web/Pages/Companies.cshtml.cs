using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Authentication;
using System.Security.Claims;

namespace Web.Pages;

public class CompaniesModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<CompaniesModel> _logger;

    public CompaniesModel(ApplicationDbContext context, ILogger<CompaniesModel> logger)
    {
        _context = context;
        _logger = logger;
    }

    public IList<Company> Companies { get; set; } = default!;
    public string? CurrentUserId { get; set; }

    public async Task OnGetAsync()
    {
        CurrentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        var query = _context.Companies
            .Include(c => c.Address)
            .Include(c => c.User)
            .AsQueryable();

        // Filter to show only the current user's companies if authenticated
        if (!string.IsNullOrEmpty(CurrentUserId))
        {
            query = query.Where(c => c.UserId == CurrentUserId);
        }

        Companies = await query.ToListAsync();
    }
}