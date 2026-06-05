using Review_Guard.Application.Abstractions.Repositories.GenericRepository;
using Review_Guard.Application.Feature.BusinessModul.Dto;
using Review_Guard.Domain.Entities;

namespace Review_Guard.Application.Abstractions.Repositories.BusinessReppository;

public interface IReadBusinessRepository : IGenericReadRepository<Business>
{
    Task<Dictionary<Guid, BusinessRatingDto>> GetBusinessRatingsAsync(
    List<Guid> businessIds,
    CancellationToken ct);

    Task<Dictionary<Guid, BusinessRatingDto>> GetBusinessRatingsByCategoryAsync(
    Guid categoryId,
    CancellationToken ct);

    Task<bool> NameExistsForOwnerAsync(Guid ownerId, string name, CancellationToken ct = default);
}
