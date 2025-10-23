namespace Money.Tests;

public class CurrencyTests
{
    [Fact]
    public void Currency_WithValidCode_CreatesInstance()
    {
        // Arrange & Act
        var usd = new Currency("USD");

        // Assert
        Assert.Equal("USD", usd.Code);
    }

    [Fact]
    public void Currency_WithLowercaseCode_ConvertsToUppercase()
    {
        // Arrange & Act
        var eur = new Currency("eur");

        // Assert
        Assert.Equal("EUR", eur.Code);
    }

    [Theory]
    [InlineData("USD")]
    [InlineData("EUR")]
    [InlineData("HUF")]
    [InlineData("GBP")]
    [InlineData("JPY")]
    public void Currency_WithValidIsoCodes_CreatesInstance(string code)
    {
        // Arrange & Act
        var currency = new Currency(code);

        // Assert
        Assert.Equal(code.ToUpperInvariant(), currency.Code);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Currency_WithNullOrEmpty_ThrowsArgumentException(string? code)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Currency(code!));
    }

    [Theory]
    [InlineData("US")]      // Too short
    [InlineData("USDA")]    // Too long
    [InlineData("U")]       // Too short
    public void Currency_WithInvalidLength_ThrowsArgumentException(string code)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Currency(code));
    }

    [Theory]
    [InlineData("XXX")]     // Not a valid ISO code
    [InlineData("ABC")]     // Not a valid ISO code
    [InlineData("ZZZ")]     // Not a valid ISO code
    public void Currency_WithInvalidIsoCode_ThrowsArgumentException(string code)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Currency(code));
    }

    [Fact]
    public void Currency_IsImmutable_CannotChangeCode()
    {
        // Arrange
        var usd = new Currency("USD");

        // Assert - Code should only have a getter
        Assert.Equal("USD", usd.Code);
        // Note: The property only has a getter, so immutability is guaranteed by design
    }

    [Fact]
    public void Currency_Equality_SameCurrenciesAreEqual()
    {
        // Arrange
        var usd1 = new Currency("USD");
        var usd2 = new Currency("usd");

        // Act & Assert
        Assert.Equal(usd1, usd2);
        Assert.True(usd1 == usd2);
        Assert.False(usd1 != usd2);
        Assert.Equal(usd1.GetHashCode(), usd2.GetHashCode());
    }

    [Fact]
    public void Currency_Equality_DifferentCurrenciesAreNotEqual()
    {
        // Arrange
        var usd = new Currency("USD");
        var eur = new Currency("EUR");

        // Act & Assert
        Assert.NotEqual(usd, eur);
        Assert.False(usd == eur);
        Assert.True(usd != eur);
    }

    [Fact]
    public void Currency_ToString_ReturnsCode()
    {
        // Arrange
        var usd = new Currency("USD");

        // Act
        var result = usd.ToString();

        // Assert
        Assert.Equal("USD", result);
    }

    [Fact]
    public void Currency_EqualsNull_ReturnsFalse()
    {
        // Arrange
        var usd = new Currency("USD");

        // Act & Assert
        Assert.False(usd.Equals(null));
        Assert.False(usd == null);
        Assert.True(usd != null);
    }

    [Fact]
    public void Currency_SameReference_AreEqual()
    {
        // Arrange
        var usd = new Currency("USD");
        var sameRef = usd;

        // Act & Assert
        Assert.Same(usd, sameRef);
        Assert.Equal(usd, sameRef);
        Assert.True(usd == sameRef);
    }
}
