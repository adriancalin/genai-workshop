namespace Money;

/// <summary>
/// Represents a currency according to ISO 4217 specification (ISO-3 currency symbols).
/// This is an immutable type.
/// </summary>
public sealed class Currency : IEquatable<Currency>
{
    // Common ISO 4217 currency codes for validation
    private static readonly HashSet<string> ValidCurrencyCodes = new(StringComparer.OrdinalIgnoreCase)
    {
        "AED", "AFN", "ALL", "AMD", "ANG", "AOA", "ARS", "AUD", "AWG", "AZN",
        "BAM", "BBD", "BDT", "BGN", "BHD", "BIF", "BMD", "BND", "BOB", "BRL",
        "BSD", "BTN", "BWP", "BYN", "BZD", "CAD", "CDF", "CHF", "CLP", "CNY",
        "COP", "CRC", "CUP", "CVE", "CZK", "DJF", "DKK", "DOP", "DZD", "EGP",
        "ERN", "ETB", "EUR", "FJD", "FKP", "GBP", "GEL", "GGP", "GHS", "GIP",
        "GMD", "GNF", "GTQ", "GYD", "HKD", "HNL", "HRK", "HTG", "HUF", "IDR",
        "ILS", "IMP", "INR", "IQD", "IRR", "ISK", "JEP", "JMD", "JOD", "JPY",
        "KES", "KGS", "KHR", "KMF", "KPW", "KRW", "KWD", "KYD", "KZT", "LAK",
        "LBP", "LKR", "LRD", "LSL", "LYD", "MAD", "MDL", "MGA", "MKD", "MMK",
        "MNT", "MOP", "MRU", "MUR", "MVR", "MWK", "MXN", "MYR", "MZN", "NAD",
        "NGN", "NIO", "NOK", "NPR", "NZD", "OMR", "PAB", "PEN", "PGK", "PHP",
        "PKR", "PLN", "PYG", "QAR", "RON", "RSD", "RUB", "RWF", "SAR", "SBD",
        "SCR", "SDG", "SEK", "SGD", "SHP", "SLL", "SOS", "SPL", "SRD", "STN",
        "SVC", "SYP", "SZL", "THB", "TJS", "TMT", "TND", "TOP", "TRY", "TTD",
        "TVD", "TWD", "TZS", "UAH", "UGX", "USD", "UYU", "UZS", "VEF", "VND",
        "VUV", "WST", "XAF", "XCD", "XDR", "XOF", "XPF", "YER", "ZAR", "ZMW", "ZWD"
    };

    private readonly string _code;

    /// <summary>
    /// Gets the ISO 4217 currency code (e.g., USD, EUR, HUF).
    /// </summary>
    public string Code => _code;

    /// <summary>
    /// Creates a new Currency instance with the specified ISO 4217 code.
    /// </summary>
    /// <param name="code">The ISO 4217 currency code (3 uppercase letters).</param>
    /// <exception cref="ArgumentException">Thrown when the currency code is invalid.</exception>
    public Currency(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Currency code cannot be null or empty.", nameof(code));

        var upperCode = code.ToUpperInvariant();

        if (upperCode.Length != 3)
            throw new ArgumentException("Currency code must be exactly 3 characters.", nameof(code));

        if (!ValidCurrencyCodes.Contains(upperCode))
            throw new ArgumentException($"'{upperCode}' is not a valid ISO 4217 currency code.", nameof(code));

        _code = upperCode;
    }

    public bool Equals(Currency? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return _code == other._code;
    }

    public override bool Equals(object? obj) => Equals(obj as Currency);

    public override int GetHashCode() => _code.GetHashCode();

    public override string ToString() => _code;

    public static bool operator ==(Currency? left, Currency? right)
    {
        if (left is null) return right is null;
        return left.Equals(right);
    }

    public static bool operator !=(Currency? left, Currency? right) => !(left == right);
}
