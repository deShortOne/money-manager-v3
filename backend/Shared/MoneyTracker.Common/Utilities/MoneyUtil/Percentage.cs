namespace MoneyTracker.Common.Utilities.MoneyUtil;
public class Percentage
{
    public decimal Value { get; }
    private Percentage(decimal percentage)
    {
        Value = percentage;
    }

    public static Percentage From(decimal percentage)
    {
        return new Percentage(percentage / 100);
    }
}
