namespace Review_Guard.Infrastructure.Implementation.Repositories.ReviewReppository;

internal sealed class ReadReviewRepository : GenericReadRepository<Review>, IReadReviewRepository
{
    public ReadReviewRepository(AppDbContext appDbContext) : base(appDbContext)
    {
    }

    public Task<List<(decimal Rating, decimal Trust)>> GetApprovedRatingsAsync(Guid branchId, CancellationToken ct)
    {
        return _appDbContext.Reviews
            .Where(r => r.BranchId == branchId && r.Status == ReviewStatus.Approved)
            .Select(r => new ValueTuple<decimal, decimal>(
                r.OverallRating,
                r.User.TrustScoreValue
            ))
            .ToListAsync(ct);
    }
}
