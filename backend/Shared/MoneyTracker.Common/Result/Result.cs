
using System.Text.Json.Serialization;

namespace MoneyTracker.Common.Result;
public class Result
{
    protected Result()
    {
        IsSuccess = true;
        Error = default;
    }

    protected Result(Error error)
    {
        IsSuccess = false;
        Error = error;
    }

    public bool IsSuccess { get; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public Error? Error { get; }

    public static implicit operator Result(Error error) =>
        new(error);

    public static Result Success() =>
        new();

    public static Result Failure(Error error) =>
        new(error);

    public override bool Equals(object? obj)
    {
        var other = obj as Result;
        if (other == null) return false;
        if (Error != null)
            if (!Error.Equals(other.Error))
                return false;

        return IsSuccess == other.IsSuccess;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(IsSuccess, Error);
    }
}
