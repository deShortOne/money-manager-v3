
namespace MoneyTracker.Contracts.Requests.Category;
public class DeleteCategoryRequest
{
    public DeleteCategoryRequest(int id)
    {
        Id = id;
    }
    public int Id { get; private set; }
}
