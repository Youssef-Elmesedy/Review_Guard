using Review_Guard.Domain.Enums;
using Review_Guard.Domain.ValueObject;

namespace Review_Guard.Application.Common;

public static class AppErrorsCataloge
{
    public static AppError NotFound(string code, string message)
        => new(code, message, ErrorType.NotFound);

    public static AppError Unauthorized(string code, string message)
        => new(code, message, ErrorType.Unauthorized);

    public static AppError Validation(string code, string message)
        => new(code, message, ErrorType.Validation);

    public static AppError Conflict(string code, string message)
        => new(code, message, ErrorType.Conflict);

    public static AppError Failure(string code, string message)
        => new(code, message, ErrorType.Failure);

    public static AppError Forbidden(string code, string message)
        => new(code, message, ErrorType.Forbidden);
}