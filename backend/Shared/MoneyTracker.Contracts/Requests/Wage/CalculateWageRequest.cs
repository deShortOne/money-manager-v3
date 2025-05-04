
namespace MoneyTracker.Contracts.Requests.Wage;
public class CalculateWageRequest(
    decimal grossIncome,
    string frequencyOfIncome,
    string taxCode)
{
    public decimal GrossIncome { get; } = grossIncome;
    public string FrequencyOfIncome { get; } = frequencyOfIncome;
    public string TaxCode { get; } = taxCode;
}
