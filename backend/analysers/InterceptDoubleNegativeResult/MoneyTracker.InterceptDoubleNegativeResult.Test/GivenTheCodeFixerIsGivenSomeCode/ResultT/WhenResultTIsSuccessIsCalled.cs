using Verifier = Microsoft.CodeAnalysis.CSharp.Testing.XUnit.CodeFixVerifier<
    MoneyTracker.InterceptDoubleNegativeResult.InterceptDoubleNegativeResultAnalyzer,
    MoneyTracker.InterceptDoubleNegativeResult.InterceptDoubleNegativeResultCodeFixProvider>;

namespace MoneyTracker.InterceptDoubleNegativeResult.Test.GivenTheCodeFixerIsGivenSomeCode.ResultT;

public class WhenResultTIsSuccessIsCalled
{
    private const string InputText = @"
public class MyCompanyClass
{
    public void Main()
    {
        var res = new ResultT<int>(69);
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
    public async Task ThenTheCodeIsKeptTheSame()
    {
        await Verifier.VerifyCodeFixAsync(InputText, InputText);
    }
}
