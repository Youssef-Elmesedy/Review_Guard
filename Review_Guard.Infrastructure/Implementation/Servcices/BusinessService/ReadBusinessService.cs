namespace Review_Guard.Infrastructure.Implementation.Servcices.BusinessService;

internal sealed class ReadBusinessService : IReadBusinessService
{
    private readonly IReadBusinessRepository _readBusinessRepository;
    private readonly IReadBranchRepository _readBranchRepository;
    private readonly ICacheService _cache;
    private readonly IStringLocalizer<ReadBusinessService> _localizer;
    private readonly ILogger<ReadBusinessService> _logger;

    public ReadBusinessService(IReadBusinessRepository readBusinessRepository, IReadBranchRepository readBranchRepository, ICacheService cache, IStringLocalizer<ReadBusinessService> localizer, ILogger<ReadBusinessService> logger)
    {
        _readBusinessRepository = readBusinessRepository;
        _readBranchRepository = readBranchRepository;
        _cache = cache;
        _localizer = localizer;
        _logger = logger;
    }

    public async Task<Result<PagedResult<BusinessListtDto>>> GetAllBusiness(PaginationParams paginationParams, CancellationToken ct)
    {
        var cacheKey = $"business:list:{paginationParams.PageNumber}:{paginationParams.PageSize}";
        try
        {
            var cached = await _cache.GetAsync<PagedResult<BusinessListtDto>>(cacheKey, ct);

            if (cached is not null)
                return Result<PagedResult<BusinessListtDto>>.Success(cached);

            var spec = new BusinessListWithBranchsSpecification(paginationParams);

            var businesses = await _readBusinessRepository.ProjectAsync(
                spec,
                BusinessProjections.BusinessList,
                ct);


            var businessIds = businesses.Select(x => x.Id).ToList();

            var reating = await _readBusinessRepository.GetBusinessRatingsAsync(businessIds, ct);

            var result = businesses.Select(b =>
            {
                reating.TryGetValue(b.Id, out var r);

                return b with
                {
                    AverageRating = r?.Avg ?? 0,
                    TotalReviews = r?.Count ?? 0
                };
            }).ToList();

            var total = await _readBusinessRepository.CountAsync();

            var response = PagedResult<BusinessListtDto>.Create(
                result,
                total,
                paginationParams.PageNumber,
                paginationParams.PageSize);

            await _cache.SetAsync(cacheKey, response, TimeSpan.FromMinutes(5), ct);

            return Result<PagedResult<BusinessListtDto>>.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting businesses");
            return Result<PagedResult<BusinessListtDto>>.Failure(
                AppErrorsCataloge.Failure(
                    _localizer[BusinessMessage.BusinessWithReviewsAndBranchsFetchError],
                    _localizer[BusinessMessage.BusinessFetchError]));
        }
    }

    public async Task<Result<BusinessListWithBranchsDto>> GetByBusinessIdWithBranchs(
    Guid businessId,
    CancellationToken ct)
    {
        var cacheKey = $"business:With:Branchs:all:{businessId}";
        try
        {
            var cached = await _cache.GetAsync<BusinessListWithBranchsDto>(cacheKey, ct);

            if (cached is not null)
                return Result<BusinessListWithBranchsDto>.Success(cached);

            var spec = new BusinessWithBranchsSpecification(businessId);

            var businesses = await _readBusinessRepository.ProjectFirstOrDefaultAsync(
                spec,
                BusinessProjections.BusinessWithBranchs,
                ct);

            if (businesses is null)
                return Result<BusinessListWithBranchsDto>.Failure(
                    AppErrorsCataloge.Failure(
                        _localizer[BusinessMessage.BusinessNotFound],
                        _localizer[BusinessMessage.BusinessFetchError]));

            var businessIds = new List<Guid> { businesses.Id };

            var ratings = await _readBusinessRepository
                .GetBusinessRatingsAsync(businessIds, ct);

            ratings.TryGetValue(businesses.Id, out var rating);

            var result = businesses with
            {
                AverageRating = rating?.Avg ?? 0,
                TotalReviews = rating?.Count ?? 0
            };

            await _cache.SetAsync(cacheKey, result, TimeSpan.FromMinutes(5), ct);

            return Result<BusinessListWithBranchsDto>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting businesses With Branchs");

            return Result<BusinessListWithBranchsDto>.Failure(
                AppErrorsCataloge.Failure(
                    _localizer[BusinessMessage.BusinessWithBranchsFetchError],
                    _localizer[BusinessMessage.BusinessFetchError]));
        }
    }

    public async Task<Result<PagedResult<BusinessListWithBranchsDto>>> GetAllBusinessWithBranchsByCategoryAsync(
    Guid categoryId,
    PaginationParams paginationParams,
    CancellationToken ct)
    {
        var cacheKey =
            $"business:With:Branchs:category:{categoryId}:{paginationParams.PageNumber}:{paginationParams.PageSize}";

        try
        {
            var cached = await _cache.GetAsync<PagedResult<BusinessListWithBranchsDto>>(cacheKey, ct);

            if (cached is not null)
                return Result<PagedResult<BusinessListWithBranchsDto>>.Success(cached);

            var spec = new BusinessWithBranchsByCategorySpecification(paginationParams, categoryId);

            var businesses = await _readBusinessRepository.ProjectAsync(
                spec,
                BusinessProjections.BusinessWithBranchs,
                ct);

            var businessIds = businesses.Select(x => x.Id).ToList();

            var ratings = await _readBusinessRepository
                .GetBusinessRatingsAsync(businessIds, ct);

            var result = businesses.Select(b =>
            {
                ratings.TryGetValue(b.Id, out var r);

                return b with
                {
                    AverageRating = r?.Avg ?? 0,
                    TotalReviews = r?.Count ?? 0
                };
            }).ToList();

            var total = await _readBusinessRepository.CountAsync(
                b => b.BusinessCategoryId == categoryId,
                ct);

            var response = PagedResult<BusinessListWithBranchsDto>.Create(
                result,
                total,
                paginationParams.PageNumber,
                paginationParams.PageSize);

            await _cache.SetAsync(cacheKey, response, TimeSpan.FromMinutes(5), ct);

            return Result<PagedResult<BusinessListWithBranchsDto>>.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting businesses With Branchs by category {CategoryId}", categoryId);

            return Result<PagedResult<BusinessListWithBranchsDto>>.Failure(
                AppErrorsCataloge.Failure(
                    _localizer[BusinessMessage.BusinessWithBranchsFetchError],
                    _localizer[BusinessMessage.BusinessFetchError]));
        }
    }

    public async Task<Result<PagedResult<BusinessWithReviewDto>>> GetAllBusinessWithReview(PaginationParams paginationParams, CancellationToken ct)
    {
        var cacheKey = $"business:With:Reviews:{paginationParams.PageNumber}:{paginationParams.PageSize}";

        try
        {
            var cached = await _cache.GetAsync<PagedResult<BusinessWithReviewDto>>(cacheKey, ct);

            if (cached is not null)
                return Result<PagedResult<BusinessWithReviewDto>>.Success(cached);

            var spec = new BusinessListWithBranchsSpecification(paginationParams);

            var businesses = await _readBusinessRepository.ProjectAsync(
                spec,
                BusinessProjections.BusinessDetails,
                ct);

            var businessIds = businesses.Select(x => x.Id).ToList();

            var ratings = await _readBusinessRepository
                .GetBusinessRatingsAsync(businessIds, ct);

            var result = businesses.Select(b =>
            {
                ratings.TryGetValue(b.Id, out var r);

                return b with
                {
                    AverageRating = r?.Avg ?? 0,
                    TotalReviews = r?.Count ?? 0
                };
            }).ToList();

            var total = await _readBusinessRepository.CountAsync();

            var response = PagedResult<BusinessWithReviewDto>.Create(
                result,
                total,
                paginationParams.PageNumber,
                paginationParams.PageSize);

            await _cache.SetAsync(cacheKey, response, TimeSpan.FromMinutes(5), ct);

            return Result<PagedResult<BusinessWithReviewDto>>.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting businesses With Reviews");

            return Result<PagedResult<BusinessWithReviewDto>>.Failure(
                AppErrorsCataloge.Failure(
                    _localizer[BusinessMessage.BusinessWithReviewsFetchError],
                    _localizer[BusinessMessage.BusinessFetchError]));
        }
    }

    public async Task<Result<BranchWithReviewsDto>> GetByBranchIdWithReview(
    Guid branchId,
    CancellationToken ct)
    {
        var cacheKey = $"business:branch:reviews:{branchId}";

        try
        {
            var cached = await _cache.GetAsync<BranchWithReviewsDto>(cacheKey, ct);

            if (cached is not null)
                return Result<BranchWithReviewsDto>.Success(cached);

            var spec = new BranchWithReviewsSpecification(branchId);

            var branch = await _readBranchRepository.ProjectFirstOrDefaultAsync(
                spec,
                BusinessProjections.BrancheWithReviews,
                ct);

            if (branch is null)
            {
                return Result<BranchWithReviewsDto>.Failure(
                    AppErrorsCataloge.NotFound(
                        _localizer[BusinessMessage.BrancheNotFound, branchId],
                        _localizer[BusinessMessage.BusinessFetchError]));
            }

            await _cache.SetAsync(
                cacheKey,
                branch,
                TimeSpan.FromMinutes(10),
                ct);

            return Result<BranchWithReviewsDto>.Success(branch);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting branch reviews {BranchId}", branchId);

            return Result<BranchWithReviewsDto>.Failure(
                AppErrorsCataloge.Failure(
                    _localizer[BusinessMessage.BranchReviewFetchError],
                    _localizer[BusinessMessage.BusinessFetchError]));
        }
    }

    public async Task<Result<PagedResult<BusinessListWithBranchsDto>>> SearchByBusinessName(string businessName, PaginationParams paginationParams, CancellationToken ct = default)
    {
        var cacheKey = $"business:search:With:Reviews:{businessName}:{paginationParams.PageNumber}:{paginationParams.PageSize}";

        try
        {
            var cached = await _cache.GetAsync<PagedResult<BusinessListWithBranchsDto>>(cacheKey, ct);

            if (cached is not null)
                return Result<PagedResult<BusinessListWithBranchsDto>>.Success(cached);

            var spec = new SearchByBusinessNameSpecification(paginationParams, businessName);

            var branches = await _readBusinessRepository.ProjectAsync(
                spec,
                BusinessProjections.BusinessWithBranchs,
                ct);

            if (branches is null || !branches.Any())
            {
                return Result<PagedResult<BusinessListWithBranchsDto>>.Failure(
                    AppErrorsCataloge.Failure(
                        _localizer[BusinessMessage.BusinessNotFound],
                        _localizer[BusinessMessage.BusinessSearchError]));
            }

            var businessIds = branches.Select(x => x.Id).ToList();

            var ratings = await _readBusinessRepository
                .GetBusinessRatingsAsync(businessIds, ct);

            var result = branches.Select(b =>
            {
                ratings.TryGetValue(b.Id, out var r);
                return b with
                {
                    AverageRating = r?.Avg ?? 0,
                    TotalReviews = r?.Count ?? 0
                };
            }).ToList();

            var total = await _readBusinessRepository.CountAsync(
                b => b.Name.Contains(businessName),
                ct);

            var response = PagedResult<BusinessListWithBranchsDto>.Create(result, total, paginationParams.PageNumber, paginationParams.PageSize);

            await _cache.SetAsync(cacheKey, response, TimeSpan.FromMinutes(5), ct);

            return Result<PagedResult<BusinessListWithBranchsDto>>.Success(response);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching businesses by name {BusinessName}", businessName);

            return Result<PagedResult<BusinessListWithBranchsDto>>.Failure(
                AppErrorsCataloge.Failure(
                    _localizer[BusinessMessage.BusinessSearchError],
                    _localizer[BusinessMessage.BusinessFetchError]));
        }
    }
}