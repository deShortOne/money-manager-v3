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

    public WageResult CalculateYearlyWage(Money grossYearlyWage)
    {
        return _wageCalculatorBuilder.CalculateYearlyWage(grossYearlyWage);
    }
}
