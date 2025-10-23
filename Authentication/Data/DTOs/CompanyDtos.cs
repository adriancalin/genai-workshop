namespace Authentication.Data.DTOs;

/// <summary>
/// Address data transfer object
/// </summary>
public class AddressDto
{
    public Guid Id { get; set; }
    public Guid? CompanyId { get; set; }
    public string Street { get; set; } = string.Empty;
    public string? Suite { get; set; }
    public string City { get; set; } = string.Empty;
    public string? State { get; set; }
    public string PostalCode { get; set; } = string.Empty;
    public string? Country { get; set; }

    public string FullAddress
    {
        get
        {
            var parts = new List<string>();
            
            if (!string.IsNullOrWhiteSpace(Street))
                parts.Add(Street);
            
            if (!string.IsNullOrWhiteSpace(Suite))
                parts.Add(Suite);
            
            var cityStateZip = new List<string>();
            if (!string.IsNullOrWhiteSpace(City))
                cityStateZip.Add(City);
            if (!string.IsNullOrWhiteSpace(State))
                cityStateZip.Add(State);
            if (!string.IsNullOrWhiteSpace(PostalCode))
                cityStateZip.Add(PostalCode);
            
            if (cityStateZip.Any())
                parts.Add(string.Join(", ", cityStateZip));
            
            if (!string.IsNullOrWhiteSpace(Country))
                parts.Add(Country);
            
            return string.Join("\n", parts);
        }
    }
}

/// <summary>
/// Company data transfer object (basic info)
/// </summary>
public class CompanyDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string? UserName { get; set; }
    public int AddressCount { get; set; }
}

/// <summary>
/// Company data transfer object with full details including address
/// </summary>
public class CompanyDetailsDto : CompanyDto
{
    public IEnumerable<AddressDto> Addresses { get; set; } = new List<AddressDto>();
}

/// <summary>
/// Paginated result wrapper
/// </summary>
/// <typeparam name="T">Item type</typeparam>
public class PaginatedResult<T>
{
    public IEnumerable<T> Items { get; set; } = new List<T>();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
}

/// <summary>
/// DTO for creating a new address
/// </summary>
public class CreateAddressDto
{
    public Guid CompanyId { get; set; }
    public string Street { get; set; } = string.Empty;
    public string? Suite { get; set; }
    public string City { get; set; } = string.Empty;
    public string? State { get; set; }
    public string PostalCode { get; set; } = string.Empty;
    public string? Country { get; set; }
}

/// <summary>
/// DTO for updating an existing address
/// </summary>
public class UpdateAddressDto
{
    public string Street { get; set; } = string.Empty;
    public string? Suite { get; set; }
    public string City { get; set; } = string.Empty;
    public string? State { get; set; }
    public string PostalCode { get; set; } = string.Empty;
    public string? Country { get; set; }
}