using System.Net;
namespace EventFlow.Contracts.Common;

// Generic API response wrapper.
public sealed class ApiResponse<T>
{
    // Indicates whether the API operation was successful.
    public bool IsSuccess { get; init; }
    public int StatusCode { get; init; }

    // Actual response data.
    // T? means the data can be null, especially for failed responses.
    public T? Data { get; init; }

    // Human-readable message describing the result.
    public string Message { get; init; } = string.Empty;

    // Collection of validation or business errors.
    // Null when there are no errors.
    public List<string>? Errors { get; init; }

    // Optional machine-readable error code.
    // Useful when the frontend needs to handle specific errors.
    public string? ErrorCode { get; init; }

    // Creates a successful API response.
    public static ApiResponse<T> Success(T data, string message = "Success", HttpStatusCode statusCode = HttpStatusCode.OK) => new()
        {
            IsSuccess = true,
            StatusCode = (int)statusCode,
            Data = data,
            Message = message
        };

    // Creates a failed API response.
    public static ApiResponse<T> Fail(
        IEnumerable<string> errors,
        string message = "Failed",
        HttpStatusCode statusCode = HttpStatusCode.BadRequest,
        string? errorCode = null) => new()
        {
            IsSuccess = false,
            StatusCode = (int)statusCode,
            Message = message,

            // Removes duplicate errors and converts the result to a List.
            Errors = errors.Distinct().ToList(),

            // Optional code that identifies the type of error.
            ErrorCode = errorCode
        };
}
