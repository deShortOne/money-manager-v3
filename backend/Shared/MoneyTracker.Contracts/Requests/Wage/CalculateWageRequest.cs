
namespace MoneyTracker.Contracts.Requests.Wage;
public class CalculateWageRequest(
    decimal grossIncome,
    string frequencyOfIncome)
{
    public decimal GrossIncome { get; } = grossIncome;
    public string FrequencyOfIncome { get; } = frequencyOfIncome;
}
