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

        var grossMonthlyWage = grossYearlyWage / 12;
        var totalStudentLoan = CalculateStudentLoanForPlans1_2_4_5(grossMonthlyWage);

        if (_studentLoanOptions.PostGraduate)
        {
            var postGraduateLoanBand = GetPostGraduateLoanBand();
            var remainingWage = grossMonthlyWage - postGraduateLoanBand.MonthlyIncomeThreshold;
            if (remainingWage > Money.Zero)
            {
                var remainingWageWithPercentageTakeOff = remainingWage * postGraduateLoanBand.Rate;
                totalStudentLoan += Money.From(decimal.Round(remainingWageWithPercentageTakeOff.Amount, 0, MidpointRounding.ToZero));
            }
        }

        return result with
        {
            StudentLoanAmount = totalStudentLoan * 12,
        };
    }

    private Money CalculateStudentLoanForPlans1_2_4_5(Money grossYearlyWage)
    {
        var studentLoanBandPosition = GetStudentLoanBand();
        if (studentLoanBandPosition is null)
            return Money.Zero;

        var totalStudentLoan = Money.Zero;
        var remainingWage = grossYearlyWage - studentLoanBandPosition.MonthlyIncomeThreshold;
        if (remainingWage > Money.Zero)
        {
            var remainingWageWithPercentageTakeOff = remainingWage * studentLoanBandPosition.Rate;
            totalStudentLoan = Money.From(decimal.Round(remainingWageWithPercentageTakeOff.Amount, 0, MidpointRounding.ToZero));
        }

        return totalStudentLoan;
    }

    private StudentLoanBand GetStudentLoanBand()
    {
        if (_studentLoanOptions.Plan5)
            return UkStudentLoanRepayment.StudentLoanBands[StudentLoanPlan.Plan5];

        if (_studentLoanOptions.Plan1)
            return UkStudentLoanRepayment.StudentLoanBands[StudentLoanPlan.Plan1];

        if (_studentLoanOptions.Plan2)
            return UkStudentLoanRepayment.StudentLoanBands[StudentLoanPlan.Plan2];

        if (_studentLoanOptions.Plan4)
            return UkStudentLoanRepayment.StudentLoanBands[StudentLoanPlan.Plan4];

        return null;
    }

    private StudentLoanBand GetPostGraduateLoanBand()
    {
        return UkStudentLoanRepayment.StudentLoanBands[StudentLoanPlan.PostgraduateLoan];
    }
}

public class StudentLoanBand(Money yearlyIncomeThreshold, Money monthlyIncomeThreshold, Money weeklyIncomeThreshold,
    Percentage rate)
{
    public Money YearlyIncomeThreshold { get; } = yearlyIncomeThreshold;
    public Money MonthlyIncomeThreshold { get; } = monthlyIncomeThreshold;
    public Money WeeklyIncomeThreshold { get; } = weeklyIncomeThreshold;
    public Percentage Rate { get; } = rate;
}

public static class UkStudentLoanRepayment
{
    public static readonly Dictionary<StudentLoanPlan, StudentLoanBand> StudentLoanBands = new Dictionary<StudentLoanPlan, StudentLoanBand>
    {
        { StudentLoanPlan.Plan1, new StudentLoanBand(Money.From(26065), Money.From(2172), Money.From(501), Percentage.From(9)) },
        { StudentLoanPlan.Plan2, new StudentLoanBand(Money.From(28470), Money.From(2372), Money.From(547), Percentage.From(9)) },
        { StudentLoanPlan.Plan4, new StudentLoanBand(Money.From(32745), Money.From(2728), Money.From(629), Percentage.From(9)) },
        { StudentLoanPlan.Plan5, new StudentLoanBand(Money.From(25000), Money.From(2083), Money.From(480), Percentage.From(9)) },
        { StudentLoanPlan.PostgraduateLoan, new StudentLoanBand(Money.From(21000), Money.From(1750), Money.From(403), Percentage.From(6)) },
    };
}
