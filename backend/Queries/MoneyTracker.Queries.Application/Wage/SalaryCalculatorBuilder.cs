using MoneyTracker.Common.Utilities.MoneyUtil;
using MoneyTracker.Contracts.Requests.Wage;

namespace MoneyTracker.Queries.Application.Wage.TaxCode;
public class SalaryCalculatorBuilder
{
    private readonly IWageCalculator _wageCalculatorBuilder;
    public SalaryCalculatorBuilder(CalculateWageRequest request)
    {
        _wageCalculatorBuilder = CreateBuilder(request);
    }

    private IWageCalculator CreateBuilder(CalculateWageRequest request)
    {
        var wageCalculatorBuilder = TaxCodeSelector.SelectTaxCodeImplementorFrom(request.TaxCode);

        wageCalculatorBuilder = new CalculateStudentLoan(wageCalculatorBuilder, request.StudentLoanOptions);

        if (request.Pension != null)
            wageCalculatorBuilder = new CalculatePension(wageCalculatorBuilder, request.Pension);

        if (request.PayNationalInsurance)
            wageCalculatorBuilder = new CalculateNationalInsurance(wageCalculatorBuilder);

        return wageCalculatorBuilder;
    }

    public Money CalculateYearlyWage(Money grossYearlyWage)
    {
        var preTaxResult = _wageCalculatorBuilder.CalculatePreTaxGrossIncome(grossYearlyWage);
        var preTaxGrossYearlyWage = grossYearlyWage - preTaxResult.TotalDeduction;

        var taxOnYearlyWage = _wageCalculatorBuilder.CalculateYearlyWage(preTaxGrossYearlyWage);

        return preTaxGrossYearlyWage - taxOnYearlyWage.TotalDeduction;
    }
}
