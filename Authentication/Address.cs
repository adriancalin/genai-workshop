using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Authentication;

public class Address
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    // Foreign key to Company (optional)
    public Guid? CompanyId { get; set; }

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