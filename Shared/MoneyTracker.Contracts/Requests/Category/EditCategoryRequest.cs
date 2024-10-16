
namespace MoneyTracker.Contracts.Requests.Category;
public class EditCategoryRequest
{
    public EditCategoryRequest(int id, string name)
    {
        Id = id;
        Name = name;
    }

    public int Id { get; private set; }
    public string Name { get; private set; }
}
