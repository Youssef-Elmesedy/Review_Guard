using Review_Guard.Application.Abstractions.Repositories.GenericRepository;
using Review_Guard.Domain.Entities;

namespace Review_Guard.Application.Abstractions.Repositories.ReviewReppository;

public interface IReadReviewRepository : IGenericReadRepository<Review>
{
    Task<List<(decimal Rating, decimal Trust)>> GetApprovedRatingsAsync(Guid branchId, CancellationToken ct);
}
