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
        if (amount < 0)
        {
            throw new InvalidDataException($"Money value cannot be negative: {amount}");
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

    public static Money operator /(Money money1, decimal divisor)
    {
        return new Money(decimal.Round(money1.Amount / divisor, 2, MidpointRounding.ToNegativeInfinity));
    }

    public static Money operator *(Money money1, decimal multiplier)
    {
        return new Money(money1.Amount * multiplier);
    }

    public static Money operator +(Money money1, Money money2)
    {
        return new Money(money1.Amount + money2.Amount);
    }

    public static Money operator -(Money money1, Money money2)
    {
        return new Money(money1.Amount - money2.Amount);
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
