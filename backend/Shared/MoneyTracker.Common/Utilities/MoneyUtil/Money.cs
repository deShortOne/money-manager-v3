namespace MoneyTracker.Common.Utilities.MoneyUtil;
public sealed class Money
{
    public decimal Amount { get; }
    private Money(decimal amount)
    {
        Amount = amount;
    }

    public static Money From(decimal amount)
    {
        int numberOfDecimalPlaces = BitConverter.GetBytes(decimal.GetBits(amount)[3])[2];
        if (numberOfDecimalPlaces > 2)
        {
            throw new InvalidDataException($"Money value has too many decimal places: {amount}");
        }

        return new Money(amount);
    }

    public static Money From(string amount)
    {
        if (!decimal.TryParse(amount, out var tmpAmount))
        {
            throw new InvalidDataException($"Money value must be a valid number: {amount}");
        }

        return From(tmpAmount);
    }

    public override bool Equals(object? obj)
    {
        var other = obj as Money;
        if (other == null)
            return false;
        return Amount == other.Amount;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Amount);
    }
}
