using Microsoft.AspNetCore.Http;
using Review_Guard.Domain.Rules;

namespace Review_Guard.Infrastructure.Implementation.Servcices.UserService;

internal sealed class WriteUserService : IWriteUserService
{
    private readonly IReadUserRepository _readRepo;
    private readonly IWriteUserRepository _writeRepo;
    private readonly IPasswordHasher _hasher;
    private readonly INotificationService _notifications;
    private readonly IFileStorageService _fileStorge;
    private readonly IUnitOfWork _uow;
    private readonly ICacheService _cache;
    private readonly ILogger<WriteUserService> _logger;
    private readonly IStringLocalizer<WriteUserService> _localizer;

    public WriteUserService(
        IReadUserRepository readRepo,
        IWriteUserRepository writeRepo,
        IPasswordHasher hasher,
        INotificationService notifications,
        IUnitOfWork uow,
        ICacheService cache,
        ILogger<WriteUserService> logger,
        IStringLocalizer<WriteUserService> localizer,
        IFileStorageService fileStorge)
    {
        _readRepo = readRepo;
        _writeRepo = writeRepo;
        _hasher = hasher;
        _notifications = notifications;
        _uow = uow;
        _cache = cache;
        _logger = logger;
        _localizer = localizer;
        _fileStorge = fileStorge;
    }

    // ── UpdateProfile ─────────────────────────────────────────────────────
    public async Task<Result<FileUploadResult?>> UpdateProfileImage(
    Guid userId,
    IFormFile fileImage,
    CancellationToken ct = default)
    {
        try
        {
            var user = await _readRepo.GetByIdAsync(userId, ct);

            if (user is null)
                return Result<FileUploadResult?>.Failure(
                    AppErrorsCataloge.NotFound(
                        _localizer[UserMessage.UserNotFound],
                        _localizer[UserMessage.UserNotFound]));

            if (user.Status == AccountStatus.PendingVerification)
                return Result<FileUploadResult?>.Failure(
                    AppErrorsCataloge.Validation(
                        _localizer[AuthMessage.AccountInactive],
                        _localizer[AuthMessage.AccountInactive]));

            if (!_fileStorge.IsAllowedContentType(fileImage.ContentType))
                return Result<FileUploadResult?>.Failure(
                    AppErrorsCataloge.Validation(
                        _localizer[UserMessage.UserUpdateError],
                        _localizer[UserMessage.InvalidFileType, string.Join(", ", FileStorageSettingsTypes.AllowedTypes)]));

            if (!_fileStorge.IsAllowedFileSize(fileImage.Length))
                return Result<FileUploadResult?>.Failure(
                    AppErrorsCataloge.Validation(
                        _localizer[UserMessage.UserUpdateError],
                        _localizer[UserMessage.FileTooLarge, FileStorageSettingsTypes.MaxFileSizeBytes]));

            string? oldImageUrl = user.ProfileImageUrl;
            string? newImageUrl = null;

            await using var stream = fileImage.OpenReadStream();

            var uploadResult = await _fileStorge.UploadAsync(
                stream,
                fileImage.FileName,
                fileImage.ContentType,
                "users",
                ct);

            if (!uploadResult.IsSuccess)
            {
                return Result<FileUploadResult?>.Failure(
                    AppErrorsCataloge.Failure(
                        UserMessage.UserUpdateError,
                        uploadResult.Error ?? _localizer[UserMessage.UserUpdateError]));
            }

            newImageUrl = uploadResult.FileUrl;

            try
            {
                await _uow.ExecuteAsync(async () =>
                {
                    user.UpdateProfileImageUrl(newImageUrl);

                    await _writeRepo.UpdateAsync(user, ct);

                    await _cache.RemoveAsync(
                        $"user:profile:{userId}",
                        ct);
                }, ct);
            }
            catch
            {
                // Database Rollback حصل
                // احذف الملف الجديد لأنه أصبح orphan file
                await _fileStorge.DeleteAsync(newImageUrl, ct);

                throw;
            }

            if (!string.IsNullOrWhiteSpace(oldImageUrl))
            {
                await _fileStorge.DeleteAsync(oldImageUrl, ct);
            }

            // 🔔 Notify user
            await _notifications.NotifyUserAsync(
                userId,
                NotificationType.ProfileUpdated,
                "Profile Image Updated",
                "Your profile image has been updated.",
                userId.ToString(),
                "User",
                ct);

            return Result<FileUploadResult?>.Success(uploadResult);
        }
        catch (DomainException ex)
        {
            return Result<FileUploadResult?>.Failure(
                AppErrorsCataloge.Failure(
                    ex.ErrorCode,
                    _localizer[ex.MessageKey]));
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error updating profile image for {UserId}",
                userId);

            return Result<FileUploadResult?>.Failure(
                AppErrorsCataloge.Failure(
                    UserMessage.UserUpdateError,
                    _localizer[UserMessage.UserUpdateError]));
        }
    }

    public async Task<Result<string>> UpdateProfileAsync(
        Guid userId, UpdateProfileRequest request, CancellationToken ct = default)
    {
        try
        {
            var user = await _readRepo.GetByIdAsync(userId, ct);

            if (user is null)
                return Result<string>.Failure(AppErrorsCataloge.NotFound(
                    UserMessage.UserNotFound, _localizer[UserMessage.UserNotFound]));

            UserBusinessRules.UserMustBeUniqueFullName(user, await _readRepo.AnyAsync(u => u.FullName == request.FullName && u.Id != userId, ct));

            UserBusinessRules.UserMustBeUniquePhone(user, await _readRepo.AnyAsync(u => u.Phone == request.Phone && u.Id != userId, ct));

            var changed = user.UpdateProfile(request.FullName, request.Descriiption, request.Phone);

            if (changed)
            {
                await _writeRepo.UpdateAsync(user, ct);

                await _uow.SaveChangesAsync(ct);

                // Invalidate cache
                await _cache.RemoveAsync($"user:profile:{userId}", ct);

                await _cache.RemoveByPrefixAsync("user:list:", ct);

                return Result<string>.Success(_localizer[UserMessage.UserUpdateSuccess]);
            }

            return Result<string>.Success(_localizer[UserMessage.Norecentdataavailable]);
        }
        catch (DomainException ex)
        {
            return Result<string>.Failure(AppErrorsCataloge.Validation(ex.ErrorCode, _localizer[ex.MessageKey]));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating profile for {UserId}", userId);
            return Result<string>.Failure(AppErrorsCataloge.Failure(
                UserMessage.UserUpdateError, _localizer[UserMessage.UserUpdateError]));
        }
    }

    // ── ChangePassword ────────────────────────────────────────────────────
    public async Task<Result<string>> ChangePasswordAsync(
        Guid userId, ChangePasswordRequest request, CancellationToken ct = default)
    {
        try
        {
            var user = await _readRepo.GetByIdAsync(userId, ct);
            if (user is null)
                return Result<string>.Failure(AppErrorsCataloge.NotFound(
                    UserMessage.UserNotFound, _localizer[UserMessage.UserNotFound]));

            if (!_hasher.VerifyPassword(request.CurrentPassword, user.PasswordHash))
                return Result<string>.Failure(AppErrorsCataloge.Validation(
                    _localizer[UserMessage.PasswordWrong], _localizer[UserMessage.PasswordWrong]));

            if (_hasher.VerifyPassword(request.NewPassword, user.PasswordHash))
                return Result<string>.Failure(
                    AppErrorsCataloge.Validation(
                        _localizer[DomainMessagies.PasswordUnchanged],
                        _localizer[DomainMessagies.PasswordUnchanged]));

            var newHash = _hasher.HashPassword(request.NewPassword);

            user.ChangePassword(newHash);

            await _writeRepo.UpdateAsync(user, ct);

            await _uow.SaveChangesAsync(ct);

            return Result<string>.Success(_localizer[UserMessage.PasswordUpdateSuccess]);
        }
        catch (DomainException ex)
        {
            return Result<string>.Failure(AppErrorsCataloge
                .Validation(_localizer[ex.ErrorCode], _localizer[ex.MessageKey]));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error changing password for {UserId}", userId);
            return Result<string>.Failure(AppErrorsCataloge.Failure(
                UserMessage.PasswordUpdateFailed, _localizer[UserMessage.PasswordUpdateFailed]));
        }
    }

    // ── SuspendUser ───────────────────────────────────────────────────────
    public async Task<Result> SuspendUserAsync(
        Guid adminId, Guid userId, SuspendUserRequest request, CancellationToken ct = default)
    {
        try
        {
            var user = await _readRepo.GetByIdAsync(userId, ct);
            if (user is null)
                return Result.Failure(AppErrorsCataloge.NotFound(
                    UserMessage.UserNotFound, _localizer[UserMessage.UserNotFound]));

            user.Suspend(request.Reason, request.SuspendedUntil);

            await _writeRepo.UpdateAsync(user, ct);

            await _uow.SaveChangesAsync(ct);

            await _cache.RemoveAsync($"user:profile:{userId}", ct);
            await _cache.RemoveByPrefixAsync("user:list:", ct);

            // 🔔 Notify user
            await _notifications.NotifyUserAsync(
                userId,
                NotificationType.AccountSuspended,
                "Account Suspended",
                $"Your account has been suspended. Reason: {request.Reason}",
                userId.ToString(), "User", ct);

            return Result.Success();
        }
        catch (DomainException ex)
        {
            return Result.Failure(AppErrorsCataloge.Validation(ex.ErrorCode, _localizer[ex.MessageKey]));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error suspending user {UserId}", userId);
            return Result.Failure(AppErrorsCataloge.Failure("User.SuspendFailed", "Failed to suspend user."));
        }
    }

    // ── BanUser ───────────────────────────────────────────────────────────
    public async Task<Result> BanUserAsync(
        Guid adminId, Guid userId, BanUserRequest request, CancellationToken ct = default)
    {
        try
        {
            var user = await _readRepo.GetByIdAsync(userId, ct);
            if (user is null)
                return Result.Failure(AppErrorsCataloge.NotFound(
                    UserMessage.UserNotFound, _localizer[UserMessage.UserNotFound]));

            user.Ban(request.Reason);
            await _writeRepo.UpdateAsync(user, ct);
            await _uow.SaveChangesAsync(ct);

            await _cache.RemoveAsync($"user:profile:{userId}", ct);
            await _cache.RemoveByPrefixAsync("user:list:", ct);

            // 🔔 Notify user
            await _notifications.NotifyUserAsync(
                userId,
                NotificationType.AccountBanned,
                "Account Banned",
                $"Your account has been permanently banned. Reason: {request.Reason}",
                userId.ToString(), "User", ct);

            return Result.Success();
        }
        catch (DomainException ex)
        {
            return Result.Failure(AppErrorsCataloge.Validation(ex.ErrorCode, _localizer[ex.MessageKey]));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error banning user {UserId}", userId);
            return Result.Failure(AppErrorsCataloge.Failure("User.BanFailed", "Failed to ban user."));
        }
    }

    // ── ReactivateUser ────────────────────────────────────────────────────
    public async Task<Result> ReactivateUserAsync(
        Guid adminId, Guid userId, CancellationToken ct = default)
    {
        try
        {
            var user = await _readRepo.GetByIdAsync(userId, ct);
            if (user is null)
                return Result.Failure(AppErrorsCataloge.NotFound(
                    UserMessage.UserNotFound, _localizer[UserMessage.UserNotFound]));

            user.Reactivate();
            await _writeRepo.UpdateAsync(user, ct);
            await _uow.SaveChangesAsync(ct);

            await _cache.RemoveAsync($"user:profile:{userId}", ct);
            await _cache.RemoveByPrefixAsync("user:list:", ct);

            // 🔔 Notify user
            await _notifications.NotifyUserAsync(
                userId,
                NotificationType.AccountReactivated,
                "Account Reactivated",
                "Your account has been reactivated. Welcome back!",
                userId.ToString(), "User", ct);

            return Result.Success();
        }
        catch (DomainException ex)
        {
            return Result.Failure(AppErrorsCataloge.Validation(ex.ErrorCode, _localizer[ex.MessageKey]));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reactivating user {UserId}", userId);
            return Result.Failure(AppErrorsCataloge.Failure("User.ReactivateFailed", "Failed to reactivate user."));
        }
    }
}
