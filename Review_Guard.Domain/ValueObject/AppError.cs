using Review_Guard.Domain.Enums;

namespace Review_Guard.Domain.ValueObject;

public sealed record AppError(
    string Message,
    ErrorType Type
);