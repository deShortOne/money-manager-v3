using Verifier = Microsoft.CodeAnalysis.CSharp.Testing.XUnit.CodeFixVerifier<
    MoneyTracker.InterceptDoubleNegativeResult.InterceptDoubleNegativeResultAnalyzer,
    MoneyTracker.InterceptDoubleNegativeResult.InterceptDoubleNegativeResultCodeFixProvider>;

namespace MoneyTracker.InterceptDoubleNegativeResult.Test.GivenTheCodeFixerIsGivenSomeCode.ResultT;

public class WhenNotResultTIsSuccessIsCalled
{
    private const string InputText = @"
public class MyCompanyClass
{
    public void Main()
    {
        var res = new ResultT<decimal>(297437);
        if (!res.IsSuccess)
        {

        }
    }
}

public class ResultT<TValue>(TValue value)
{
    public bool IsSuccess => false;
    public bool HasError => true;
    public TValue Value = value;
}
";

    private const string OutputText = @"
public class MyCompanyClass
{
    public void Main()
    {
        var res = new ResultT<decimal>(297437);
        if (res.HasError)
        {

        }
    }
}

public class ResultT<TValue>(TValue value)
{
    public bool IsSuccess => false;
    public bool HasError => true;
    public TValue Value = value;
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
