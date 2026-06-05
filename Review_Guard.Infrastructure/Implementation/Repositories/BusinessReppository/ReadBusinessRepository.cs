using Review_Guard.Application.Feature.BusinessModul.Dto;

namespace Review_Guard.Infrastructure.Implementation.Repositories.BusinessReppository;

internal sealed class ReadBusinessRepository : GenericReadRepository<Business>, IReadBusinessRepository
{
    public ReadBusinessRepository(AppDbContext appDbContext)
        : base(appDbContext)
    {
    }

    public async Task<Dictionary<Guid, BusinessRatingDto>> GetBusinessRatingsAsync(
        List<Guid> businessIds,
        CancellationToken ct)
    {
        return await _appDbContext.Branches
            .Where(b => businessIds.Contains(b.BusinessId))
            .GroupBy(b => b.BusinessId)
            .Select(g => new
            {
                BusinessId = g.Key,

                // total reviews
                Total = g.Sum(x => x.TotalReviews),

                // correct weighted sum (IMPORTANT FIX)
                WeightedSum = g.Sum(x => x.WeightedAverageRating * x.TotalReviews)
            })
            .Select(x => new BusinessRatingDto(
                x.BusinessId,
                x.Total == 0 ? 0 : x.WeightedSum / x.Total,
                x.Total
            ))
            .ToDictionaryAsync(x => x.BusinessId, ct);
    }

    public async Task<Dictionary<Guid, BusinessRatingDto>> GetBusinessRatingsByCategoryAsync(
        Guid categoryId,
        CancellationToken ct)
    {
        return await _appDbContext.Branches
            .Where(b => b.Business.BusinessCategoryId == categoryId)
            .GroupBy(b => b.BusinessId)
            .Select(g => new
            {
                BusinessId = g.Key,
                Total = g.Sum(x => x.TotalReviews),
                WeightedSum = g.Sum(x => x.WeightedAverageRating * x.TotalReviews)
            })
            .Select(x => new BusinessRatingDto(
                x.BusinessId,
                x.Total == 0 ? 0 : x.WeightedSum / x.Total,
                x.Total
            ))
            .ToDictionaryAsync(x => x.BusinessId, ct);
    }

    public async Task<bool> NameExistsForOwnerAsync(
      Guid ownerId, string name, CancellationToken ct = default)
      => await _appDbContext.Set<Business>()
          .AnyAsync(b => b.OwnerId == ownerId
                      && b.Name == name.Trim(), ct);
}