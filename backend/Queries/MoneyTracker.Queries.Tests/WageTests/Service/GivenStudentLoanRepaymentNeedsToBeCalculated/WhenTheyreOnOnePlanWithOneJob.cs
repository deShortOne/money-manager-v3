using MoneyTracker.Common.Utilities.MoneyUtil;
using MoneyTracker.Contracts.Requests.Wage;
using MoneyTracker.Queries.Application;

namespace MoneyTracker.Queries.Tests.WageTests.Service.GivenStudentLoanRepaymentNeedsToBeCalculated;
public sealed class WhenTheyreOnOnePlanWithOneJob
{
    public static TheoryData<StudentLoanOptions, Money, Money> GrossYearlyWages = new()
    {
        // Plan 1
        { new StudentLoanOptions(true, false, false, false, false), Money.From(33000), Money.From(52) },
        { new StudentLoanOptions(true, false, false, false, false), Money.From(30000), Money.From(29) },

        // Plan 2
        { new StudentLoanOptions(false, true, false, false, false), Money.From(30000), Money.From(11) },

        // Plan 4
        { new StudentLoanOptions(false, false, true, false, false), Money.From(36000), Money.From(24) },

        // Plan 5
        { new StudentLoanOptions(false, false, false, true, false), Money.From(30000), Money.From(37) },
    };


    [Theory, MemberData(nameof(GrossYearlyWages))]
    public void ThenTheCorrectMonthlyDeductionsAreMade(StudentLoanOptions studentLoanOptions, Money grossYearlyWage, Money expectedStudentLoanRepaymentAmount)
    {
        var actual = WageService.CalculateStudentLoan(grossYearlyWage, studentLoanOptions);

        Assert.Equal(expectedStudentLoanRepaymentAmount, actual);
    }
}
