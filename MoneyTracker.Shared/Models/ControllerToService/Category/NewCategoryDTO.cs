
namespace MoneyTracker.Shared.Models.ControllerToService.Category
{
    public class NewCategoryRequestDTO
    {
        public NewCategoryRequestDTO(string name)
        {
            Name = name;
        }
        public string Name { get; set; }
    }
}
