using MoneyTracker.Common.Utilities.MoneyUtil;

namespace MoneyTracker.Tests.MoneyTests;
public sealed class GivenADecimalAmountIsPassedIn
{
    public static TheoryData<decimal> ValidRequests = new()
    {
        { 15m },
        { 35.4m },
        { 54.99m },
        { 81.05m },
        { 59.00m },
    };

    [Theory, MemberData(nameof(ValidRequests))]
    public void WhenAValidMoneyDecimalIsPassedIn(decimal validIncome)
    {
        // Given
        var actual = Money.From(validIncome);
        // Then
        Assert.Equal(Money.From(validIncome), actual);
    }

    public static TheoryData<decimal> InvalidRequests = new()
    {
        { 15.245m },
        { 23.000m },
    };

    [Theory, MemberData(nameof(InvalidRequests))]
    public void WhenAnInvalidMoneyDecimalIsPassedIn(decimal invalidIncome)
    {
        // Given
        var errorMessage = Assert.Throws<InvalidDataException>(() =>
        {
            Money.From(invalidIncome);
        });
        // Then
        Assert.Equal($"Money value has too many decimal places: {invalidIncome}", errorMessage.Message);
    }

    [Fact]
    public void WhenANegativeAmountIsPassedIn()
    {
        // Given
        var errorMessage = Assert.Throws<InvalidDataException>(() =>
        {
            Money.From(-25);
        });
        // Then
        Assert.Equal($"Money value cannot be negative: -25", errorMessage.Message);
    }
}
