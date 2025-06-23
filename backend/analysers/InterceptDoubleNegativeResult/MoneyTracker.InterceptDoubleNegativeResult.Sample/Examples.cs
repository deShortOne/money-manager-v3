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
#pragma warning disable MT0001 // Usage of double negative of Result class
        if (!a.IsSuccess)
#pragma warning restore MT0001 // Usage of double negative of Result class
        {
            Console.WriteLine("b");
        }
        if (a.HasError)
        {
            Console.WriteLine("a");
        }
#pragma warning disable MT0001 // Usage of double negative of Result class
        if (!a.HasError)
#pragma warning restore MT0001 // Usage of double negative of Result class
        {
            Console.WriteLine("b");
        }
    }

    public void ResultGenericThing()
    {
        var a = new ResultT<string>("Hii");
        if (a.IsSuccess)
        {
            Console.WriteLine("a");
        }
#pragma warning disable MT0001 // Usage of double negative of Result class
        if (!a.IsSuccess)
#pragma warning restore MT0001 // Usage of double negative of Result class
        {
            Console.WriteLine("b");
        }
        if (a.HasError)
        {
            Console.WriteLine("a");
        }
#pragma warning disable MT0001 // Usage of double negative of Result class
        if (!a.HasError)
#pragma warning restore MT0001 // Usage of double negative of Result class
        {
            Console.WriteLine("b");
        }
    }
}
