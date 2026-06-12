using Microsoft.Extensions.Localization;
using Review_Guard.Application.Common;
using Review_Guard.Application.Common.CommonMessages;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Application.Feature.ProofModul;
using Review_Guard.Application.Feature.ProofModul.Dto;
using Review_Guard.Application.Feature.ProofModul.Mapping;
using Review_Guard.Application.Feature.ProofModul.Services;
using Review_Guard.Application.Feature.ProofModul.Specification;
using Review_Guard.Domain.Enums;
using Review_Guard.Domain.Exceptions;

namespace Review_Guard.Infrastructure.Implementation.Servcices.ProofService;

internal sealed class WriteProofService : IWriteProofService
{
    private readonly IReadProofRepository  _readRepo;
    private readonly IWriteProofRepository _writeRepo;
    private readonly IReadBranchRepository _branchRepo;
    private readonly INotificationService  _notifications;
    private readonly IUnitOfWork           _uow;
    private readonly ICacheService         _cache;
    private readonly ILogger<WriteProofService> _logger;
    private readonly IStringLocalizer<WriteProofService> _localizer;

    public WriteProofService(
        IReadProofRepository readRepo,
        IWriteProofRepository writeRepo,
        IReadBranchRepository branchRepo,
        INotificationService notifications,
        IUnitOfWork uow,
        ICacheService cache,
        ILogger<WriteProofService> logger,
        IStringLocalizer<WriteProofService> localizer)
    {
        _readRepo = readRepo;
        _writeRepo = writeRepo;
        _branchRepo = branchRepo;
        _notifications = notifications;
        _uow = uow;
        _cache = cache;
        _logger = logger;
        _localizer = localizer;
    }

    public async Task<Result<ProofResponseDto>> SubmitByFileAsync(
        Guid userId, SubmitProofByFileRequest request, CancellationToken ct = default)
    {
        try
        {
            var branch = await _branchRepo.GetByIdAsync(request.BranchId, ct);
            if (branch is null)
                return Result<ProofResponseDto>.Failure(
                    AppErrorsCataloge.NotFound(DomainMessagies.BranchNotFound, _localizer[DomainMessagies.BranchNotFound]));

            var proof = Proof.CreateFromFile(userId, request.BranchId, request.FileUrl);
            await _writeRepo.AddAsync(proof, ct);
            await _uow.SaveChangesAsync(ct);

            // 🔔 Notify all admins
            await _notifications.NotifyAllAdminsAsync(
                NotificationType.NewProofPending,
                "New proof pending",
                "A user submitted a proof file for verification.",
                proof.Id.ToString(), "Proof", ct);

            var dto = await _readRepo.ProjectFirstOrDefaultAsync(new ProofByIdSpecification(proof.Id), ProofProjections.Full, ct);
            return Result<ProofResponseDto>.Success(dto!);
        }
        catch (DomainException ex)
        {
            return Result<ProofResponseDto>.Failure(AppErrorsCataloge.Failure(ex.ErrorCode, _localizer[ex.MessageKey]));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error submitting proof by file for user {UserId}", userId);
            return Result<ProofResponseDto>.Failure(
                AppErrorsCataloge.Failure(ProofMessage.CreateFailed, _localizer[ProofMessage.CreateFailed]));
        }
    }

    public async Task<Result<ProofResponseDto>> SubmitByOrderAsync(
        Guid userId, SubmitProofByOrderRequest request, CancellationToken ct = default)
    {
        try
        {
            var branch = await _branchRepo.GetByIdAsync(request.BranchId, ct);
            if (branch is null)
                return Result<ProofResponseDto>.Failure(
                    AppErrorsCataloge.NotFound(DomainMessagies.BranchNotFound, _localizer[DomainMessagies.BranchNotFound]));

            var proof = Proof.CreateFromOrder(userId, request.BranchId, request.OrderId);
            await _writeRepo.AddAsync(proof, ct);
            await _uow.SaveChangesAsync(ct);

            // 🔔 Notify all admins
            await _notifications.NotifyAllAdminsAsync(
                NotificationType.NewProofPending,
                "New proof pending",
                $"A user submitted order proof '{request.OrderId}' for verification.",
                proof.Id.ToString(), "Proof", ct);

            var dto = await _readRepo.ProjectFirstOrDefaultAsync(new ProofByIdSpecification(proof.Id), ProofProjections.Full, ct);
            return Result<ProofResponseDto>.Success(dto!);
        }
        catch (DomainException ex)
        {
            return Result<ProofResponseDto>.Failure(AppErrorsCataloge.Failure(ex.ErrorCode, _localizer[ex.MessageKey]));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error submitting proof by order for user {UserId}", userId);
            return Result<ProofResponseDto>.Failure(
                AppErrorsCataloge.Failure(ProofMessage.CreateFailed, _localizer[ProofMessage.CreateFailed]));
        }
    }

    public async Task<Result> VerifyAsync(
        Guid adminId, Guid proofId, AdminProofActionRequest request, CancellationToken ct = default)
    {
        try
        {
            var proof = await _readRepo.GetByIdAsync(proofId, ct);
            if (proof is null)
                return Result.Failure(AppErrorsCataloge.NotFound(ProofMessage.NotFound, _localizer[ProofMessage.NotFound]));

            proof.Verify(adminId, request.Note);
            await _writeRepo.UpdateAsync(proof, ct);
            await _uow.SaveChangesAsync(ct);
            await _cache.RemoveAsync($"proof:{proofId}", ct);

            // 🔔 Notify user
            await _notifications.NotifyUserAsync(
                proof.UserId,
                NotificationType.ProofVerified,
                "Your proof was verified",
                "Your proof of visit has been verified. You can now submit a review.",
                proof.Id.ToString(), "Proof", ct);

            return Result.Success();
        }
        catch (DomainException ex)
        {
            return Result.Failure(AppErrorsCataloge.Failure(ex.ErrorCode, _localizer[ex.MessageKey]));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying proof {ProofId}", proofId);
            return Result.Failure(AppErrorsCataloge.Failure(ProofMessage.VerifyFailed, _localizer[ProofMessage.VerifyFailed]));
        }
    }

    public async Task<Result> RejectAsync(
        Guid adminId, Guid proofId, AdminProofActionRequest request, CancellationToken ct = default)
    {
        try
        {
            var proof = await _readRepo.GetByIdAsync(proofId, ct);
            if (proof is null)
                return Result.Failure(AppErrorsCataloge.NotFound(ProofMessage.NotFound, _localizer[ProofMessage.NotFound]));

            var reason = request.Note ?? string.Empty;
            proof.Reject(adminId, reason);
            await _writeRepo.UpdateAsync(proof, ct);
            await _uow.SaveChangesAsync(ct);
            await _cache.RemoveAsync($"proof:{proofId}", ct);

            // 🔔 Notify user
            await _notifications.NotifyUserAsync(
                proof.UserId,
                NotificationType.ProofRejected,
                "Your proof was rejected",
                $"Your proof was rejected. Reason: {reason}",
                proof.Id.ToString(), "Proof", ct);

            return Result.Success();
        }
        catch (DomainException ex)
        {
            return Result.Failure(AppErrorsCataloge.Failure(ex.ErrorCode, _localizer[ex.MessageKey]));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rejecting proof {ProofId}", proofId);
            return Result.Failure(AppErrorsCataloge.Failure(ProofMessage.RejectFailed, _localizer[ProofMessage.RejectFailed]));
        }
    }
}
