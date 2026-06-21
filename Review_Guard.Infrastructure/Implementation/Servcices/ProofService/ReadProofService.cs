using Review_Guard.Application.Feature.ProofModul;
using Review_Guard.Application.Feature.ProofModul.Dto;
using Review_Guard.Application.Feature.ProofModul.Mapping;
using Review_Guard.Application.Feature.ProofModul.Services;
using Review_Guard.Application.Feature.ProofModul.Specification;

namespace Review_Guard.Infrastructure.Implementation.Servcices.ProofService;

internal sealed class ReadProofService : IReadProofService
{
    private readonly IReadProofRepository _repo;
    private readonly ICacheService _cache;
    private readonly ILogger<ReadProofService> _logger;
    private readonly IStringLocalizer<ReadProofService> _localizer;

    public ReadProofService(
        IReadProofRepository repo,
        ICacheService cache,
        ILogger<ReadProofService> logger,
        IStringLocalizer<ReadProofService> localizer)
    {
        _repo = repo;
        _cache = cache;
        _logger = logger;
        _localizer = localizer;
    }

    public async Task<Result<ProofResponseDto>> GetByIdAsync(Guid proofId, CancellationToken ct = default)
    {
        try
        {
            var cacheKey = $"proof:{proofId}";
            var cached = await _cache.GetAsync<ProofResponseDto>(cacheKey, ct);
            if (cached is not null) return Result<ProofResponseDto>.Success(cached);

            var dto = await _repo.ProjectFirstOrDefaultAsync(new ProofByIdSpecification(proofId), ProofProjections.Full, ct);
            if (dto is null)
                return Result<ProofResponseDto>.Failure(
                    AppErrorsCataloge.NotFound(_localizer[ProofMessage.NotFound]));

            await _cache.SetAsync(cacheKey, dto, TimeSpan.FromMinutes(10), ct);
            return Result<ProofResponseDto>.Success(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching proof {ProofId}", proofId);
            return Result<ProofResponseDto>.Failure(
                AppErrorsCataloge.Failure(_localizer[ProofMessage.FetchFailed]));
        }
    }

    public async Task<Result<PagedResult<ProofListItemDto>>> GetMyProofsAsync(
        Guid userId, PaginationParams paging, CancellationToken ct = default)
    {
        try
        {
            var items = await _repo.ProjectAsync(new MyProofsSpecification(userId, paging), ProofProjections.ListItem, ct);
            var total = await _repo.CountAsync(p => p.UserId == userId, ct);
            return Result<PagedResult<ProofListItemDto>>.Success(
                PagedResult<ProofListItemDto>.Create(items, total, paging.PageNumber, paging.PageSize));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching proofs for user {UserId}", userId);
            return Result<PagedResult<ProofListItemDto>>.Failure(
                AppErrorsCataloge.Failure(_localizer[ProofMessage.FetchFailed]));
        }
    }

    public async Task<Result<PagedResult<ProofListItemDto>>> GetPendingProofsAsync(
        PaginationParams paging, CancellationToken ct = default)
    {
        try
        {
            var items = await _repo.ProjectAsync(new PendingProofsSpecification(paging), ProofProjections.ListItem, ct);
            var total = await _repo.CountAsync(p => p.Status == ProofStatus.Pending, ct);
            return Result<PagedResult<ProofListItemDto>>.Success(
                PagedResult<ProofListItemDto>.Create(items, total, paging.PageNumber, paging.PageSize));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching pending proofs");
            return Result<PagedResult<ProofListItemDto>>.Failure(
                AppErrorsCataloge.Failure(_localizer[ProofMessage.FetchFailed]));
        }
    }
}
