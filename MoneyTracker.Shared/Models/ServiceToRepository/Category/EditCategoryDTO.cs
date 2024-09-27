namespace MoneyTracker.Shared.Models.ServiceToRepository.Category;

public class EditCategoryDTO
{
    public EditCategoryDTO(int id, string name)
    {
        Id = id;
        Name = name;
    }

    public int Id { get; private set; }
    public string Name { get; private set; }
}
