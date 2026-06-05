using Review_Guard.Application.Abstractions.Repositories.MediaRepository;
using Review_Guard.Domain.Enums;

namespace Review_Guard.Infrastructure.Implementation.Repositories.MediaRepository;

internal sealed class ReadMediaRepository : GenericReadRepository<MediaAsset>, IReadMediaRepository
{
    public ReadMediaRepository(AppDbContext appDbContext) : base(appDbContext) { }

    public async Task<IReadOnlyList<MediaAsset>> GetByOwnerAsync(
        Guid ownerId, MediaOwnerType ownerType, CancellationToken ct = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(m => m.OwnerType == ownerType && (
                (ownerType == MediaOwnerType.Business && m.BusinessId == ownerId) ||
                (ownerType == MediaOwnerType.Branch && m.BranchId == ownerId) ||
                (ownerType == MediaOwnerType.User && m.UserId == ownerId) ||
                (ownerType == MediaOwnerType.Proof && m.ProofId == ownerId)
            ))
            .OrderBy(m => m.SortOrder)
            .ThenBy(m => m.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task<MediaAsset?> GetPrimaryAsync(
        Guid ownerId, MediaOwnerType ownerType, CancellationToken ct = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(m => m.IsPrimary && m.OwnerType == ownerType && (
                (ownerType == MediaOwnerType.Business && m.BusinessId == ownerId) ||
                (ownerType == MediaOwnerType.Branch && m.BranchId == ownerId) ||
                (ownerType == MediaOwnerType.User && m.UserId == ownerId) ||
                (ownerType == MediaOwnerType.Proof && m.ProofId == ownerId)
            ))
            .FirstOrDefaultAsync(ct);
    }
}
