using Microsoft.Extensions.Localization;
using Review_Guard.Application.Common;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Application.Feature.ReportModul;
using Review_Guard.Application.Feature.ReportModul.Dto;
using Review_Guard.Application.Feature.ReportModul.Mapping;
using Review_Guard.Application.Feature.ReportModul.Services;
using Review_Guard.Application.Feature.ReportModul.Specification;
using Review_Guard.Domain.Enums;
using Review_Guard.Domain.Exceptions;

namespace Review_Guard.Infrastructure.Implementation.Servcices.ReportService;

internal sealed class WriteReportService : IWriteReportService
{
    private readonly IReadReportRepository  _readRepo;
    private readonly IWriteReportRepository _writeRepo;
    private readonly IReadReviewRepository  _reviewRepo;
    private readonly INotificationService   _notifications;
    private readonly IUnitOfWork            _uow;
    private readonly ILogger<WriteReportService> _logger;
    private readonly IStringLocalizer<WriteReportService> _localizer;

    public WriteReportService(
        IReadReportRepository readRepo,
        IWriteReportRepository writeRepo,
        IReadReviewRepository reviewRepo,
        INotificationService notifications,
        IUnitOfWork uow,
        ILogger<WriteReportService> logger,
        IStringLocalizer<WriteReportService> localizer)
    {
        _readRepo = readRepo;
        _writeRepo = writeRepo;
        _reviewRepo = reviewRepo;
        _notifications = notifications;
        _uow = uow;
        _logger = logger;
        _localizer = localizer;
    }

    public async Task<Result<ReportResponseDto>> CreateAsync(
        Guid userId, CreateReportRequest request, CancellationToken ct = default)
    {
        try
        {
            var review = await _reviewRepo.GetByIdAsync(request.ReviewId, ct);
            if (review is null)
                return Result<ReportResponseDto>.Failure(
                    AppErrorsCataloge.NotFound(Review_Guard.Application.Feature.ReviewModul.ReviewMessage.NotFound,
                        _localizer[Review_Guard.Application.Feature.ReviewModul.ReviewMessage.NotFound]));

            var existing = await _readRepo.FindFirstAsync(
                r => r.ReportedByUserId == userId && r.ReviewId == request.ReviewId, ct);
            if (existing is not null)
                return Result<ReportResponseDto>.Failure(
                    AppErrorsCataloge.Conflict(ReportMessage.AlreadyReported, _localizer[ReportMessage.AlreadyReported]));

            var report = Report.Create(userId, request.ReviewId, request.Reason, request.Description);
            await _writeRepo.AddAsync(report, ct);
            await _uow.SaveChangesAsync(ct);

            // 🔔 Notify all admins
            await _notifications.NotifyAllAdminsAsync(
                NotificationType.NewReportPending,
                "New report submitted",
                $"A user reported a review. Reason: {request.Reason}",
                report.Id.ToString(), "Report", ct);

            var dto = await _readRepo.ProjectFirstOrDefaultAsync(new ReportByIdSpecification(report.Id), ReportProjections.Full, ct);
            return Result<ReportResponseDto>.Success(dto!);
        }
        catch (DomainException ex)
        {
            return Result<ReportResponseDto>.Failure(AppErrorsCataloge.Failure(ex.ErrorCode, _localizer[ex.MessageKey]));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating report for user {UserId}", userId);
            return Result<ReportResponseDto>.Failure(
                AppErrorsCataloge.Failure(ReportMessage.CreateFailed, _localizer[ReportMessage.CreateFailed]));
        }
    }

    public async Task<Result> ResolveAsync(
        Guid adminId, Guid reportId, AdminReportActionRequest request, CancellationToken ct = default)
    {
        try
        {
            var report = await _readRepo.GetByIdAsync(reportId, ct);
            if (report is null)
                return Result.Failure(AppErrorsCataloge.NotFound(ReportMessage.NotFound, _localizer[ReportMessage.NotFound]));

            report.Resolve(adminId, request.Note);
            await _writeRepo.UpdateAsync(report, ct);
            await _uow.SaveChangesAsync(ct);
            return Result.Success();
        }
        catch (DomainException ex)
        {
            return Result.Failure(AppErrorsCataloge.Failure(ex.ErrorCode, _localizer[ex.MessageKey]));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resolving report {ReportId}", reportId);
            return Result.Failure(AppErrorsCataloge.Failure(ReportMessage.ResolveFailed, _localizer[ReportMessage.ResolveFailed]));
        }
    }

    public async Task<Result> DismissAsync(
        Guid adminId, Guid reportId, AdminReportActionRequest request, CancellationToken ct = default)
    {
        try
        {
            var report = await _readRepo.GetByIdAsync(reportId, ct);
            if (report is null)
                return Result.Failure(AppErrorsCataloge.NotFound(ReportMessage.NotFound, _localizer[ReportMessage.NotFound]));

            report.Dismiss(adminId, request.Note);
            await _writeRepo.UpdateAsync(report, ct);
            await _uow.SaveChangesAsync(ct);
            return Result.Success();
        }
        catch (DomainException ex)
        {
            return Result.Failure(AppErrorsCataloge.Failure(ex.ErrorCode, _localizer[ex.MessageKey]));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error dismissing report {ReportId}", reportId);
            return Result.Failure(AppErrorsCataloge.Failure(ReportMessage.DismissFailed, _localizer[ReportMessage.DismissFailed]));
        }
    }
}
