using System.Threading.Tasks;
using Xunit;
using Verifier =
    Microsoft.CodeAnalysis.CSharp.Testing.XUnit.CodeFixVerifier<InterceptDoubleNegativeResult.InterceptDoubleNegativeResultAnalyzer,
        InterceptDoubleNegativeResult.InterceptDoubleNegativeResultCodeFixProvider>;

namespace InterceptDoubleNegativeResult.Tests.GivenTheCodeFixerIsGivenSomeCode;

public class WhenResultHasErrorIsCalled
{
    private const string InputText = @"
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
    public async Task ThenTheCodeIsKeptTheSame()
    {
        await Verifier.VerifyCodeFixAsync(InputText, InputText).ConfigureAwait(false);
    }
}
