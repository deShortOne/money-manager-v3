
namespace MoneyTracker.Shared.Models.ServiceToController.Category
{
    public class CategoryResponseDTO
    {
        public CategoryResponseDTO(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public int Id { get; private set; }
        public string Name { get; private set; }

        public override bool Equals(object? obj)
        {
            var other = obj as CategoryResponseDTO;

            if (other == null)
            {
                return false;
            }
            return Id == other.Id && Name == other.Name;
        }

        public override int GetHashCode()
        {
            return Id;
        }
    }
}
