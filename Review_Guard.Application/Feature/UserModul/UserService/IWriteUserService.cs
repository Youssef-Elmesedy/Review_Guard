using Microsoft.AspNetCore.Http;
using Review_Guard.Application.Abstractions.Storage;

namespace Review_Guard.Application.Feature.UserModul.UserService;

public interface IWriteUserService
{
    Task<Result<FileUploadResult?>> UpdateProfileImage(Guid userId, IFormFile ImageFile, CancellationToken ct = default);

    Task<Result<string>> UpdateProfileAsync(
        Guid userId, UpdateProfileRequest request, CancellationToken ct = default);

    Task<Result<string>> ChangePasswordAsync(
        Guid userId, ChangePasswordRequest request, CancellationToken ct = default);

    Task<Result> SuspendUserAsync(
        Guid adminId, Guid userId, SuspendUserRequest request, CancellationToken ct = default);

    Task<Result> BanUserAsync(
        Guid adminId, Guid userId, BanUserRequest request, CancellationToken ct = default);

    Task<Result> ReactivateUserAsync(
        Guid adminId, Guid userId, CancellationToken ct = default);
}
