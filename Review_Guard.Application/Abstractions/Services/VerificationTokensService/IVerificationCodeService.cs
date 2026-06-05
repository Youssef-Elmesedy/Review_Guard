using Review_Guard.Domain.Entities;
using Review_Guard.Domain.Enums;

namespace Review_Guard.Application.Abstractions.Services.VerificationTokensService;

public interface IVerificationCodeService
{
    Task<VerificationCode> CreateOrRefreshAsync(
        Guid userId,
        VerificationCodeType type,
        int expiryMinutes,
        CancellationToken ct = default);
}