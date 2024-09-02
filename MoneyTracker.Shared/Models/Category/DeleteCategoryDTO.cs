namespace MoneyTracker.Shared.Models.Category
{
    public class DeleteCategoryDTO
    {
        public DeleteCategoryDTO(int id)
        {
            Id = id;
        }
        public int Id { get; private set; }
    }
}
