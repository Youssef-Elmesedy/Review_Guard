namespace Review_Guard.Application.Abstractions.Email;

public interface IEmailService
{
    Task SendEmailConfirmationAsync(
    string toEmail,
    string toName,
    string verificationCode,
    CancellationToken ct = default);

    Task SendWelcomeEmailAsync(string toEmail, string toName, CancellationToken ct = default);
    Task SendReviewStatusUpdateAsync
        (string toEmail, string toName, string businessName, string status, string? reason, CancellationToken ct = default);
    Task SendReviewReceivedAsync
        (string toEmail, string toName, string businessName, CancellationToken ct = default);
    Task SendAccountSuspendedAsync
        (string toEmail, string toName, string reason, CancellationToken ct = default);
    Task SendPasswordResetAsync
        (string toEmail, string toName, string resetToken, CancellationToken ct = default);
    Task SendGenericNotificationAsync
            (string toEmail, string toName, string subject, string htmlBody, CancellationToken ct = default);
}
