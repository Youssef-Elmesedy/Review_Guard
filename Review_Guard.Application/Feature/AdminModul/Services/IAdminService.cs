// FILE: Review_Guard.Application / Feature / AdminModule / Services / IAdminService.cs

using Review_Guard.Application.Feature.AdminModule.DTOs;

namespace Review_Guard.Application.Feature.AdminModule.Services;

public interface IAdminService
{
    Task<Result<AdminProfileResponse>> GetProfileAsync(
        Guid adminId, CancellationToken ct = default);

    Task<Result<AdminDashboardResponse>> GetDashboardAsync(
        Guid adminId, CancellationToken ct = default);

    Task<Result> UpdateProfileAsync(
        Guid adminId, UpdateAdminProfileRequest request, CancellationToken ct = default);

    Task<Result> ChangePasswordAsync(
        Guid adminId, ChangeAdminPasswordRequest request, CancellationToken ct = default);

    Task<Result> BroadcastNotificationAsync(
        Guid adminId, BroadcastNotificationRequest request, CancellationToken ct = default);
}
