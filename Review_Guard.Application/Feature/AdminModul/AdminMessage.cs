// FILE: Review_Guard.Application / Feature / AdminModule / AdminMessage.cs

namespace Review_Guard.Application.Feature.AdminModule;

public static class AdminMessage
{
    public const string NotFound = "Admin.NotFound";
    public const string ProfileUpdated = "Admin.ProfileUpdated";
    public const string UpdateFailed = "Admin.UpdateFailed";
    public const string DashboardFetchFailed = "Admin.DashboardFetchFailed";
    public const string BroadcastSent = "Admin.BroadcastSent";
    public const string BroadcastFailed = "Admin.BroadcastFailed";
    public const string Unauthorized = "Admin.Unauthorized";
    public const string ChangePasswordFailed = "Admin.ChangePasswordFailed";
    public const string InvalidPassword = "Admin.InvalidPassword";
    public const string PasswordsDoNotMatch = "Admin.PasswordsDoNotMatch";
    public const string NewPasswordSameAsOld = "Admin.NewPasswordSameAsOld";
}
