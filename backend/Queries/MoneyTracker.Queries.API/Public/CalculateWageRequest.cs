using MoneyTracker.Contracts.Requests.Wage;

namespace MoneyTracker.Queries.API.Public;

public class CalculateWageRequest(
    decimal grossIncome,
    string frequencyOfIncome,
    string taxCode,
    bool payNationalInsurance,
    Pension pension,
    StudentLoanOptions studentLoanOptions)
{
    public decimal GrossIncome { get; } = grossIncome;
    public string FrequencyOfIncome { get; } = frequencyOfIncome;
    public string TaxCode { get; } = taxCode;
    public bool PayNationalInsurance { get; } = payNationalInsurance;
    public Pension Pension { get; } = pension;
    public StudentLoanOptions StudentLoanOptions { get; } = studentLoanOptions;
}

public class Pension(string pensionType, decimal value, string pensionCalculationType)
{
    public string PensionType { get; } = pensionType;
    public decimal Value { get; } = value;
    public string PensionCalculationType { get; } = pensionCalculationType;
}

public enum PensionCalculationType
{
    Unknown = 0,
    Amount = 1,
    Percentage = 2,
}
