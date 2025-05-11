using MoneyTracker.Common.Utilities.MoneyUtil;
using MoneyTracker.Contracts.Requests.Wage;
using MoneyTracker.Queries.Application;

namespace MoneyTracker.Queries.Tests.WageTests.Service.GivenStudentLoanRepaymentNeedsToBeCalculated.GivenTheyHaveOneJob;
public sealed class WhenTheyreOnPlanTwoAndPlanFive
{
    private Money _actual;

    public WhenTheyreOnPlanTwoAndPlanFive()
    {
        var grossYearlyWage = Money.From(30000);
        var studentLoanOptions = new StudentLoanOptions(false, true, false, true, false);

        _actual = WageService.CalculateStudentLoan(grossYearlyWage, studentLoanOptions);
    }

    [Fact]
    public void ThenTheDeductionIsOnlyOfPlanOne()
    {
        Assert.Equal(Money.From(37), _actual);
    }
}
