namespace EventFlow.Contracts.Common;

public static class ApiErrorCodes
{
    public const string Validation = "Validation.Error";
    public const string BadRequest = "Request.Invalid";
    public const string NotFound = "Resource.NotFound";
    public const string Conflict = "Resource.Conflict";
    public const string Unauthorized = "Auth.Unauthorized";
    public const string Forbidden = "Auth.Forbidden";
    public const string InternalServerError = "Server.InternalError";
}
