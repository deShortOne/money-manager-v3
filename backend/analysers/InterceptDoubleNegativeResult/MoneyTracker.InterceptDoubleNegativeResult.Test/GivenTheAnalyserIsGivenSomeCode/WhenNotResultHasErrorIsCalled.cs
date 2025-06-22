using Verifier =
    Microsoft.CodeAnalysis.CSharp.Testing.XUnit.AnalyzerVerifier<
        MoneyTracker.InterceptDoubleNegativeResult.InterceptDoubleNegativeResultAnalyzer>;


namespace MoneyTracker.InterceptDoubleNegativeResult.Test.GivenTheAnalyserIsGivenSomeCode;

public class WhenNotResultHasErrorIsCalled
{
    private const string SampleCode = @"
public class Program
{
    public void Main()
    {
        var res = new Result();
        if (!res.HasError)
        {

        }
    }
}

public class Result
{
    public bool HasError => true;
}
";

    [Fact]
    public async Task ThenTheAnalyserFindsTheErrorsWithTheCorrectParameters()
    {
        var expected = Verifier.Diagnostic()
            .WithLocation(7, 13)
            .WithArguments("HasError", "IsSuccess");

        await Verifier.VerifyAnalyzerAsync(SampleCode, expected);
    }
}
