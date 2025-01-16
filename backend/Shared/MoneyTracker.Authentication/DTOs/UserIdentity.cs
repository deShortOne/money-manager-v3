
public class UserIdentity(string identifier)
{
    public string Identifier { get; set; } = identifier;

    public override bool Equals(object? obj)
    {
        var other = obj as UserIdentity;
        if (other == null)
            return false;
        return Identifier == other.Identifier;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Identifier);
    }
}
