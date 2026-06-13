namespace Review_Guard.Infrastructure.Implementation.Repositories.ReportReppository;

internal sealed class ReadReportRepository : GenericReadRepository<Report>, IReadReportRepository
{
    public ReadReportRepository(AppDbContext appDbContext) : base(appDbContext)
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