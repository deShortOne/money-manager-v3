using MoneyTracker.Common.Utilities.MoneyUtil;
using MoneyTracker.Contracts.Requests.Wage;
using MoneyTracker.Queries.Application;

namespace MoneyTracker.Queries.Tests.WageTests.Service.GivenStudentLoanRepaymentNeedsToBeCalculated.GivenTheyHaveOneJob;
public sealed class WhenTheyreOnPlanTwoAndPlanFour
{
    private Money _actual;

    public WhenTheyreOnPlanTwoAndPlanFour()
    {
        var grossYearlyWage = Money.From(30000);
        var studentLoanOptions = new StudentLoanOptions(false, true, true, false, false);

        _actual = WageService.CalculateStudentLoan(grossYearlyWage, studentLoanOptions);
    }

    [Fact]
    public void ThenTheDeductionIsOnlyOfPlanOne()
    {
        Assert.Equal(Money.From(11), _actual);
    }
}
