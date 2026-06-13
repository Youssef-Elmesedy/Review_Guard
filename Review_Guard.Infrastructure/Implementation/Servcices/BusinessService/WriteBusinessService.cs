using Microsoft.Extensions.Localization;
using Review_Guard.Application.Abstractions.Services.CurrentUserService;
using Review_Guard.Application.Common;
using Review_Guard.Application.Common.CommonMessages;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Application.Feature.Auth;
using Review_Guard.Application.Feature.BusinessModul;
using Review_Guard.Application.Feature.BusinessModul.Dto;
using Review_Guard.Application.Feature.BusinessModul.Services;
using Review_Guard.Domain.Exceptions;
using Review_Guard.Domain.Rules;

namespace Review_Guard.Infrastructure.Implementation.Servcices.BusinessService;

internal sealed class WriteBusinessService : IWriteBusinessService
{
    private readonly IReadBusinessRepository _readBusinessRepository;
    private readonly IWriteBusinessRepository _writeBusinessRepository;
    private readonly IReadBusinessCategoryRepository _categoryRepository;
    private readonly IReadUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;
    private readonly INotificationService _notifications;
    private readonly ICacheService _cache;
    private readonly IStringLocalizer<WriteBusinessService> _stringLocalizer;
    private readonly ILogger<WriteBusinessService> _logger;

    // Centralized cache-prefix invalidation for every list endpoint exposed by BusinessController
    private const string BusinessCachePrefix = "business:";

    public WriteBusinessService(
        IReadBusinessRepository readBusinessRepository,
        IWriteBusinessRepository writeBusinessRepository,
        IReadBusinessCategoryRepository categoryRepository,
        IReadUserRepository userRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser,
        INotificationService notifications,
        ICacheService cache,
        IStringLocalizer<WriteBusinessService> str,
        ILogger<WriteBusinessService> logger)
    {
        _readBusinessRepository = readBusinessRepository;
        _writeBusinessRepository = writeBusinessRepository;
        _categoryRepository = categoryRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _notifications = notifications;
        _cache = cache;
        _stringLocalizer = str;
        _logger = logger;
    }

    public async Task<Result<CreateBusinessResponse>> CreateBusinessAsync(CreateBusinessResponse command, CancellationToken ct = default)
    {
        try
        {
            if (!_currentUser.UserId.HasValue)
                return Result<CreateBusinessResponse>.Failure(AppErrorsCataloge.
                    Unauthorized(_stringLocalizer[CommonMessage.Unauthorized],
                                      _stringLocalizer[CommonMessage.Unauthorized]));

            var userId = _currentUser.UserId.Value;

            var existingBusiness = await _readBusinessRepository.FindFirstAsync(b => b.OwnerId == userId, ct);
            if (existingBusiness is not null)
                return Result<CreateBusinessResponse>.Failure(AppErrorsCataloge.
                    Conflict(_stringLocalizer[BusinessMessage.CreatedBusinessExists, existingBusiness.Name, userId],
                             _stringLocalizer[BusinessMessage.CreatedBusinessExists, existingBusiness.Name, userId]));

            // ── Load & validate owner ──────────────────────────────────────────
            var user = await _userRepository.GetByIdAsync(userId, ct);
            if (user is null)
                return Result<CreateBusinessResponse>.Failure(AppErrorsCataloge.
                    NotFound(_stringLocalizer[AuthMessage.UserNotFound],
                            _stringLocalizer[AuthMessage.UserNotFound]));

            UserBusinessRules.AccountMustBeActive(user);

            var nameExists = await _readBusinessRepository.NameExistsForOwnerAsync(userId, command.Name, ct);
            if (nameExists)
                return Result<CreateBusinessResponse>.Failure(AppErrorsCataloge.
                    Conflict(_stringLocalizer[BusinessMessage.NameExistsForOwnerAsync],
                             _stringLocalizer[BusinessMessage.NameExistsForOwnerAsync]));

            // ── Validate category against BusinessCategory table (NOT Business!) ──
            var categoryExists = await _categoryRepository.AnyAsync(c => c.Id == command.BusinessCategoryId, ct);
            if (!categoryExists)
                return Result<CreateBusinessResponse>.Failure(AppErrorsCataloge.
                    NotFound(_stringLocalizer[BusinessMessage.BusinessCategoryNotFound],
                            _stringLocalizer[BusinessMessage.BusinessCategoryNotFound]));

            // ── Create business ───────────────────────────────────────────────
            var business = Business.Create(
                userId,
                command.Name,
                command.Description,
                command.BusinessCategoryId);

            await _writeBusinessRepository.AddAsync(business, ct);

            await _unitOfWork.SaveChangesAsync(ct);

            var response = new CreateBusinessResponse
            (
                business.Id,
                business.Name,
                business.Description,
                business.OwnerId,
                business.BusinessCategoryId,
                business.Status.ToString()
            );

            await _cache.RemoveByPrefixAsync(BusinessCachePrefix, ct);

            // 🔔 Notify all admins — new business pending approval
            await _notifications.NotifyAllAdminsAsync(
                NotificationType.NewBusinessPending,
                "New business pending approval",
                $"'{business.Name}' was submitted by {user.FullName} and is awaiting review.",
                business.Id.ToString(), "Business", ct);

            return Result<CreateBusinessResponse>.Success(response);
        }
        catch (DomainException ex)
        {
            _logger.LogError(ex, "Domain error occurred while creating business. ErrorCode: {ErrorCode}, MessageKey: {MessageKey}", ex.ErrorCode, ex.MessageKey);
            return Result<CreateBusinessResponse>.Failure(AppErrorsCataloge.Failure
                (
                  ex.ErrorCode,
                  _stringLocalizer[ex.MessageKey]
                ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while creating business.");
            return Result<CreateBusinessResponse>.Failure(AppErrorsCataloge.Failure
                (
                  BusinessMessage.CreateBusinessFailed,
                  _stringLocalizer[BusinessMessage.CreateBusinessFailed]
                ));
        }
    }

    // ── Update ──────────────────────────────────────────────────────────────
    public async Task<Result<UpdateBusinessResponse>> UpdateBusinessAsync(UpdateBusinessResponse command, CancellationToken ct = default)
    {
        try
        {
            if (!_currentUser.UserId.HasValue)
                return Result<UpdateBusinessResponse>.Failure(AppErrorsCataloge.
                    Unauthorized(_stringLocalizer[CommonMessage.Unauthorized], _stringLocalizer[CommonMessage.Unauthorized]));

            var business = await _readBusinessRepository.FindFirstAsync(b => b.OwnerId == _currentUser.UserId.Value, ct);
            if (business is null)
                return Result<UpdateBusinessResponse>.Failure(AppErrorsCataloge.
                    NotFound(BusinessMessage.BusinessNotFound, _stringLocalizer[BusinessMessage.BusinessNotFound]));

            business.UpdateInfo(_currentUser.UserId.Value, command.Name, command.Description);

            await _writeBusinessRepository.UpdateAsync(business, ct);

            await _unitOfWork.SaveChangesAsync(ct);

            await _cache.RemoveByPrefixAsync(BusinessCachePrefix, ct);

            await _cache.RemoveAsync($"business:{business.Id}", ct);

            var response = new UpdateBusinessResponse(
                business.Id,
                business.Name,
                business.Description,
                business.OwnerId,
                business.BusinessCategoryId,
                business.Status);

            return Result<UpdateBusinessResponse>.Success(response);
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Domain error updating business {BusinessId}", command.Id);
            return Result<UpdateBusinessResponse>.Failure(AppErrorsCataloge.Forbidden(ex.ErrorCode, _stringLocalizer[ex.MessageKey]));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error updating business {BusinessId}", command.Id);
            return Result<UpdateBusinessResponse>.Failure(AppErrorsCataloge.Failure(
                BusinessMessage.BusinessUpdateFailed, _stringLocalizer[BusinessMessage.BusinessUpdateFailed]));
        }
    }

    // ── Delete ──────────────────────────────────────────────────────────────
    public async Task<Result<bool>> DeleteBusinessAsync(Guid businessId, CancellationToken ct = default)
    {
        try
        {
            if (!_currentUser.UserId.HasValue)
                return Result<bool>.Failure(AppErrorsCataloge.
                    Unauthorized(_stringLocalizer[CommonMessage.Unauthorized], _stringLocalizer[CommonMessage.Unauthorized]));

            var business = await _readBusinessRepository.GetByIdAsync(businessId, ct);
            if (business is null)
                return Result<bool>.Failure(AppErrorsCataloge.
                    NotFound(BusinessMessage.BusinessNotFound, _stringLocalizer[BusinessMessage.BusinessNotFound]));

            var isAdmin = _currentUser.IsAdmin;
            if (!isAdmin && !business.IsOwnedBy(_currentUser.UserId.Value))
                return Result<bool>.Failure(AppErrorsCataloge.
                    Forbidden(CommonMessage.Forbidden, _stringLocalizer[CommonMessage.Forbidden]));

            // Soft delete: deactivate rather than hard-delete to preserve reviews/branches history
            business.Deactivate();

            await _writeBusinessRepository.UpdateAsync(business, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            await _cache.RemoveByPrefixAsync(BusinessCachePrefix, ct);
            await _cache.RemoveAsync($"business:{business.Id}", ct);

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error deleting business {BusinessId}", businessId);
            return Result<bool>.Failure(AppErrorsCataloge.Failure(
                BusinessMessage.BusinessDeleteFailed, _stringLocalizer[BusinessMessage.BusinessDeleteFailed]));
        }
    }

    // ── Approve (Admin) ────────────────────────────────────────────────────
    public async Task<Result> ApproveBusinessAsync(Guid businessId, string? note, CancellationToken ct = default)
    {
        try
        {
            if (!_currentUser.AdminId.HasValue)
                return Result.Failure(AppErrorsCataloge.Unauthorized(CommonMessage.Unauthorized, _stringLocalizer[CommonMessage.Unauthorized]));

            var business = await _readBusinessRepository.GetByIdAsync(businessId, ct);
            if (business is null)
                return Result.Failure(AppErrorsCataloge.NotFound(BusinessMessage.BusinessNotFound, _stringLocalizer[BusinessMessage.BusinessNotFound]));

            if (business.Status != BusinessStatus.PendingApproval)
                return Result.Failure(AppErrorsCataloge.Conflict(BusinessMessage.BusinessAlreadyProcessed, _stringLocalizer[BusinessMessage.BusinessAlreadyProcessed]));

            business.Approve(_currentUser.AdminId.Value, note);

            await _writeBusinessRepository.UpdateAsync(business, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            await _cache.RemoveByPrefixAsync(BusinessCachePrefix, ct);
            await _cache.RemoveAsync($"business:{business.Id}", ct);

            // 🔔 Notify owner
            await _notifications.NotifyBusinessOwnerAsync(
                business.OwnerId,
                NotificationType.BusinessApproved,
                "Your business was approved",
                $"'{business.Name}' is now live and visible to users.",
                business.Id.ToString(), "Business", ct);

            return Result.Success();
        }
        catch (DomainException ex)
        {
            return Result.Failure(AppErrorsCataloge.Failure(ex.ErrorCode, _stringLocalizer[ex.MessageKey]));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error approving business {BusinessId}", businessId);
            return Result.Failure(AppErrorsCataloge.Failure(BusinessMessage.BusinessApproveFailed, _stringLocalizer[BusinessMessage.BusinessApproveFailed]));
        }
    }

    // ── Reject (Admin) ─────────────────────────────────────────────────────
    public async Task<Result> RejectBusinessAsync(Guid businessId, string reason, CancellationToken ct = default)
    {
        try
        {
            if (!_currentUser.AdminId.HasValue)
                return Result.Failure(AppErrorsCataloge.Unauthorized(CommonMessage.Unauthorized, _stringLocalizer[CommonMessage.Unauthorized]));

            var business = await _readBusinessRepository.GetByIdAsync(businessId, ct);
            if (business is null)
                return Result.Failure(AppErrorsCataloge.NotFound(BusinessMessage.BusinessNotFound, _stringLocalizer[BusinessMessage.BusinessNotFound]));

            if (business.Status != BusinessStatus.PendingApproval)
                return Result.Failure(AppErrorsCataloge.Conflict(BusinessMessage.BusinessAlreadyProcessed, _stringLocalizer[BusinessMessage.BusinessAlreadyProcessed]));

            business.Reject(_currentUser.AdminId.Value, reason);

            await _writeBusinessRepository.UpdateAsync(business, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            await _cache.RemoveByPrefixAsync(BusinessCachePrefix, ct);
            await _cache.RemoveAsync($"business:{business.Id}", ct);

            // 🔔 Notify owner
            await _notifications.NotifyBusinessOwnerAsync(
                business.OwnerId,
                NotificationType.BusinessRejected,
                "Your business was rejected",
                $"'{business.Name}' was rejected. Reason: {reason}",
                business.Id.ToString(), "Business", ct);

            return Result.Success();
        }
        catch (DomainException ex)
        {
            return Result.Failure(AppErrorsCataloge.Failure(ex.ErrorCode, _stringLocalizer[ex.MessageKey]));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rejecting business {BusinessId}", businessId);
            return Result.Failure(AppErrorsCataloge.Failure(BusinessMessage.BusinessRejectFailed, _stringLocalizer[BusinessMessage.BusinessRejectFailed]));
        }
    }
}
