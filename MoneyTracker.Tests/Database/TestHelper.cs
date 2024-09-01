using Newtonsoft.Json;

namespace MoneyTracker.Tests.Database;
public class TestHelper
{
    public static void CompareLists<T>(List<T> expected, List<T> actual)
    {

        if (expected.Count != actual.Count)
        {
            Assert.Fail($"Length not equal! Expected {expected.Count} but got {actual.Count}");
        }
        for (int i = 0; i < expected.Count; i++)
        {
            if (JsonConvert.SerializeObject(expected[i]) != JsonConvert.SerializeObject(actual[i]))
            {
                Assert.Fail($"Item #{i} failed\nExpected:\n{JsonConvert.SerializeObject(expected[i])}\n" +
                    $"but got:\n{JsonConvert.SerializeObject(actual[i])}");
            }
        }
    }
}
