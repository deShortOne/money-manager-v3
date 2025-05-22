using MoneyTracker.Common.Utilities.MoneyUtil;
using MoneyTracker.Contracts.Requests.Wage;
using MoneyTracker.Queries.Application.Wage;
using Moq;

namespace MoneyTracker.Queries.Tests.WageTests.Service.GivenAStudentLoanCalculation;
public sealed class WhenTheyreOnOnePlan
{
    public static TheoryData<StudentLoanOptions, Money, Money> GrossYearlyWages = new()
    {
        // Plan 1
        { new StudentLoanOptions(true, false, false, false, false), Money.From(33000), Money.From(624) },
        { new StudentLoanOptions(true, false, false, false, false), Money.From(30000), Money.From(348) },

        // Plan 2
        { new StudentLoanOptions(false, true, false, false, false), Money.From(30000), Money.From(132) },

        // Plan 4
        { new StudentLoanOptions(false, false, true, false, false), Money.From(36000), Money.From(288) },

        // Plan 5
        { new StudentLoanOptions(false, false, false, true, false), Money.From(30000), Money.From(444) },

        // Post gradudate
        { new StudentLoanOptions(false, false, false, false, true), Money.From(30000), Money.From(540) },
    };


    [Theory, MemberData(nameof(GrossYearlyWages))]
    public void ThenTheCorrectMonthlyDeductionsAreMade(StudentLoanOptions studentLoanOptions, Money grossYearlyWage, Money expectedStudentLoanRepaymentAmount)
    {
        var next = new Mock<WageInterface>();
        next
            .Setup(x => x.CalculateYearlyWage(It.IsAny<Money>()))
            .Returns(new WageResult());

        var calculateStudentLoan = new CalculateStudentLoan(next.Object, studentLoanOptions);
        var actual = calculateStudentLoan.CalculateYearlyWage(grossYearlyWage);

        Assert.Equal(expectedStudentLoanRepaymentAmount, actual.StudentLoanAmount);
    }
}
