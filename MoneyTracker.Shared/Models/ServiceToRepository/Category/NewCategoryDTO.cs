
namespace MoneyTracker.Shared.Models.ServiceToRepository.Category
{
    public class NewCategoryDTO
    {
        public NewCategoryDTO(string name)
        {
            Name = name;
        }
        public string Name { get; set; }
    }
}
