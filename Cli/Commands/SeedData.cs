using Spectre.Console.Cli;
using System.Diagnostics.CodeAnalysis;
using Authentication;
using Microsoft.EntityFrameworkCore;

namespace Commands;

public class SeedData : Command<SeedData.Settings>
{
    private readonly ApplicationDbContext _dbContext;

    public SeedData(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public sealed class Settings : CommandSettings
    {
    }

    public override int Execute([NotNull] CommandContext context, [NotNull] Settings settings)
    {
        Console.WriteLine("Seeding sample companies and addresses...");
        
        // Get the first user (owner) to assign companies to
        var owner = _dbContext.Users.FirstOrDefault(u => u.UserName == "owner");
        if (owner == null)
        {
            Console.WriteLine("Error: No 'owner' user found. Please run 'resetdb' first.");
            return 1;
        }
        
        // Clear existing data
        _dbContext.Addresses.RemoveRange(_dbContext.Addresses);
        _dbContext.Companies.RemoveRange(_dbContext.Companies);
        _dbContext.SaveChanges();

        // Create sample companies with addresses
        var companies = new List<Company>
        {
            new Company
            {
                Name = "Tech Solutions Inc.",
                Description = "Leading provider of innovative technology solutions for modern businesses.",
                UserId = owner.Id,
                Address = new Address
                {
                    Street = "123 Silicon Valley Blvd",
                    Suite = "Suite 400",
                    City = "San Francisco",
                    State = "CA",
                    PostalCode = "94105",
                    Country = "USA"
                }
            },
            new Company
            {
                Name = "Green Energy Corp",
                Description = "Sustainable energy solutions for a cleaner tomorrow.",
                UserId = owner.Id,
                Address = new Address
                {
                    Street = "456 Renewable Way",
                    City = "Austin",
                    State = "TX",
                    PostalCode = "73301",
                    Country = "USA"
                }
            },
            new Company
            {
                Name = "Global Manufacturing Ltd",
                Description = "International manufacturing and supply chain management.",
                UserId = owner.Id,
                Address = new Address
                {
                    Street = "789 Industrial Park Dr",
                    Suite = "Building B",
                    City = "Detroit",
                    State = "MI",
                    PostalCode = "48201",
                    Country = "USA"
                }
            },
            new Company
            {
                Name = "Creative Designs Studio",
                Description = "Award-winning graphic design and branding agency.",
                UserId = owner.Id,
                Address = new Address
                {
                    Street = "321 Art District Ave",
                    City = "Portland",
                    State = "OR",
                    PostalCode = "97201",
                    Country = "USA"
                }
            },
            new Company
            {
                Name = "Financial Advisors Group",
                Description = "Comprehensive financial planning and investment services.",
                UserId = owner.Id,
                Address = new Address
                {
                    Street = "654 Wall Street",
                    Suite = "Floor 25",
                    City = "New York",
                    State = "NY",
                    PostalCode = "10005",
                    Country = "USA"
                }
            },
            new Company
            {
                Name = "Remote Services Co",
                Description = "Fully remote company providing digital consulting services.",
                UserId = owner.Id
                // No address - to demonstrate companies without addresses
            }
        };

        _dbContext.Companies.AddRange(companies);
        _dbContext.SaveChanges();

        Console.WriteLine($"Successfully seeded {companies.Count} companies with addresses.");
        return 0;
    }
}