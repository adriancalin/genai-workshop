using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Authentication;

public class Company
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    // Foreign key to ApplicationUser (owner)
    [Required]
    public string UserId { get; set; } = null!;

    [ForeignKey(nameof(UserId))]
    public ApplicationUser? User { get; set; }

    // One-to-one (optional) relationship: a Company can have a single Address
    public Address? Address { get; set; }

    // Soft delete flag
    public bool Deleted { get; set; } = false;
}