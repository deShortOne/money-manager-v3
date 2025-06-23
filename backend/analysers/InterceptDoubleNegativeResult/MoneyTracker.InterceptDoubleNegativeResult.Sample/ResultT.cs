
namespace MoneyTracker.InterceptDoubleNegativeResult.Sample;

// not implemented properly but this is for test purposes
internal class ResultT<TValue>(TValue value) : Result
{
    public readonly TValue? Value = value;
}
