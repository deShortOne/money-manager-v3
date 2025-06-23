using Verifier = Microsoft.CodeAnalysis.CSharp.Testing.XUnit.CodeFixVerifier<
    MoneyTracker.InterceptDoubleNegativeResult.InterceptDoubleNegativeResultAnalyzer,
    MoneyTracker.InterceptDoubleNegativeResult.InterceptDoubleNegativeResultCodeFixProvider>;

namespace MoneyTracker.InterceptDoubleNegativeResult.Test.GivenTheCodeFixerIsGivenSomeCode.ResultT;

public class WhenNotResultTHasErrorIsCalled
{
    private const string InputText = @"
public class MyCompanyClass
{
    public void Main()
    {
        var res = new ResultT<string>(""something cool"");
        if (!res.HasError)
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
        var res = new ResultT<string>(""something cool"");
        if (res.IsSuccess)
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
            .WithArguments("HasError", "IsSuccess");
        await Verifier.VerifyCodeFixAsync(InputText, expected, OutputText);
    }
}
