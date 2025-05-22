using MoneyTracker.Common.Utilities.MoneyUtil;
using MoneyTracker.Contracts.Requests.Wage;
using MoneyTracker.Queries.Application.Wage;
using MoneyTracker.Queries.Application.Wage.TaxCode;
using Moq;

namespace MoneyTracker.Queries.Tests.WageTests.Service.GivenAStudentLoanCalculation;
public sealed class WhenTheyreOnPlanOneAndPlanTwo
{
    private Money _actual;

    public WhenTheyreOnPlanOneAndPlanTwo()
    {
        var next = new Mock<WageInterface>();
        next
            .Setup(x => x.CalculateYearlyWage(It.IsAny<Money>()))
            .Returns(new WageResult());

        var grossYearlyWage = Money.From(33000);

        var studentLoanOptions = new StudentLoanOptions(true, true, false, false, false);

        var subject = new CalculateStudentLoan(next.Object, studentLoanOptions);

        _actual = subject.CalculateYearlyWage(grossYearlyWage).StudentLoanAmount;
    }

    [Fact]
    public void ThenTheDeductionIsOnlyOfPlanOne()
    {
        Assert.Equal(Money.From(624), _actual);
    }
}
