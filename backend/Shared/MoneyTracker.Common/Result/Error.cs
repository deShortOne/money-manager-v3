
namespace MoneyTracker.Common.Result;
public class Error
{
    private Error(
        string code,
        string description,
        ErrorType errorType
        )
    {
        Code = code;
        Description = description;
        ErrorType = errorType;
    }
    public string Code { get; }
    public string Description { get; }
    public ErrorType ErrorType { get; }

    public static Error Failure(string code, string description) =>
        new(code, description, ErrorType.Failure);

    public static Error NotFound(string code, string description) =>
        new(code, description, ErrorType.NotFound);

    public static Error Validation(string code, string description) =>
        new(code, description, ErrorType.Validation);

    public static Error AccessUnAuthorised(string code, string description) =>
        new(code, description, ErrorType.AccessUnAuthorised);

    public override bool Equals(object? obj)
    {
        var other = obj as Error;
        if (other == null) return false;
        return Code == other.Code
            && Description == other.Description
            && ErrorType == other.ErrorType;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Code, Description, ErrorType);
    }
}
