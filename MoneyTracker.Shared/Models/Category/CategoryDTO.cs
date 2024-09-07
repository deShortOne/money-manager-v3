using MoneyTracker.Shared.Models.Bill;

namespace MoneyTracker.Shared.Models.Category
{
    public class CategoryDTO
    {
        public CategoryDTO(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public int Id { get; private set; }
        public string Name { get; private set; }

        public override bool Equals(object? obj)
        {
            var other = obj as CategoryDTO;

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
