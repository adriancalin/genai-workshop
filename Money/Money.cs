namespace Money;

/// <summary>
/// Represents a non-negative money amount in a given currency with a specific precision.
/// </summary>
public sealed class Money : IEquatable<Money>
{
    private readonly Currency _currency;
    private readonly decimal _amount;
    private readonly int _precision;

    /// <summary>
    /// Gets the currency of this money.
    /// </summary>
    public Currency Currency => _currency;

    /// <summary>
    /// Gets the amount of this money (non-negative, rounded to precision).
    /// </summary>
    public decimal Amount => _amount;

    /// <summary>
    /// Gets the precision (number of decimal places: 2, 4, or 6).
    /// </summary>
    public int Precision => _precision;

    /// <summary>
    /// Creates a new Money instance.
    /// </summary>
    /// <param name="currency">The currency.</param>
    /// <param name="amount">The non-negative amount.</param>
    /// <param name="precision">The precision (2, 4, or 6 decimal places).</param>
    /// <exception cref="ArgumentNullException">Thrown when currency is null.</exception>
    /// <exception cref="ArgumentException">Thrown when amount is negative or precision is invalid.</exception>
    public Money(Currency currency, decimal amount, int precision)
    {
        _currency = currency ?? throw new ArgumentNullException(nameof(currency));

        if (amount < 0)
            throw new ArgumentException("Amount cannot be negative.", nameof(amount));

        if (precision != 2 && precision != 4 && precision != 6)
            throw new ArgumentException("Precision must be 2, 4, or 6.", nameof(precision));

        _precision = precision;
        _amount = Math.Round(amount, precision);
    }

    /// <summary>
    /// Adds two Moneys with the same currency (strict addition).
    /// </summary>
    /// <param name="other">The money to add.</param>
    /// <returns>A new Money with the sum of amounts and the higher precision.</returns>
    /// <exception cref="ArgumentNullException">Thrown when other is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when currencies don't match.</exception>
    public Money Add(Money other)
    {
        if (other is null)
            throw new ArgumentNullException(nameof(other));

        if (!_currency.Equals(other._currency))
            throw new InvalidOperationException("Cannot add money with different currencies using strict addition.");

        var newPrecision = Math.Max(_precision, other._precision);
        var newAmount = _amount + other._amount;

        return new Money(_currency, newAmount, newPrecision);
    }

    /// <summary>
    /// Adds two general Moneys (relaxed addition), creating a MoneyBag.
    /// </summary>
    /// <param name="other">The money to add.</param>
    /// <returns>A MoneyBag containing both moneys.</returns>
    public MoneyBag AddRelaxed(Money other)
    {
        if (other is null)
            throw new ArgumentNullException(nameof(other));

        return new MoneyBag(this).Add(other);
    }

    /// <summary>
    /// Scales this money by a decimal factor.
    /// </summary>
    /// <param name="factor">The scaling factor.</param>
    /// <returns>A new Money with the scaled amount and same precision.</returns>
    public Money Scale(decimal factor)
    {
        var newAmount = _amount * factor;
        return new Money(_currency, newAmount, _precision);
    }

    /// <summary>
    /// Subtracts another Money from this one.
    /// </summary>
    /// <param name="other">The money to subtract.</param>
    /// <returns>A new Money with the difference.</returns>
    /// <exception cref="ArgumentNullException">Thrown when other is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when currencies don't match or other amount is greater.</exception>
    public Money Subtract(Money other)
    {
        if (other is null)
            throw new ArgumentNullException(nameof(other));

        if (!_currency.Equals(other._currency))
            throw new InvalidOperationException("Cannot subtract money with different currencies.");

        if (other._amount > _amount)
            throw new InvalidOperationException("Cannot subtract a larger amount from a smaller amount.");

        var newPrecision = Math.Max(_precision, other._precision);
        var newAmount = _amount - other._amount;

        return new Money(_currency, newAmount, newPrecision);
    }

    public bool Equals(Money? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return _currency.Equals(other._currency) && _amount == other._amount && _precision == other._precision;
    }

    public override bool Equals(object? obj) => Equals(obj as Money);

    public override int GetHashCode() => HashCode.Combine(_currency, _amount, _precision);

    public override string ToString() 
    {
        var format = $"F{_precision}";
        return $"{_amount.ToString(format, System.Globalization.CultureInfo.InvariantCulture)} {_currency.Code}";
    }

    public static bool operator ==(Money? left, Money? right)
    {
        if (left is null) return right is null;
        return left.Equals(right);
    }

    public static bool operator !=(Money? left, Money? right) => !(left == right);

    public static Money operator +(Money left, Money right) => left.Add(right);

    public static Money operator -(Money left, Money right) => left.Subtract(right);

    public static Money operator *(Money money, decimal factor) => money.Scale(factor);

    public static Money operator *(decimal factor, Money money) => money.Scale(factor);
}
