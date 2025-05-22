using System.Text.RegularExpressions;
using MoneyTracker.Common.Utilities.MoneyUtil;
using MoneyTracker.Contracts.Requests.Wage;

namespace MoneyTracker.Queries.Application.Wage.TaxCode;
public class TaxCodeSelector
{
    public static WageInterface SelectTaxCodeImplementorFrom(CalculateWageRequest request)
    {
        var taxCodeElements = Regex.Match(request.TaxCode, @"^(\d+)(\w+)$");
        var personalAllowanceAmount = Money.From(int.Parse(taxCodeElements.Groups[1].Value) * 10);
        var taxLetter = taxCodeElements.Groups[2].Value.ToUpper();

        if (taxLetter != "L")
        {
            throw new NotImplementedException("Only tax letter L is accepted");
        }

        WageInterface wageCalculatorBuilder = new TaxCodeL(personalAllowanceAmount);

        wageCalculatorBuilder = new CalculateStudentLoan(wageCalculatorBuilder, request.StudentLoanOptions);

        if (request.Pension != null)
            wageCalculatorBuilder = new CalculatePension(wageCalculatorBuilder, request.Pension);

        if (request.PayNationalInsurance)
            wageCalculatorBuilder = new CalculateNationalInsurance(wageCalculatorBuilder);

        return wageCalculatorBuilder;
    }
}
