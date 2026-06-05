using Review_Guard.Application.Feature.Auth.Specifications;
using System.Security;

namespace Review_Guard.Infrastructure.Authentication;

public sealed class RefreshTokenService : IRefreshTokenService
{
    private readonly IGenericWriteRepository<RefreshToken> _writeRepo;
    private readonly IGenericReadRepository<RefreshToken> _readRepo;
    private readonly IUnitOfWork _unitOfWork;

    public RefreshTokenService(
        IGenericWriteRepository<RefreshToken> writeRepo,
        IGenericReadRepository<RefreshToken> readRepo,
        IUnitOfWork unitOfWork)
    {
        _writeRepo = writeRepo;
        _readRepo = readRepo;
        _unitOfWork = unitOfWork;
    }

    // ─────────────────────────────────────────────
    // Generate
    // ─────────────────────────────────────────────
    public RefreshToken Generate(
        Guid? userId,
        Guid? adminId,
        string? ipAddress)
    {
        var token =
            Convert.ToBase64String(Guid.NewGuid().ToByteArray());

        return RefreshToken.Create(
            token,
            DateTime.UtcNow.AddDays(7),
            ipAddress!,
            userId,
            adminId);
    }

    // ─────────────────────────────────────────────
    // Get Or Create
    // ─────────────────────────────────────────────
    public async Task<RefreshToken> GetOrCreateAsync(
        Guid? userId,
        Guid? adminId,
        string? ipAddress,
        CancellationToken ct = default)
    {
        var spc =
            new GetRefreshTokenForUserOrUpdate(
                userId,
                adminId,
                ipAddress);

        var existingToken =
            await _readRepo.FirstOrDefaultAsync(spc, ct);

        if (existingToken is not null &&
            existingToken.IsActive)
        {
            return existingToken;
        }

        var refreshToken =
            Generate(userId, adminId, ipAddress);

        await _writeRepo.AddAsync(refreshToken, ct);

        await _unitOfWork.SaveChangesAsync(ct);

        return refreshToken;
    }

    // ─────────────────────────────────────────────
    // Get
    // ─────────────────────────────────────────────
    public async Task<RefreshToken?> GetAsync(
        string token,
        CancellationToken ct = default)
    {
        return await _readRepo.FindFirstAsync(
            x => x.Token == token,
            ct);
    }

    // ─────────────────────────────────────────────
    // Revoke
    // ─────────────────────────────────────────────
    public async Task RevokeAsync(
        RefreshToken refreshToken,
        string ipAddress,
        string reason,
        CancellationToken ct = default)
    {

        refreshToken.Revoke(ipAddress, reason);

        await _writeRepo.UpdateAsync(refreshToken, ct);
    }

    // ─────────────────────────────────────────────
    // Rotate
    // ─────────────────────────────────────────────
    public async Task<RefreshToken> RotateAsync(
        RefreshToken oldToken,
        string ipAddress,
        CancellationToken ct = default)
    {
        oldToken.Revoke(ipAddress, "Refresh Token Rotated");

        var newRefreshToken =
            Generate(
                oldToken.UserId,
                oldToken.AdminId,
                ipAddress);

        oldToken.Replace(newRefreshToken.Token);

        await _writeRepo.UpdateAsync(oldToken, ct);

        await _writeRepo.AddAsync(newRefreshToken, ct);

        await _unitOfWork.SaveChangesAsync(ct);

        return newRefreshToken;
    }

    public async Task ValidateOwnershipAsync(
    RefreshToken token,
    Guid? currentUserId,
    Guid? currentAdminId)
    {
        var valid =
            (token.UserId.HasValue && token.UserId == currentUserId)
            ||
            (token.AdminId.HasValue && token.AdminId == currentAdminId);

        if (!valid)
            throw new SecurityException("Invalid token ownership");
    }
}