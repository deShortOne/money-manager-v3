
using System.Text.Json.Serialization;

namespace MoneyTracker.Contracts.Responses.Category;
public class CategoryResponse
{
    public CategoryResponse(int id, string name)
    {
        Id = id;
        Name = name;
    }

    [JsonPropertyName("id")]
    public int Id { get; }

    [JsonPropertyName("name")]
    public string Name { get; }

    public override bool Equals(object? obj)
    {
        var other = obj as CategoryResponse;

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
