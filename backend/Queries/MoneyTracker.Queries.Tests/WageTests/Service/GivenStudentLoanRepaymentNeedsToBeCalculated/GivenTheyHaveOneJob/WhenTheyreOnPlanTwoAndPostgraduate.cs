using MoneyTracker.Common.Utilities.MoneyUtil;
using MoneyTracker.Contracts.Requests.Wage;
using MoneyTracker.Queries.Application;

namespace MoneyTracker.Queries.Tests.WageTests.Service.GivenStudentLoanRepaymentNeedsToBeCalculated.GivenTheyHaveOneJob;
public sealed class WhenTheyreOnPlanTwoAndPostgraduate
{
    private Money _actual;

    public WhenTheyreOnPlanTwoAndPostgraduate()
    {
        var grossYearlyWage = Money.From(28800);
        var studentLoanOptions = new StudentLoanOptions(false, true, false, false, true);

        _actual = WageService.CalculateStudentLoan(grossYearlyWage, studentLoanOptions);
    }

    [Fact]
    public void ThenTheDeductionIsOnlyOfPlanOne()
    {
        Assert.Equal(Money.From(41), _actual);
    }
}
