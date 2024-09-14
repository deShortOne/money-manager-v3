namespace MoneyTracker.Shared.Models.ServiceToRepository.Category
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
