using Review_Guard.Domain.Enums;

namespace Review_Guard.Domain.ValueObject;

public sealed record AppError(
    string Code,
    string Message,
    ErrorType Type
);