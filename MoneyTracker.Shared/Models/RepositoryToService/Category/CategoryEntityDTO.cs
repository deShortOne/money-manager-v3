
namespace MoneyTracker.Shared.Models.RepositoryToService.Category
{
    public class CategoryEntityDTO
    {
        public CategoryEntityDTO(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public int Id { get; private set; }
        public string Name { get; private set; }

        public override bool Equals(object? obj)
        {
            var other = obj as CategoryEntityDTO;

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
