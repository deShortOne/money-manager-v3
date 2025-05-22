using System.Text.RegularExpressions;
using MoneyTracker.Common.Utilities.MoneyUtil;

namespace MoneyTracker.Queries.Application.Wage.TaxCode;
public class TaxCodeSelector
{
    public static WageInterface SelectTaxCodeImplementorFrom(string taxCode)
    {
        var taxCodeElements = Regex.Match(taxCode, @"^(\d+)(\w+)$");
        var personalAllowanceAmount = Money.From(int.Parse(taxCodeElements.Groups[1].Value) * 10);
        var taxLetter = taxCodeElements.Groups[2].Value.ToUpper();

        if (taxLetter != "L")
        {
            throw new NotImplementedException($"Invalid tax code: {taxCode}");
        }

        return new TaxCodeL(personalAllowanceAmount);
    }
}
