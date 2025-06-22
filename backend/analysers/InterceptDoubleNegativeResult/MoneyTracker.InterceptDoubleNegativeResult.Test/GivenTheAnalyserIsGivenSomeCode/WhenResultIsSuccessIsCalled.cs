using Verifier =
    Microsoft.CodeAnalysis.CSharp.Testing.XUnit.AnalyzerVerifier<
        MoneyTracker.InterceptDoubleNegativeResult.InterceptDoubleNegativeResultAnalyzer>;

namespace MoneyTracker.InterceptDoubleNegativeResult.Test.GivenTheAnalyserIsGivenSomeCode;

public class WhenResultIsSuccessIsCalled
{
    private const string SampleCode = @"
public class Program
{
    public void Main()
    {
        var res = new Result();
        if (res.IsSuccess)
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
    public async Task ThenTheAnalyserFindsNoErrors()
    {
        await Verifier.VerifyAnalyzerAsync(SampleCode);
    }
}
