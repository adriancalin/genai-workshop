using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Authentication;

namespace Web.Models;

public class Address
{
    [Key]
    public int Id { get; set; }

    // Foreign key to Company (optional)
    public int? CompanyId { get; set; }

    [ForeignKey(nameof(CompanyId))]
    public Company? Company { get; set; }

    [Required]
    public string Street { get; set; } = null!;

    public string? Suite { get; set; }

    [Required]
    public string City { get; set; } = null!;

    public string? State { get; set; }

    [Required]
    public string PostalCode { get; set; } = null!;

    public string? Country { get; set; }
}