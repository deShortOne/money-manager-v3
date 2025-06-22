using Verifier = Microsoft.CodeAnalysis.CSharp.Testing.XUnit.CodeFixVerifier<
    MoneyTracker.InterceptDoubleNegativeResult.InterceptDoubleNegativeResultAnalyzer,
    MoneyTracker.InterceptDoubleNegativeResult.InterceptDoubleNegativeResultCodeFixProvider>;

namespace MoneyTracker.InterceptDoubleNegativeResult.Test.GivenTheCodeFixerIsGivenSomeCode;

public class WhenNotResultIsSuccessIsCalled
{
    private const string InputText = @"
public class MyCompanyClass
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
    public bool IsSuccess => false;
    public bool HasError => true;
}
";

    private const string OutputText = @"
public class MyCompanyClass
{
    public void Main()
    {
        var res = new Result();
        if (res.HasError)
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
    public async Task ThenTheCodeIsUpdated()
    {
        var expected = Verifier.Diagnostic()
            .WithLocation(7, 13)
            .WithArguments("IsSuccess", "HasError");

        await Verifier.VerifyCodeFixAsync(InputText, expected, OutputText);
    }
}
