namespace MoneyTracker.Shared.Models.ControllerToService.Category;

public class EditCategoryRequestDTO
{
    public EditCategoryRequestDTO(int id, string name)
    {
        Id = id;
        Name = name;
    }

    public int Id { get; private set; }
    public string Name { get; private set; }
}
