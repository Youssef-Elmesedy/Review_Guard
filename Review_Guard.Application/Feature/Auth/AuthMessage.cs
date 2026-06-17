namespace Review_Guard.Application.Feature.Auth;

public static class AuthMessage
{
    public const string UserAlreadyExists =
        "Auth.UserAlreadyExists";

    public const string FullNameAlreadyTaken =
        "Auth.FullNameAlreadyTaken";

    public const string InvalidCredentials =
        "Auth.InvalidCredentials";

    public const string EmailNotVerified =
        "Auth.EmailNotVerified";

    public const string RegisterationFailed =
        "Auth.RegisterationFailed";

    public const string LoginFailed =
        "Auth.LoginFailed";

    public const string AdminLoginFailed =
        "Auth.AdminLoginFailed";

    public const string AccountInactive =
        "Auth.AccountInactive";

    public const string InvalidRefreshToken =
        "Auth.InvalidRefreshToken";

    public const string RefreshTokenExpired =
        "Auth.RefreshTokenExpired";

    public const string RefreshTokenFailed =
        "Auth.RefreshTokenFailed";

    public const string LogoutFailed =
        "Auth.LogoutFailed";

    public const string LogoutSuccessfully =
        "Auth.LogoutSuccessfully";

    public const string InvalidVerificationToken =
        "Auth.InvalidVerificationCode";

    public const string VerificationTokenExpired =
        "Auth.VerificationCodeExpired";

    public const string EmailVerificationFailed =
        "Auth.EmailVerificationFailed";

    public const string EmailAlreadyVerified =
        "Auth.EmailAlreadyVerified";

    public const string PasswordResetFailed =
        "Auth.PasswordResetFailed";

    public const string PasswordsDoNotMatch =
        "Auth.PasswordsDoNotMatch";

    public const string ResetPasswordCodeSentSuccessfully =
        "Auth.ResetPasswordCodeSentSuccessfully";

    public const string IfTheEmailExistsAResetCodeWasSent =
        "Auth.IfTheEmailExistsAResetCodeWasSent";

    public const string UpdatePasswordSuccessfully =
        "Auth.UpdatePasswordSuccessfully";

    public const string UserNotFound =
        "Auth.UserNotFound";
}

