using MoneyTracker.Common.Utilities;
using MoneyTracker.Common.Utilities.MoneyUtil;
using MoneyTracker.Contracts.Requests.Wage;

namespace MoneyTracker.Queries.Application.Wage;
public class CalculateStudentLoan : IWageCalculator
{
    private readonly IWageCalculator _next;
    private readonly StudentLoanOptions _studentLoanOptions;

    public CalculateStudentLoan(IWageCalculator next, StudentLoanOptions studentLoanOptions)
    {
        _next = next;
        _studentLoanOptions = studentLoanOptions;
    }

    public PreTaxGrossIncomeResult CalculatePreTaxGrossIncome(Money grossYearlyWage)
    {
        return _next.CalculatePreTaxGrossIncome(grossYearlyWage);
    }

    public WageResult CalculateYearlyWage(Money grossYearlyWage)
    {
        var result = _next.CalculateYearlyWage(grossYearlyWage);

        grossYearlyWage /= 12;
        var totalStudentLoan = Money.Zero;
        if (_studentLoanOptions.Plan5)
        {
            var remainingWage = grossYearlyWage - UkStudentLoanRepayment.StudentLoanBands[3].MonthlyIncomeThreshold;
            if (remainingWage > Money.Zero)
            {
                var remainingWageWithPercentageTakeOff = remainingWage * UkStudentLoanRepayment.StudentLoanBands[3].Rate;
                totalStudentLoan = Money.From(decimal.Round(remainingWageWithPercentageTakeOff.Amount, 0, MidpointRounding.ToZero));
            }
        }
        else if (_studentLoanOptions.Plan1)
        {
            var remainingWage = grossYearlyWage - UkStudentLoanRepayment.StudentLoanBands[0].MonthlyIncomeThreshold;
            if (remainingWage > Money.Zero)
            {
                var remainingWageWithPercentageTakeOff = remainingWage * UkStudentLoanRepayment.StudentLoanBands[0].Rate;
                totalStudentLoan = Money.From(decimal.Round(remainingWageWithPercentageTakeOff.Amount, 0, MidpointRounding.ToZero));
            }
        }
        else if (_studentLoanOptions.Plan2)
        {
            var remainingWage = grossYearlyWage - UkStudentLoanRepayment.StudentLoanBands[1].MonthlyIncomeThreshold;
            if (remainingWage > Money.Zero)
            {
                var remainingWageWithPercentageTakeOff = remainingWage * UkStudentLoanRepayment.StudentLoanBands[1].Rate;
                totalStudentLoan = Money.From(decimal.Round(remainingWageWithPercentageTakeOff.Amount, 0, MidpointRounding.ToZero));
            }
        }
        else if (_studentLoanOptions.Plan4)
        {
            var remainingWage = grossYearlyWage - UkStudentLoanRepayment.StudentLoanBands[2].MonthlyIncomeThreshold;
            if (remainingWage > Money.Zero)
            {
                var remainingWageWithPercentageTakeOff = remainingWage * UkStudentLoanRepayment.StudentLoanBands[2].Rate;
                totalStudentLoan = Money.From(decimal.Round(remainingWageWithPercentageTakeOff.Amount, 0, MidpointRounding.ToZero));
            }
        }
        if (_studentLoanOptions.PostGraduate)
        {
            var remainingWage = grossYearlyWage - UkStudentLoanRepayment.StudentLoanBands[4].MonthlyIncomeThreshold;
            if (remainingWage > Money.Zero)
            {
                var remainingWageWithPercentageTakeOff = remainingWage * UkStudentLoanRepayment.StudentLoanBands[4].Rate;
                totalStudentLoan += Money.From(decimal.Round(remainingWageWithPercentageTakeOff.Amount, 0, MidpointRounding.ToZero));
            }
        }

        return result with
        {
            StudentLoanAmount = totalStudentLoan * 12,
        };
    }
}

public class StudentLoanBand(StudentLoanPlan plan, Money yearlyIncomeThreshold, Money monthlyIncomeThreshold,
    Money weeklyIncomeThreshold, Percentage rate)
{
    public StudentLoanPlan StudentLoanPlan { get; } = plan;
    public Money YearlyIncomeThreshold { get; } = yearlyIncomeThreshold;
    public Money MonthlyIncomeThreshold { get; } = monthlyIncomeThreshold;
    public Money WeeklyIncomeThreshold { get; } = weeklyIncomeThreshold;
    public Percentage Rate { get; } = rate;
}

public static class UkStudentLoanRepayment
{
    public static List<StudentLoanBand> StudentLoanBands =
    [
        new StudentLoanBand(StudentLoanPlan.Plan1, Money.From(26065), Money.From(2172), Money.From(501), Percentage.From(9)),
        new StudentLoanBand(StudentLoanPlan.Plan2, Money.From(28470), Money.From(2372), Money.From(547), Percentage.From(9)),
        new StudentLoanBand(StudentLoanPlan.Plan4, Money.From(32745), Money.From(2728), Money.From(629), Percentage.From(9)),
        new StudentLoanBand(StudentLoanPlan.Plan5, Money.From(25000), Money.From(2083), Money.From(480), Percentage.From(9)),
        new StudentLoanBand(StudentLoanPlan.PostgraduateLoan, Money.From(21000), Money.From(1750), Money.From(403), Percentage.From(6)),
    ];
}
