
using System.Text.Json.Serialization;

namespace MoneyTracker.Contracts.Responses.Account;
public class AccountResponse(int id, string name)
{
    [JsonPropertyName("id")]
    public int Id { get; } = id;
    [JsonPropertyName("name")]
    public string Name { get; } = name;

    public override bool Equals(object? obj)
    {
        var other = obj as AccountResponse;
        if (other == null) return false;
        return Id == other.Id && Name == other.Name;
    }

    public override int GetHashCode()
    {
        return Id;
    }
}
