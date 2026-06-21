using Review_Guard.Domain.ValueObject;

namespace Review_Guard.Application.Common.ResultPattern;

public class Result
{
    protected Result(bool isSuccess, AppError? error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;

    public AppError? Error { get; }

    public static Result Success()
        => new(true, null);

    public static Result Failure(AppError error)
        => new(false, error);
}
public class Result<T> : Result
{
    private readonly T? _value;

    private Result(T? value, bool isSuccess, AppError? error)
        : base(isSuccess, error)
    {
        _value = value;
    }

    public T Value =>
        IsSuccess
            ? _value!
            : throw new InvalidOperationException(
                $"Cannot access Value when failure. ErrorMessage: {Error?.Message}");

    public static Result<T> Success(T value)
        => new(value, true, null);

    public new static Result<T> Failure(AppError error)
        => new(default, false, error);
}
