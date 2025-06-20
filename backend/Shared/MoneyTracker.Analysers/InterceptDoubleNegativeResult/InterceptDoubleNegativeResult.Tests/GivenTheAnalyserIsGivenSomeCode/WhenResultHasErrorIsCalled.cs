using System.Threading.Tasks;
using Xunit;
using Verifier =
    Microsoft.CodeAnalysis.CSharp.Testing.XUnit.AnalyzerVerifier<
        InterceptDoubleNegativeResult.InterceptDoubleNegativeResultAnalyzer>;

namespace InterceptDoubleNegativeResult.Tests.GivenTheAnalyserIsGivenSomeCode;

public class WhenResultHasErrorIsCalled
{
    private const string SampleCode = @"
public class Program
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
    public bool HasError => true;
}
";

    [Fact]
    public async Task ThenTheAnalyserFindsNoErrors()
    {
        await Verifier.VerifyAnalyzerAsync(SampleCode);
    }
}
