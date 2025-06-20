using System.Threading.Tasks;
using Xunit;
using Verifier =
    Microsoft.CodeAnalysis.CSharp.Testing.XUnit.AnalyzerVerifier<
        InterceptDoubleNegativeResult.InterceptDoubleNegativeResultAnalyzer>;

namespace InterceptDoubleNegativeResult.Tests.GivenTheAnalyserIsGivenSomeCode;

public class WhenNotResultIsSuccessIsCalled
{
    private const string SampleCode = @"
public class Program
{
    public void Main()
    {
        var res = new Result();
        if (!res.IsSuccess)
        {

        }
    }
}

public class Result
{
    public bool IsSuccess => true;
}
";

    [Fact]
    public async Task ThenTheAnalyserFindsTheErrorsWithTheCorrectParameters()
    {
        var expected = Verifier.Diagnostic()
            .WithLocation(7, 13)
            .WithArguments("IsSuccess", "HasError");
        await Verifier.VerifyAnalyzerAsync(SampleCode, expected);
    }
}
