using Review_Guard.Application.Feature.Auth.Specifications;
using Review_Guard.Domain.Enums;

namespace Review_Guard.Infrastructure.Implementation.Servcices.VerificationTokensService;

internal sealed class VerificationCodeService : IVerificationCodeService
{
    private readonly IGenericReadRepository<VerificationCode> _readToken;
    private readonly IGenericWriteRepository<VerificationCode> _writeToken;
    private readonly IUnitOfWork _unitOfWork;

    public VerificationCodeService(
        IGenericReadRepository<VerificationCode> readToken,
        IGenericWriteRepository<VerificationCode> writeToken,
        IUnitOfWork unitOfWork)
    {
        _readToken = readToken;
        _writeToken = writeToken;
        _unitOfWork = unitOfWork;
    }

    public async Task<VerificationCode> CreateOrRefreshAsync(
        Guid userId,
        VerificationCodeType type,
        int expiryMinutes,
        CancellationToken ct = default)
    {
        var spc = new ActiveVerificationCodeSpecification(userId, type);

        var existingCode = await _readToken.FirstOrDefaultAsync(spc, ct);

        if (existingCode is not null)
        {
            existingCode.Refresh(expiryMinutes, type);

            await _unitOfWork.SaveChangesAsync(ct);

            return existingCode;
        }

        var token = VerificationCode.Create(
            userId,
            type,
            expiryMinutes);

        await _writeToken.AddAsync(token, ct);

        await _unitOfWork.SaveChangesAsync(ct);

        return token;
    }
}
