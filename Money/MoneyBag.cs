using System.Collections;

namespace Money;

/// <summary>
/// Represents a collection of Money instances in different currencies.
/// Useful in accounting scenarios where multiple currencies need to be tracked.
/// </summary>
public sealed class MoneyBag : IEnumerable<Money>
{
    private readonly Dictionary<Currency, Money> _moneys;

    /// <summary>
    /// Gets the sequence of Moneys in this bag.
    /// </summary>
    public IEnumerable<Money> Moneys => _moneys.Values;

    /// <summary>
    /// Gets the number of different currencies in this bag.
    /// </summary>
    public int Count => _moneys.Count;

    /// <summary>
    /// Creates a new MoneyBag with a single Money instance.
    /// </summary>
    /// <param name="money">The initial money.</param>
    /// <exception cref="ArgumentNullException">Thrown when money is null.</exception>
    public MoneyBag(Money money)
    {
        if (money is null)
            throw new ArgumentNullException(nameof(money));

        _moneys = new Dictionary<Currency, Money>
        {
            { money.Currency, money }
        };
    }

    /// <summary>
    /// Creates a new MoneyBag from a collection of Money instances.
    /// </summary>
    /// <param name="moneys">The collection of moneys.</param>
    /// <exception cref="ArgumentNullException">Thrown when moneys is null.</exception>
    /// <exception cref="ArgumentException">Thrown when moneys is empty.</exception>
    private MoneyBag(Dictionary<Currency, Money> moneys)
    {
        if (moneys is null || moneys.Count == 0)
            throw new ArgumentException("MoneyBag must contain at least one Money.", nameof(moneys));

        _moneys = new Dictionary<Currency, Money>(moneys);
    }

    /// <summary>
    /// Adds new Money to this bag.
    /// If the currency exists, sums them using strict addition rules.
    /// If the currency is new, adds it to the collection.
    /// </summary>
    /// <param name="money">The money to add.</param>
    /// <returns>A new MoneyBag with the added money.</returns>
    /// <exception cref="ArgumentNullException">Thrown when money is null.</exception>
    public MoneyBag Add(Money money)
    {
        if (money is null)
            throw new ArgumentNullException(nameof(money));

        var newMoneys = new Dictionary<Currency, Money>(_moneys);

        if (newMoneys.ContainsKey(money.Currency))
        {
            // Currency exists - use strict addition (same currency)
            var existing = newMoneys[money.Currency];
            newMoneys[money.Currency] = existing.Add(money);
        }
        else
        {
            // New currency - add to collection
            newMoneys[money.Currency] = money;
        }

        return new MoneyBag(newMoneys);
    }

    /// <summary>
    /// Adds another MoneyBag to this one.
    /// </summary>
    /// <param name="other">The MoneyBag to add.</param>
    /// <returns>A new MoneyBag with all moneys combined.</returns>
    /// <exception cref="ArgumentNullException">Thrown when other is null.</exception>
    public MoneyBag Add(MoneyBag other)
    {
        if (other is null)
            throw new ArgumentNullException(nameof(other));

        var result = this;
        foreach (var money in other.Moneys)
        {
            result = result.Add(money);
        }

        return result;
    }

    /// <summary>
    /// Tries to fold this MoneyBag into a single Money instance.
    /// Only possible if the bag contains a single currency.
    /// </summary>
    /// <returns>The single Money if successful.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the bag contains multiple currencies.</exception>
    public Money Fold()
    {
        if (_moneys.Count != 1)
            throw new InvalidOperationException("Cannot fold a MoneyBag containing multiple currencies.");

        return _moneys.Values.First();
    }

    /// <summary>
    /// Tries to fold this MoneyBag into a single Money instance.
    /// </summary>
    /// <param name="money">The folded money if successful.</param>
    /// <returns>True if folding was successful, false otherwise.</returns>
    public bool TryFold(out Money? money)
    {
        if (_moneys.Count == 1)
        {
            money = _moneys.Values.First();
            return true;
        }

        money = null;
        return false;
    }

    /// <summary>
    /// Checks if this bag contains a specific currency.
    /// </summary>
    /// <param name="currency">The currency to check.</param>
    /// <returns>True if the currency exists in the bag.</returns>
    public bool ContainsCurrency(Currency currency)
    {
        return _moneys.ContainsKey(currency);
    }

    /// <summary>
    /// Gets the Money for a specific currency, if it exists.
    /// </summary>
    /// <param name="currency">The currency to get.</param>
    /// <param name="money">The money if found.</param>
    /// <returns>True if the currency was found.</returns>
    public bool TryGetMoney(Currency currency, out Money? money)
    {
        return _moneys.TryGetValue(currency, out money);
    }

    public IEnumerator<Money> GetEnumerator() => _moneys.Values.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public override string ToString()
    {
        return $"MoneyBag[{string.Join(", ", _moneys.Values.Select(m => m.ToString()))}]";
    }

    public static MoneyBag operator +(MoneyBag left, Money right) => left.Add(right);

    public static MoneyBag operator +(MoneyBag left, MoneyBag right) => left.Add(right);
}
