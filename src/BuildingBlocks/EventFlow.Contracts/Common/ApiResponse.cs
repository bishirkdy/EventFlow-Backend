using System.Net;

namespace EventFlow.Contracts.Common;

public sealed class ApiResponse<T>
{
    public bool IsSuccess { get; init; }
    public int StatusCode { get; init; }
    public T? Data { get; init; }
    public string Message { get; init; } = string.Empty;
    public List<string>? Errors { get; init; }
    public string? ErrorCode { get; init; }

    public static ApiResponse<T> Success(
        T data,
        string message = "Success",
        HttpStatusCode statusCode = HttpStatusCode.OK) => new()
        {
            IsSuccess = true,
            StatusCode = (int)statusCode,
            Data = data,
            Message = message
        };

    public static ApiResponse<T> Fail(
        IEnumerable<string> errors,
        string message = "Failed",
        HttpStatusCode statusCode = HttpStatusCode.BadRequest,
        string? errorCode = null) => new()
        {
            IsSuccess = false,
            StatusCode = (int)statusCode,
            Message = message,
            Errors = errors.Distinct().ToList(),
            ErrorCode = errorCode
        };
}
