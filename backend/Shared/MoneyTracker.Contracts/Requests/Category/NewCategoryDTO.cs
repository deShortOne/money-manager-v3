
namespace MoneyTracker.Contracts.Requests.Category;
public class NewCategoryRequest
{
    public NewCategoryRequest(string name)
    {
        Name = name;
    }
    public string Name { get; set; }
}
