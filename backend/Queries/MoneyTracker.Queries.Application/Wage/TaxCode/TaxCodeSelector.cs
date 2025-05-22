using System.Text.RegularExpressions;
using MoneyTracker.Common.Utilities.MoneyUtil;

namespace MoneyTracker.Queries.Application.Wage.TaxCode;
public class TaxCodeSelector
{
    public static WageInterface SelectTaxCodeImplementorFrom(string taxCode)
    {
        if (taxCode.Trim().ToUpper() == "BR")
            return new CalculateTaxCodeBR();
        if (taxCode.Trim().ToUpper() == "D0")
            return new CalculateTaxCodeD0();
        if (taxCode.Trim().ToUpper() == "D1")
            return new CalculateTaxCodeD1();
        if (taxCode.Trim().ToUpper() == "NT")
            return new CalculateTaxCodeNT();

        var taxCodeElements = Regex.Match(taxCode, @"^(\d+)(\w+)$");
        var personalAllowanceAmount = Money.From(int.Parse(taxCodeElements.Groups[1].Value) * 10);
        var taxLetter = taxCodeElements.Groups[2].Value.ToUpper();

        if (taxLetter is "L" or "M" or "N")
        {
            return new CalculateProgressiveTaxAfterPersonalAllowance(personalAllowanceAmount);
        }

        throw new NotImplementedException($"Invalid tax code: {taxCode}");
    }
}
