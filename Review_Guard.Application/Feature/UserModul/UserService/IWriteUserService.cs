using Microsoft.AspNetCore.Http;
using Review_Guard.Application.Abstractions.Storage;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Application.Feature.UserModul.Dto;

namespace Review_Guard.Application.Feature.UserModul.UserService;

public interface IWriteUserService
{
    Task<Result<FileUploadResult?>> UpdateProfileImage(Guid userId, IFormFile ImageFile, CancellationToken ct = default);

    Task<Result> UpdateProfileAsync(
        Guid userId, UpdateProfileRequest request, CancellationToken ct = default);

    Task<Result> ChangePasswordAsync(
        Guid userId, ChangePasswordRequest request, CancellationToken ct = default);

    Task<Result> SuspendUserAsync(
        Guid adminId, Guid userId, SuspendUserRequest request, CancellationToken ct = default);

    Task<Result> BanUserAsync(
        Guid adminId, Guid userId, BanUserRequest request, CancellationToken ct = default);

    Task<Result> ReactivateUserAsync(
        Guid adminId, Guid userId, CancellationToken ct = default);
}
