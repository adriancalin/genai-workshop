using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Authentication;

public class Company
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    // Foreign key to ApplicationUser (owner)
    [Required]
    public string UserId { get; set; } = null!;

    [ForeignKey(nameof(UserId))]
    public ApplicationUser? User { get; set; }

    // One-to-many relationship: a Company can have multiple Addresses
    public ICollection<Address> Addresses { get; set; } = new List<Address>();

    // Soft delete flag
    public bool Deleted { get; set; } = false;
}