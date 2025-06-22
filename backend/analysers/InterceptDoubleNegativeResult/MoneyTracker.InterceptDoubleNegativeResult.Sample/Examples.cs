// ReSharper disable UnusedMember.Global
namespace MoneyTracker.InterceptDoubleNegativeResult.Sample;


// If you don't see warnings, build the Analyzers Project.
public class Examples
{
    public void ResultThings()
    {
        var a = new Result();
        if (a.IsSuccess)
        {
            Console.WriteLine("a");
        }
        if (!a.IsSuccess) // Errors
        {
            Console.WriteLine("b");
        }
        if (a.HasError)
        {
            Console.WriteLine("a");
        }
        if (!a.HasError) // Errors
        {
            Console.WriteLine("b");
        }
    }
}
