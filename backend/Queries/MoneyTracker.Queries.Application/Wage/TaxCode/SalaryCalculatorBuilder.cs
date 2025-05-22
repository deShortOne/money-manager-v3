using MoneyTracker.Contracts.Requests.Wage;

namespace MoneyTracker.Queries.Application.Wage.TaxCode;
public class SalaryCalculatorBuilder
{
    public static WageInterface CreateBuilder(CalculateWageRequest request)
    {
        WageInterface wageCalculatorBuilder = TaxCodeSelector.SelectTaxCodeImplementorFrom(request.TaxCode);

        wageCalculatorBuilder = new CalculateStudentLoan(wageCalculatorBuilder, request.StudentLoanOptions);

        if (request.Pension != null)
            wageCalculatorBuilder = new CalculatePension(wageCalculatorBuilder, request.Pension);

        if (request.PayNationalInsurance)
            wageCalculatorBuilder = new CalculateNationalInsurance(wageCalculatorBuilder);

        return wageCalculatorBuilder;
    }
}
