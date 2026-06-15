namespace Review_Guard.Application.Feature.UserModul.UserService;

public interface IReadUserService
{
    Task<Result<UserProfileResponse>> GetProfileAsync(
        Guid userId, CancellationToken ct = default);

    Task<Result<PagedResult<UserListItemDto>>> GetAllUsersAsync(
        PaginationParams paging, CancellationToken ct = default);

    Task<Result<PagedResult<UserActivityDto>>> GetUserActivitiesAsync(
        Guid userId, PaginationParams paging, CancellationToken ct = default);
}
