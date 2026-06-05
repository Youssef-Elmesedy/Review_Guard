using Review_Guard.Application.Abstractions.Specifications;
using Review_Guard.Domain.Entities;
using Review_Guard.Domain.Enums;

namespace Review_Guard.Application.Feature.Auth.Specifications;

public sealed class ActiveVerificationCodeSpecification
    : BaseSpecification<VerificationCode>
{
    public ActiveVerificationCodeSpecification(
        Guid userId,
        VerificationCodeType type)
    {
        AddCriteria(x =>
            x.UserId == userId &&
            x.Type == type);

        EnableTracking();
    }
}
