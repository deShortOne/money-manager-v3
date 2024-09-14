namespace MoneyTracker.Shared.Models.ControllerToService.Category
{
    public class DeleteCategoryRequestDTO
    {
        public DeleteCategoryRequestDTO(int id)
        {
            Id = id;
        }
        public int Id { get; private set; }
    }
}
