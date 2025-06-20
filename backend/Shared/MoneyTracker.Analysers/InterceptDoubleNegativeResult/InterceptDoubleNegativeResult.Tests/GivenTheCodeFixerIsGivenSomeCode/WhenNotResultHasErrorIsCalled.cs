using System.Threading.Tasks;
using Xunit;
using Verifier =
    Microsoft.CodeAnalysis.CSharp.Testing.XUnit.CodeFixVerifier<InterceptDoubleNegativeResult.InterceptDoubleNegativeResultAnalyzer,
        InterceptDoubleNegativeResult.InterceptDoubleNegativeResultCodeFixProvider>;

namespace InterceptDoubleNegativeResult.Tests.GivenTheCodeFixerIsGivenSomeCode;

public class WhenNotResultHasErrorIsCalled
{
    private const string InputText = @"
public class MyCompanyClass
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
    public async Task ThenTheCodeIsUpdated()
    {
        var expected = Verifier.Diagnostic()
            .WithLocation(7, 13)
            .WithArguments("HasError", "IsSuccess");
        await Verifier.VerifyCodeFixAsync(InputText, expected, OutputText).ConfigureAwait(false);
    }
}
