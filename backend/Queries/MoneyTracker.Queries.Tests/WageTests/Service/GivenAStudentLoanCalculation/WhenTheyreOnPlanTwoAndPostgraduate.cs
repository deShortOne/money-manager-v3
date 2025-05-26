using MoneyTracker.Common.Utilities.MoneyUtil;
using MoneyTracker.Contracts.Requests.Wage;
using MoneyTracker.Queries.Application;
using MoneyTracker.Queries.Application.Wage;
using MoneyTracker.Queries.Application.Wage.TaxCode;
using Moq;

namespace MoneyTracker.Queries.Tests.WageTests.Service.GivenAStudentLoanCalculation;
public sealed class WhenTheyreOnPlanTwoAndPostgraduate
{
    private Money _actual;

    public WhenTheyreOnPlanTwoAndPostgraduate()
    {
        var next = new Mock<IWageCalculator>();
        next
            .Setup(x => x.CalculateYearlyWage(It.IsAny<Money>()))
            .Returns(new WageResult());

        var grossYearlyWage = Money.From(28800);

        var studentLoanOptions = new StudentLoanOptions(false, true, false, false, true);

        var subject = new CalculateStudentLoan(next.Object, studentLoanOptions);

        _actual = subject.CalculateYearlyWage(grossYearlyWage).StudentLoanAmount;
    }

    [Fact]
    public void ThenTheDeductionIsOnlyOfPlanOne()
    {
        Assert.Equal(Money.From(492), _actual);
    }
}
