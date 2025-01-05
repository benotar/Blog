using Blog.Domain.Enums;

namespace Blog.Application.Common;

public class Result<TData>
{
    public TData Data { get; set; }
    public ErrorCode? ErrorCode { get; set; }
    public bool IsSucceed => ErrorCode is null;
    public static Result<TData> Success(TData data = default) => new() { Data = data};
    public static Result<TData> Error(ErrorCode? errorCode) => new() { ErrorCode = errorCode };

    // Allow converting a TData directly into Result<TData>
    public static implicit operator Result<TData>(ErrorCode errorCode) => Error(errorCode);
    public static implicit operator Result<TData>(TData data) => Success(data);
}

public record struct None;