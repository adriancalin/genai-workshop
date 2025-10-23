namespace Money.Tests;

public class MoneyBagTests
{
    private readonly Currency _usd = new("USD");
    private readonly Currency _eur = new("EUR");
    private readonly Currency _huf = new("HUF");

    #region Construction Tests

    [Fact]
    public void MoneyBag_WithSingleMoney_CreatesInstance()
    {
        // Arrange
        var money = new Money(_usd, 100, 2);

        // Act
        var bag = new MoneyBag(money);

        // Assert
        Assert.Single(bag.Moneys);
        Assert.Equal(money, bag.Moneys.First());
        Assert.Equal(1, bag.Count);
    }

    [Fact]
    public void MoneyBag_WithNullMoney_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new MoneyBag(null!));
    }

    [Fact]
    public void MoneyBag_AlwaysContainsAtLeastOneMoney()
    {
        // Arrange
        var money = new Money(_usd, 100, 2);
        var bag = new MoneyBag(money);

        // Assert
        Assert.True(bag.Count >= 1);
        Assert.NotEmpty(bag.Moneys);
    }

    #endregion

    #region Add Money Tests

    [Fact]
    public void MoneyBag_AddMoneyWithNewCurrency_AddsToCollection()
    {
        // Arrange
        var usdMoney = new Money(_usd, 100, 2);
        var eurMoney = new Money(_eur, 50, 2);
        var bag = new MoneyBag(usdMoney);

        // Act
        var result = bag.Add(eurMoney);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains(result.Moneys, m => m.Currency == _usd);
        Assert.Contains(result.Moneys, m => m.Currency == _eur);
    }

    [Fact]
    public void MoneyBag_AddMoneyWithExistingCurrency_SumsAmounts()
    {
        // Arrange
        var money1 = new Money(_usd, 100, 2);
        var money2 = new Money(_usd, 50, 2);
        var bag = new MoneyBag(money1);

        // Act
        var result = bag.Add(money2);

        // Assert
        Assert.Single(result.Moneys);
        var usdMoney = result.Moneys.First(m => m.Currency == _usd);
        Assert.Equal(150m, usdMoney.Amount);
    }

    [Fact]
    public void MoneyBag_AddMoneyWithExistingCurrency_ObservesStrictAdditionRules()
    {
        // Arrange
        var money1 = new Money(_usd, 100.50m, 2);
        var money2 = new Money(_usd, 50.5555m, 4);
        var bag = new MoneyBag(money1);

        // Act
        var result = bag.Add(money2);

        // Assert
        Assert.Single(result.Moneys);
        var usdMoney = result.Moneys.First(m => m.Currency == _usd);
        Assert.Equal(151.0555m, usdMoney.Amount);
        Assert.Equal(4, usdMoney.Precision); // Higher precision
    }

    [Fact]
    public void MoneyBag_OnlyOneInstancePerCurrency()
    {
        // Arrange
        var money1 = new Money(_usd, 100, 2);
        var money2 = new Money(_usd, 50, 2);
        var money3 = new Money(_usd, 25, 2);
        var bag = new MoneyBag(money1);

        // Act
        var result = bag.Add(money2).Add(money3);

        // Assert
        Assert.Single(result.Moneys);
        Assert.Equal(175m, result.Moneys.First().Amount);
    }

    [Fact]
    public void MoneyBag_AddNullMoney_ThrowsArgumentNullException()
    {
        // Arrange
        var bag = new MoneyBag(new Money(_usd, 100, 2));

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => bag.Add((Money)null!));
    }

    [Fact]
    public void MoneyBag_OperatorPlus_WorksLikeAddMoney()
    {
        // Arrange
        var usdMoney = new Money(_usd, 100, 2);
        var eurMoney = new Money(_eur, 50, 2);
        var bag = new MoneyBag(usdMoney);

        // Act
        var result = bag + eurMoney;

        // Assert
        Assert.Equal(2, result.Count);
    }

    #endregion

    #region Add MoneyBag Tests

    [Fact]
    public void MoneyBag_AddMoneyBag_CombinesAllMoneys()
    {
        // Arrange
        var bag1 = new MoneyBag(new Money(_usd, 100, 2));
        var bag2 = new MoneyBag(new Money(_eur, 50, 2));

        // Act
        var result = bag1.Add(bag2);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains(result.Moneys, m => m.Currency == _usd);
        Assert.Contains(result.Moneys, m => m.Currency == _eur);
    }

    [Fact]
    public void MoneyBag_AddMoneyBag_WithOverlappingCurrencies_SumsAmounts()
    {
        // Arrange
        var bag1 = new MoneyBag(new Money(_usd, 100, 2))
            .Add(new Money(_eur, 50, 2));
        var bag2 = new MoneyBag(new Money(_usd, 25, 2))
            .Add(new Money(_huf, 1000, 2));

        // Act
        var result = bag1.Add(bag2);

        // Assert
        Assert.Equal(3, result.Count);
        var usdMoney = result.Moneys.First(m => m.Currency == _usd);
        Assert.Equal(125m, usdMoney.Amount);
    }

    [Fact]
    public void MoneyBag_AddNullMoneyBag_ThrowsArgumentNullException()
    {
        // Arrange
        var bag = new MoneyBag(new Money(_usd, 100, 2));

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => bag.Add((MoneyBag)null!));
    }

    [Fact]
    public void MoneyBag_OperatorPlus_WorksLikeAddMoneyBag()
    {
        // Arrange
        var bag1 = new MoneyBag(new Money(_usd, 100, 2));
        var bag2 = new MoneyBag(new Money(_eur, 50, 2));

        // Act
        var result = bag1 + bag2;

        // Assert
        Assert.Equal(2, result.Count);
    }

    #endregion

    #region Fold Tests

    [Fact]
    public void MoneyBag_Fold_WithSingleCurrency_ReturnsMoneyInstance()
    {
        // Arrange
        var money = new Money(_usd, 100, 2);
        var bag = new MoneyBag(money);

        // Act
        var result = bag.Fold();

        // Assert
        Assert.Equal(money, result);
    }

    [Fact]
    public void MoneyBag_Fold_WithMultipleCurrencies_ThrowsInvalidOperationException()
    {
        // Arrange
        var bag = new MoneyBag(new Money(_usd, 100, 2))
            .Add(new Money(_eur, 50, 2));

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => bag.Fold());
    }

    [Fact]
    public void MoneyBag_TryFold_WithSingleCurrency_ReturnsTrue()
    {
        // Arrange
        var money = new Money(_usd, 100, 2);
        var bag = new MoneyBag(money);

        // Act
        var success = bag.TryFold(out var result);

        // Assert
        Assert.True(success);
        Assert.Equal(money, result);
    }

    [Fact]
    public void MoneyBag_TryFold_WithMultipleCurrencies_ReturnsFalse()
    {
        // Arrange
        var bag = new MoneyBag(new Money(_usd, 100, 2))
            .Add(new Money(_eur, 50, 2));

        // Act
        var success = bag.TryFold(out var result);

        // Assert
        Assert.False(success);
        Assert.Null(result);
    }

    [Fact]
    public void MoneyBag_Fold_AfterAddingSameCurrency_ReturnsCombinedMoney()
    {
        // Arrange
        var bag = new MoneyBag(new Money(_usd, 100, 2))
            .Add(new Money(_usd, 50, 2));

        // Act
        var result = bag.Fold();

        // Assert
        Assert.Equal(_usd, result.Currency);
        Assert.Equal(150m, result.Amount);
    }

    #endregion

    #region Helper Methods Tests

    [Fact]
    public void MoneyBag_ContainsCurrency_WithExistingCurrency_ReturnsTrue()
    {
        // Arrange
        var bag = new MoneyBag(new Money(_usd, 100, 2));

        // Act & Assert
        Assert.True(bag.ContainsCurrency(_usd));
    }

    [Fact]
    public void MoneyBag_ContainsCurrency_WithNonExistingCurrency_ReturnsFalse()
    {
        // Arrange
        var bag = new MoneyBag(new Money(_usd, 100, 2));

        // Act & Assert
        Assert.False(bag.ContainsCurrency(_eur));
    }

    [Fact]
    public void MoneyBag_TryGetMoney_WithExistingCurrency_ReturnsTrue()
    {
        // Arrange
        var money = new Money(_usd, 100, 2);
        var bag = new MoneyBag(money);

        // Act
        var success = bag.TryGetMoney(_usd, out var result);

        // Assert
        Assert.True(success);
        Assert.Equal(money, result);
    }

    [Fact]
    public void MoneyBag_TryGetMoney_WithNonExistingCurrency_ReturnsFalse()
    {
        // Arrange
        var bag = new MoneyBag(new Money(_usd, 100, 2));

        // Act
        var success = bag.TryGetMoney(_eur, out var result);

        // Assert
        Assert.False(success);
        Assert.Null(result);
    }

    #endregion

    #region Enumeration Tests

    [Fact]
    public void MoneyBag_IsEnumerable_CanIterateOverMoneys()
    {
        // Arrange
        var bag = new MoneyBag(new Money(_usd, 100, 2))
            .Add(new Money(_eur, 50, 2))
            .Add(new Money(_huf, 1000, 2));

        // Act
        var currencies = new List<Currency>();
        foreach (var money in bag)
        {
            currencies.Add(money.Currency);
        }

        // Assert
        Assert.Equal(3, currencies.Count);
        Assert.Contains(_usd, currencies);
        Assert.Contains(_eur, currencies);
        Assert.Contains(_huf, currencies);
    }

    [Fact]
    public void MoneyBag_MoneysProperty_ExposesSequence()
    {
        // Arrange
        var bag = new MoneyBag(new Money(_usd, 100, 2))
            .Add(new Money(_eur, 50, 2));

        // Act
        var moneys = bag.Moneys.ToList();

        // Assert
        Assert.Equal(2, moneys.Count);
    }

    #endregion

    #region ToString Tests

    [Fact]
    public void MoneyBag_ToString_FormatsAllMoneys()
    {
        // Arrange
        var bag = new MoneyBag(new Money(_usd, 100, 2))
            .Add(new Money(_eur, 50.50m, 2));

        // Act
        var result = bag.ToString();

        // Assert
        Assert.Contains("100.00 USD", result);
        Assert.Contains("50.50 EUR", result);
        Assert.Contains("MoneyBag", result);
    }

    #endregion

    #region Immutability Tests

    [Fact]
    public void MoneyBag_AddOperation_DoesNotModifyOriginal()
    {
        // Arrange
        var original = new MoneyBag(new Money(_usd, 100, 2));
        var originalCount = original.Count;

        // Act
        var modified = original.Add(new Money(_eur, 50, 2));

        // Assert
        Assert.Equal(originalCount, original.Count);
        Assert.Equal(2, modified.Count);
        Assert.NotSame(original, modified);
    }

    #endregion
}
