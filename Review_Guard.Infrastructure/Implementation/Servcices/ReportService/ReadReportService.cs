using Review_Guard.Application.Feature.ReportModul;
using Review_Guard.Application.Feature.ReportModul.Dto;
using Review_Guard.Application.Feature.ReportModul.Mapping;
using Review_Guard.Application.Feature.ReportModul.Services;
using Review_Guard.Application.Feature.ReportModul.Specification;

namespace Review_Guard.Infrastructure.Implementation.Servcices.ReportService;

internal sealed class ReadReportService : IReadReportService
{
    private readonly IReadReportRepository _repo;
    private readonly ILogger<ReadReportService> _logger;
    private readonly IStringLocalizer<ReadReportService> _localizer;

    public ReadReportService(
        IReadReportRepository repo,
        ILogger<ReadReportService> logger,
        IStringLocalizer<ReadReportService> localizer)
    {
        _repo = repo;
        _logger = logger;
        _localizer = localizer;
    }

    public async Task<Result<ReportResponseDto>> GetByIdAsync(Guid reportId, CancellationToken ct = default)
    {
        try
        {
            var dto = await _repo.ProjectFirstOrDefaultAsync(new ReportByIdSpecification(reportId), ReportProjections.Full, ct);
            if (dto is null)
                return Result<ReportResponseDto>.Failure(
                    AppErrorsCataloge.NotFound(_localizer[ReportMessage.NotFound]));

            return Result<ReportResponseDto>.Success(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching report {ReportId}", reportId);
            return Result<ReportResponseDto>.Failure(
                AppErrorsCataloge.Failure(_localizer[ReportMessage.FetchFailed]));
        }
    }

    public async Task<Result<PagedResult<ReportListItemDto>>> GetAllReportsAsync(
        PaginationParams paging, CancellationToken ct = default)
    {
        try
        {
            var items = await _repo.ProjectAsync(new AllReportsSpecification(paging), ReportProjections.ListItem, ct);
            var total = await _repo.CountAsync(ct: ct);
            return Result<PagedResult<ReportListItemDto>>.Success(
                PagedResult<ReportListItemDto>.Create(items, total, paging.PageNumber, paging.PageSize));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching all reports");
            return Result<PagedResult<ReportListItemDto>>.Failure(
                AppErrorsCataloge.Failure(_localizer[ReportMessage.FetchFailed]));
        }
    }

    public async Task<Result<PagedResult<ReportListItemDto>>> GetOpenReportsAsync(
        PaginationParams paging, CancellationToken ct = default)
    {
        try
        {
            var items = await _repo.ProjectAsync(new OpenReportsSpecification(paging), ReportProjections.ListItem, ct);
            var total = await _repo.CountAsync(r => r.Status == ReportStatus.Open, ct);
            return Result<PagedResult<ReportListItemDto>>.Success(
                PagedResult<ReportListItemDto>.Create(items, total, paging.PageNumber, paging.PageSize));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching open reports");
            return Result<PagedResult<ReportListItemDto>>.Failure(
                AppErrorsCataloge.Failure(_localizer[ReportMessage.FetchFailed]));
        }
    }
}
