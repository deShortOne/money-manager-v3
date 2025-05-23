using System;
using MoneyTracker.Contracts.Requests.Wage.PensionCalculator;

namespace MoneyTracker.Contracts.Requests.Wage;
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

public class StudentLoanOptions(bool plan1, bool plan2, bool plan4, bool plan5, bool postGraduate)
{
    public bool Plan1 { get; } = plan1;
    public bool Plan2 { get; } = plan2;
    public bool Plan4 { get; } = plan4;
    public bool Plan5 { get; } = plan5;
    public bool PostGraduate { get; } = postGraduate;
}

public class Pension(IPensionCalculator calculator, PensionType type)
{
    public IPensionCalculator Calculator { get; } = calculator;
    public PensionType Type { get; } = type;
}

public enum PensionType
{
    Unknown = 0,
    AutoEnrolment = 1,
    Personal = 2,
}
