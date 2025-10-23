using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Authentication;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Company> Companies { get; set; }
    public DbSet<Address> Addresses { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Configure business entities
        builder.Entity<Company>().ToTable("Companies", "dbo");
        builder.Entity<Address>().ToTable("Addresses", "business");

        builder.Entity<IdentityRole>().HasData(
            new IdentityRole
            {
                Id = "1a2b3c4d-5e6f-7a8b-9c0d-1e2f3a4b5c6d",
                Name = "Owner",
                NormalizedName = "OWNER"
            },
            new IdentityRole
            {
                Id = "2b3c4d5e-6f7a-8b9c-0d1e-2f3a4b5c6d7e",
                Name = "User",
                NormalizedName = "USER"
            }
        );
    }
}
