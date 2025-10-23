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
        
        // Get all users
        var users = _dbContext.Users.ToList();
        if (users.Count == 0)
        {
            Console.WriteLine("Error: No users found. Please run 'resetdb' first.");
            return 1;
        }
        
        // Clear existing data
        _dbContext.Addresses.RemoveRange(_dbContext.Addresses);
        _dbContext.Companies.RemoveRange(_dbContext.Companies);
        _dbContext.SaveChanges();

        var allCompanies = new List<Company>();
        var companyNames = new[]
        {
            "Tech Solutions Inc.", "Green Energy Corp", "Global Manufacturing Ltd", "Creative Designs Studio",
            "Financial Advisors Group", "Healthcare Innovations", "Retail Dynamics LLC", "Logistics Express",
            "Digital Marketing Pro", "Cloud Services Global"
        };

        var descriptions = new[]
        {
            "Leading provider of innovative technology solutions for modern businesses.",
            "Sustainable energy solutions for a cleaner tomorrow.",
            "International manufacturing and supply chain management.",
            "Award-winning graphic design and branding agency.",
            "Comprehensive financial planning and investment services.",
            "Revolutionary healthcare technology and patient care solutions.",
            "Modern retail management and customer experience solutions.",
            "Efficient logistics and transportation services worldwide.",
            "Full-service digital marketing and social media management.",
            "Enterprise cloud infrastructure and platform services."
        };

        var streets = new[]
        {
            "123 Silicon Valley Blvd", "456 Renewable Way", "789 Industrial Park Dr", "321 Art District Ave",
            "654 Wall Street", "555 Medical Center Dr", "888 Commerce Blvd", "777 Distribution Way",
            "999 Digital Plaza", "111 Cloud Tower"
        };

        var cities = new[]
        {
            new { City = "San Francisco", State = "CA", Zip = "94105" },
            new { City = "Austin", State = "TX", Zip = "73301" },
            new { City = "Detroit", State = "MI", Zip = "48201" },
            new { City = "Portland", State = "OR", Zip = "97201" },
            new { City = "New York", State = "NY", Zip = "10005" },
            new { City = "Boston", State = "MA", Zip = "02108" },
            new { City = "Seattle", State = "WA", Zip = "98101" },
            new { City = "Denver", State = "CO", Zip = "80202" },
            new { City = "Miami", State = "FL", Zip = "33131" },
            new { City = "Chicago", State = "IL", Zip = "60601" }
        };

        // Create 10 companies for each user
        foreach (var user in users)
        {
            for (int i = 0; i < 10; i++)
            {
                var location = cities[i];
                var company = new Company
                {
                    Name = $"{companyNames[i]} - {user.UserName}",
                    Description = descriptions[i],
                    UserId = user.Id,
                    Addresses = new List<Address>()
                };

                // Add 1-3 addresses per company (vary by index)
                int addressCount = (i % 3) + 1;
                for (int a = 0; a < addressCount; a++)
                {
                    var addressLocation = cities[(i + a) % cities.Length];
                    company.Addresses.Add(new Address
                    {
                        Street = $"{streets[i]} {(a > 0 ? $"Branch {a}" : "")}".Trim(),
                        Suite = a == 0 ? $"Suite {(i + 1) * 100}" : null,
                        City = addressLocation.City,
                        State = addressLocation.State,
                        PostalCode = addressLocation.Zip,
                        Country = "USA"
                    });
                }

                allCompanies.Add(company);
            }
        }

        _dbContext.Companies.AddRange(allCompanies);
        _dbContext.SaveChanges();

        Console.WriteLine($"Successfully seeded {allCompanies.Count} companies with addresses for {users.Count} users.");
        Console.WriteLine($"Each user has 10 companies with varying numbers of addresses.");
        return 0;
    }
}