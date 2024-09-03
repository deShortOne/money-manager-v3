namespace MoneyTracker.Shared.Models.Category
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
