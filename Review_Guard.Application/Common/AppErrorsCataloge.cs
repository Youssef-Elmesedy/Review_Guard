using Review_Guard.Domain.Enums;
using Review_Guard.Domain.ValueObject;

namespace Review_Guard.Application.Common;

public static class AppErrorsCataloge
{
    public static AppError NotFound(string message)
        => new(message, ErrorType.NotFound);

    public static AppError Unauthorized(string message)
        => new(message, ErrorType.Unauthorized);

    public static AppError Validation(string message)
        => new(message, ErrorType.Validation);

    public static AppError Conflict(string message)
        => new(message, ErrorType.Conflict);

    public static AppError Failure(string message)
        => new(message, ErrorType.Failure);

    public static AppError Forbidden(string message)
        => new(message, ErrorType.Forbidden);
}