namespace E_Commerce.Service.Abstraction.Common;

public class Error
{
    public string Code { get; }
    public string Description { get; }
    public ErrorType Type { get; }
    public Error(string code, string description, ErrorType type)
    {
        Code = code;
        Description = description;
        Type = type;
    }
    public static Error Failure(string code = "General.Failure", string description = "A general failure has occurred.") =>
        new Error(code, description, ErrorType.Failure);
    public static Error Validation(string code = "General.Validation", string description = "A validation error has occurred.") =>
        new Error(code, description, ErrorType.Validation);
    public static Error NotFound(string code = "General.NotFound", string description = "The requested resource was not found.") =>
        new Error(code, description, ErrorType.NotFound);
    public static Error Conflict(string code = "General.Conflict", string description = "A conflict has occurred.") =>
        new Error(code, description, ErrorType.Conflict);
    public static Error Unauthorized(string code = "General.Unauthorized", string description = "An unauthorized error has occurred.") =>
        new Error(code, description, ErrorType.Unauthorized);
}
