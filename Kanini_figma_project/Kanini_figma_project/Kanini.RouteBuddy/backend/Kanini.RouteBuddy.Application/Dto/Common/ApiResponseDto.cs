namespace Kanini.RouteBuddy.Application.Dto.Common;

public class ApiResponseDto<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public IEnumerable<string>? Errors { get; set; }

    public static ApiResponseDto<T> SuccessResult(T data, string message = "Success")
        => new() { Success = true, Message = message, Data = data };

    public static ApiResponseDto<T> ErrorResult(string message, IEnumerable<string>? errors = null)
        => new() { Success = false, Message = message, Errors = errors };
}