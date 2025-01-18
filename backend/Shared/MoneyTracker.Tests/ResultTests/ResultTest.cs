
using MoneyTracker.Common.Result;

namespace MoneyTracker.Tests.ResultTests;
public sealed class ResultTest
{
    [Fact]
    public void SuccessForType()
    {
        var a = ResultT<int>.Success(1);
        var b = ResultT<int>.Success(1);

        Assert.Equal(a, b);
    }

    [Fact]
    public void SuccessForListOfTypes()
    {
        var a = ResultT<List<int>>.Success([1, 2, 3]);
        var b = ResultT<List<int>>.Success([1, 2, 3]);

        Assert.Equal(a, b);
    }
}
