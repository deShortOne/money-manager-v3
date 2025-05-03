using MoneyTracker.Common.Utilities.MoneyUtil;

namespace MoneyTracker.Tests.MoneyTests;
public sealed class GivenAStringAmountIsPassedIn
{
    public static TheoryData<string> ValidRequests = new()
    {
        { "15" },
        { "35.4" },
        { "54.99" },
        { "81.05" },
        { "59.00" },
    };

    [Theory, MemberData(nameof(ValidRequests))]
    public void WhenAValidMoneyStringIsPassedIn(string validIncome)
    {
        // Given
        var actual = Money.From(validIncome);
        // Then
        Assert.Equal(Money.From(validIncome), actual);
    }

    public static TheoryData<string> InvalidRequestsWithInvalidNumbers = new()
    {
        { "£15" },
        { "eighteen" },
    };

    [Theory, MemberData(nameof(InvalidRequestsWithInvalidNumbers))]
    public void WhenAnInvalidMoneyStringIsPassedIn(string invalidIncome)
    {
        // Given
        var errorMessage = Assert.Throws<InvalidDataException>(() =>
        {
            Money.From(invalidIncome);
        });
        // Then
        Assert.Equal($"Money value must be a valid number: {invalidIncome}", errorMessage.Message);
    }

    public static TheoryData<string> InvalidRequestsWithTooManyDecimals = new()
    {
        { "15.245" },
        { "23.000" },
    };

    [Theory, MemberData(nameof(InvalidRequestsWithTooManyDecimals))]
    public void WhenAnInvalidMoneyStringWithTooManyDecimalsIsPassedIn(string invalidIncome)
    {
        // Given
        var errorMessage = Assert.Throws<InvalidDataException>(() =>
        {
            Money.From(invalidIncome);
        });
        // Then
        Assert.Equal($"Money value has too many decimal places: {invalidIncome}", errorMessage.Message);
    }
}
