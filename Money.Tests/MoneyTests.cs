namespace Money.Tests;

public class MoneyTests
{
    private readonly Currency _usd = new("USD");
    private readonly Currency _eur = new("EUR");
    private readonly Currency _huf = new("HUF");

    #region Construction Tests

    [Fact]
    public void Money_WithValidParameters_CreatesInstance()
    {
        // Arrange & Act
        var money = new Money(_usd, 100.50m, 2);

        // Assert
        Assert.Equal(_usd, money.Currency);
        Assert.Equal(100.50m, money.Amount);
        Assert.Equal(2, money.Precision);
    }

    [Fact]
    public void Money_WithNullCurrency_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new Money(null!, 100, 2));
    }

    [Fact]
    public void Money_WithNegativeAmount_ThrowsArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Money(_usd, -10, 2));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(3)]
    [InlineData(5)]
    [InlineData(7)]
    [InlineData(10)]
    public void Money_WithInvalidPrecision_ThrowsArgumentException(int precision)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Money(_usd, 100, precision));
    }

    [Theory]
    [InlineData(2)]
    [InlineData(4)]
    [InlineData(6)]
    public void Money_WithValidPrecision_CreatesInstance(int precision)
    {
        // Arrange & Act
        var money = new Money(_usd, 100, precision);

        // Assert
        Assert.Equal(precision, money.Precision);
    }

    [Fact]
    public void Money_RoundsAmountToPrecision()
    {
        // Arrange & Act
        var money2 = new Money(_usd, 100.999m, 2);
        var money4 = new Money(_usd, 100.99999m, 4);
        var money6 = new Money(_usd, 100.9999999m, 6);

        // Assert
        Assert.Equal(101.00m, money2.Amount);
        Assert.Equal(101.0000m, money4.Amount);
        Assert.Equal(101.000000m, money6.Amount);
    }

    #endregion

    #region Strict Addition Tests

    [Fact]
    public void Money_Add_SameCurrencySamePrecision_CreatesSum()
    {
        // Arrange
        var money1 = new Money(_usd, 100.50m, 2);
        var money2 = new Money(_usd, 50.25m, 2);

        // Act
        var result = money1.Add(money2);

        // Assert
        Assert.Equal(_usd, result.Currency);
        Assert.Equal(150.75m, result.Amount);
        Assert.Equal(2, result.Precision);
    }

    [Fact]
    public void Money_Add_SameCurrencyDifferentPrecision_UsesHigherPrecision()
    {
        // Arrange
        var money1 = new Money(_usd, 100.50m, 2);
        var money2 = new Money(_usd, 50.2525m, 4);

        // Act
        var result = money1.Add(money2);

        // Assert
        Assert.Equal(_usd, result.Currency);
        Assert.Equal(150.7525m, result.Amount);
        Assert.Equal(4, result.Precision);
    }

    [Fact]
    public void Money_Add_DifferentCurrency_ThrowsInvalidOperationException()
    {
        // Arrange
        var usdMoney = new Money(_usd, 100, 2);
        var eurMoney = new Money(_eur, 50, 2);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => usdMoney.Add(eurMoney));
    }

    [Fact]
    public void Money_Add_NullMoney_ThrowsArgumentNullException()
    {
        // Arrange
        var money = new Money(_usd, 100, 2);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => money.Add(null!));
    }

    [Fact]
    public void Money_OperatorPlus_WorksLikeAdd()
    {
        // Arrange
        var money1 = new Money(_usd, 100.50m, 2);
        var money2 = new Money(_usd, 50.25m, 2);

        // Act
        var result = money1 + money2;

        // Assert
        Assert.Equal(150.75m, result.Amount);
    }

    #endregion

    #region Relaxed Addition Tests

    [Fact]
    public void Money_AddRelaxed_SameCurrency_CreatesMoneyBagWithSingleCurrency()
    {
        // Arrange
        var money1 = new Money(_usd, 100, 2);
        var money2 = new Money(_usd, 50, 2);

        // Act
        var bag = money1.AddRelaxed(money2);

        // Assert
        Assert.Single(bag.Moneys);
        Assert.Equal(150m, bag.Moneys.First().Amount);
    }

    [Fact]
    public void Money_AddRelaxed_DifferentCurrency_CreatesMoneyBagWithMultipleCurrencies()
    {
        // Arrange
        var usdMoney = new Money(_usd, 100, 2);
        var eurMoney = new Money(_eur, 50, 2);

        // Act
        var bag = usdMoney.AddRelaxed(eurMoney);

        // Assert
        Assert.Equal(2, bag.Count);
        Assert.Contains(bag.Moneys, m => m.Currency == _usd);
        Assert.Contains(bag.Moneys, m => m.Currency == _eur);
    }

    #endregion

    #region Scaling Tests

    [Fact]
    public void Money_Scale_MultipliesAmount()
    {
        // Arrange
        var money = new Money(_usd, 100, 2);

        // Act
        var result = money.Scale(1.5m);

        // Assert
        Assert.Equal(_usd, result.Currency);
        Assert.Equal(150m, result.Amount);
        Assert.Equal(2, result.Precision);
    }

    [Fact]
    public void Money_Scale_RoundsToOriginalPrecision()
    {
        // Arrange
        var money = new Money(_usd, 100.33m, 2);

        // Act
        var result = money.Scale(1.5m);

        // Assert
        Assert.Equal(150.50m, result.Amount); // 100.33 * 1.5 = 150.495 rounded to 150.50
        Assert.Equal(2, result.Precision);
    }

    [Fact]
    public void Money_OperatorMultiply_WorksLikeScale()
    {
        // Arrange
        var money = new Money(_usd, 100, 2);

        // Act
        var result1 = money * 2m;
        var result2 = 2m * money;

        // Assert
        Assert.Equal(200m, result1.Amount);
        Assert.Equal(200m, result2.Amount);
    }

    #endregion

    #region Subtraction Tests

    [Fact]
    public void Money_Subtract_SameCurrency_CreatesResult()
    {
        // Arrange
        var money1 = new Money(_usd, 100.50m, 2);
        var money2 = new Money(_usd, 30.25m, 2);

        // Act
        var result = money1.Subtract(money2);

        // Assert
        Assert.Equal(_usd, result.Currency);
        Assert.Equal(70.25m, result.Amount);
        Assert.Equal(2, result.Precision);
    }

    [Fact]
    public void Money_Subtract_DifferentPrecision_UsesHigherPrecision()
    {
        // Arrange
        var money1 = new Money(_usd, 100.5000m, 4);
        var money2 = new Money(_usd, 30.25m, 2);

        // Act
        var result = money1.Subtract(money2);

        // Assert
        Assert.Equal(70.25m, result.Amount);
        Assert.Equal(4, result.Precision);
    }

    [Fact]
    public void Money_Subtract_DifferentCurrency_ThrowsInvalidOperationException()
    {
        // Arrange
        var usdMoney = new Money(_usd, 100, 2);
        var eurMoney = new Money(_eur, 50, 2);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => usdMoney.Subtract(eurMoney));
    }

    [Fact]
    public void Money_Subtract_LargerAmount_ThrowsInvalidOperationException()
    {
        // Arrange
        var money1 = new Money(_usd, 100, 2);
        var money2 = new Money(_usd, 150, 2);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => money1.Subtract(money2));
    }

    [Fact]
    public void Money_Subtract_NullMoney_ThrowsArgumentNullException()
    {
        // Arrange
        var money = new Money(_usd, 100, 2);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => money.Subtract(null!));
    }

    [Fact]
    public void Money_OperatorMinus_WorksLikeSubtract()
    {
        // Arrange
        var money1 = new Money(_usd, 100.50m, 2);
        var money2 = new Money(_usd, 30.25m, 2);

        // Act
        var result = money1 - money2;

        // Assert
        Assert.Equal(70.25m, result.Amount);
    }

    #endregion

    #region Equality Tests

    [Fact]
    public void Money_Equality_SameValues_AreEqual()
    {
        // Arrange
        var money1 = new Money(_usd, 100.50m, 2);
        var money2 = new Money(_usd, 100.50m, 2);

        // Act & Assert
        Assert.Equal(money1, money2);
        Assert.True(money1 == money2);
        Assert.False(money1 != money2);
        Assert.Equal(money1.GetHashCode(), money2.GetHashCode());
    }

    [Fact]
    public void Money_Equality_DifferentAmounts_AreNotEqual()
    {
        // Arrange
        var money1 = new Money(_usd, 100.50m, 2);
        var money2 = new Money(_usd, 100.51m, 2);

        // Act & Assert
        Assert.NotEqual(money1, money2);
        Assert.False(money1 == money2);
        Assert.True(money1 != money2);
    }

    [Fact]
    public void Money_Equality_DifferentCurrencies_AreNotEqual()
    {
        // Arrange
        var usdMoney = new Money(_usd, 100, 2);
        var eurMoney = new Money(_eur, 100, 2);

        // Act & Assert
        Assert.NotEqual(usdMoney, eurMoney);
    }

    [Fact]
    public void Money_Equality_DifferentPrecisions_AreNotEqual()
    {
        // Arrange
        var money1 = new Money(_usd, 100m, 2);
        var money2 = new Money(_usd, 100m, 4);

        // Act & Assert
        Assert.NotEqual(money1, money2);
    }

    #endregion

    #region ToString Tests

    [Fact]
    public void Money_ToString_FormatsCorrectly()
    {
        // Arrange
        var money2 = new Money(_usd, 100.5m, 2);
        var money4 = new Money(_eur, 100.5m, 4);
        var money6 = new Money(_huf, 100.5m, 6);

        // Act & Assert
        Assert.Equal("100.50 USD", money2.ToString());
        Assert.Equal("100.5000 EUR", money4.ToString());
        Assert.Equal("100.500000 HUF", money6.ToString());
    }

    #endregion
}
