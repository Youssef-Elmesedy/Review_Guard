using Review_Guard.Application.Abstractions.Repositories.GenericRepository;
using Review_Guard.Domain.Entities;
using Review_Guard.Domain.Enums;

namespace Review_Guard.Application.Abstractions.Repositories.MediaRepository;

public interface IReadMediaRepository : IGenericReadRepository<MediaAsset>
{
    /// <summary>Returns all media for a given owner (business, branch, etc.).</summary>
    Task<IReadOnlyList<MediaAsset>> GetByOwnerAsync(
        Guid ownerId, MediaOwnerType ownerType, CancellationToken ct = default);

    /// <summary>Returns the primary image for an owner, or null.</summary>
    Task<MediaAsset?> GetPrimaryAsync(
        Guid ownerId, MediaOwnerType ownerType, CancellationToken ct = default);
}
