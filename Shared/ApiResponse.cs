using FluentValidation.Results;

namespace Shared;

public class ApiResponse<T>
{
    public bool IsSuccess { get; set; }
    public string? Message { get; set; }
    public string? Error { get; set; }
    public T? Data { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public IEnumerable<object>? ValidationErrors { get; set; }

    public static ApiResponse<T> Success(T data, string message = "عملیات موفق")
        => new() { IsSuccess = true, Message = message, Data = data };

    public static ApiResponse<T> Failure(string error)
        => new() { IsSuccess = false, Error = error };

    public static ApiResponse<object> ValidationFailure(
        IEnumerable<ValidationFailure> errors)
        => new()
        {
            IsSuccess = false,
            Error = "خطای اعتبارسنجی",
            ValidationErrors = errors.Select(e => new { e.PropertyName, e.ErrorMessage })
        };
}
