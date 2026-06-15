namespace Review_Guard.Infrastructure.Implementation.Repositories.UserRepository;

internal sealed class ReadUserRepository : GenericReadRepository<User>, IReadUserRepository
{
    private readonly IStringLocalizer<ReadUserRepository> _localizer;
    private readonly ILogger<ReadUserRepository> _logger;

    public ReadUserRepository(AppDbContext appDbContext, IStringLocalizer<ReadUserRepository> localizer, ILogger<ReadUserRepository> logger) : base(appDbContext)
    {
        _localizer = localizer;
        _logger = logger;
    }

    public async Task<User?> GetByIdWithRewardsAsync(
    Guid id,
    CancellationToken cancellationToken = default)
    {
        return await _appDbContext.Users
            .Include(u => u.Rewards)
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<UserReviewStats> GetUserReviewStatsAsync(
    Guid userId,
    CancellationToken cancellationToken = default)
    {

        try
        {
            var userExists = await _appDbContext.Users.AnyAsync(u => u.Id == userId, cancellationToken);

            if (!userExists)
                throw new DomainException(_localizer[DomainMessagies.NotFound],
                    _localizer[DomainMessagies.NotFound]);

            return await _appDbContext.Reviews
                    .Where(r => r.UserId == userId)
                    .GroupBy(_ => 1)
                    .Select(g => new UserReviewStats(
           g.Count(),
           Math.Round((decimal)g.Average(x => x.OverallRating), 2),
           g.Count(x => x.Status == ReviewStatus.Approved),
           g.Count(x => x.Status == ReviewStatus.Rejected)
                    ))
                    .FirstOrDefaultAsync(cancellationToken)
                    ?? new UserReviewStats(0, 0, 0, 0);
        }
        catch (DomainException ex)
        {
            _logger.LogError(ex, "UserError with ID {UserId} not found.", userId);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while fetching review stats for user {UserId}.", userId);
            throw;
        }

    }
}
