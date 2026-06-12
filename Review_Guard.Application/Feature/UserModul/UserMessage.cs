namespace Review_Guard.Application.Feature.UserModul;

public static class UserMessage
{
    // ── Error keys (match ar.json / en.json) ──────
    public const string UserNotFound = "User.NotFound";
    public const string UserFetchError = "User.FetchError";
    public const string UserUpdateError = "User.UpdateError";
    public const string UserAlreadySuspended = "User.AlreadySuspended";
    public const string UserAlreadyBanned = "User.AlreadyBanned";
    public const string UserNotSuspended = "User.NotSuspended";
    public const string UserForbidden = "User.Forbidden";
    public const string PasswordWrong = "User.PasswordWrong";
    public const string PasswordUpdateFailed = "User.PasswordUpdateFailed";
    public const string GetAllUsersFailed = "User.GetAllFailed";
    public const string GetUserActivitiesFailed = "User.GetActivitiesFailed";
    public const string InvalidFileType = "User.InvalidFileType";
    public const string FileTooLarge = "User.FileTooLarge";
    public const string UserIdRequired = "User.IdRequired";
    public const string FileImageRequired = "User.FileImageRequired";
}
