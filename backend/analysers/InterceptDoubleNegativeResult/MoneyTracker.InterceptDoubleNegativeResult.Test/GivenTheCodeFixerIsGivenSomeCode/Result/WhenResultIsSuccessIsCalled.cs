using Verifier = Microsoft.CodeAnalysis.CSharp.Testing.XUnit.CodeFixVerifier<
    MoneyTracker.InterceptDoubleNegativeResult.InterceptDoubleNegativeResultAnalyzer,
    MoneyTracker.InterceptDoubleNegativeResult.InterceptDoubleNegativeResultCodeFixProvider>;

namespace MoneyTracker.InterceptDoubleNegativeResult.Test.GivenTheCodeFixerIsGivenSomeCode.Result;

public class WhenResultIsSuccessIsCalled
{
    private const string InputText = @"
public class MyCompanyClass
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
    public bool IsSuccess => false;
    public bool HasError => true;
}
";

    [Fact]
    public async Task ThenTheCodeIsKeptTheSame()
    {
        await Verifier.VerifyCodeFixAsync(InputText, InputText);
    }
}
