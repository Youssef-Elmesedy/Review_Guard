using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace Review_Guard.Infrastructure.Implementation.Email;

public class EmailService : IEmailService
{
    private readonly SmtpSettings _settings;
    private readonly ILogger<EmailService> _logger;
    private readonly IEmailTemplateRenderer _renderer;

    public EmailService(
        IOptions<SmtpSettings> settings,
        ILogger<EmailService> logger,
        IEmailTemplateRenderer renderer)
    {
        _settings = settings.Value;
        _logger = logger;
        _renderer = renderer;
    }

    public async Task SendEmailConfirmationAsync(
    string toEmail,
    string toName,
    string verificationCode,
    CancellationToken ct = default)
    {
        var body =
            await _renderer.RenderAsync(
                "Auth/ConfirmEmail",
                new Dictionary<string, string>
                {
                    ["name"] = toName,
                    ["code"] = verificationCode
                });

        await SendAsync(
            toEmail,
            toName,
            "Verify Your Email Address",
            body,
            ct);
    }

    public async Task SendWelcomeEmailAsync(
        string toEmail, string toName, CancellationToken ct = default)
    {
        var body = await _renderer.RenderAsync("User/Welcome", new Dictionary<string, string>
        {
            ["name"] = toName
        });

        await SendAsync(toEmail, toName, "Welcome to VerifiedReviews!", body, ct);
    }

    public async Task SendReviewStatusUpdateAsync(
        string toEmail, string toName, string businessName, string status,
        string? reason, CancellationToken ct = default)
    {
        var reasonBlock = string.IsNullOrEmpty(reason)
            ? ""
            : $"<div class='alert'><strong>Reason:</strong> {reason}</div>";

        var statusMessage = status == "Approved"
            ? "<p>Your TrustScore increased! 🏆</p>"
            : "<p>If you believe this is wrong, contact support.</p>";

        var body = await _renderer.RenderAsync("Review/ReviewStatus", new Dictionary<string, string>
        {
            ["name"] = toName,
            ["businessName"] = businessName,
            ["status"] = status,
            ["reasonBlock"] = reasonBlock,
            ["statusMessage"] = statusMessage
        });

        var subject = $"Your Review Has Been {status}";
        await SendAsync(toEmail, toName, subject, body, ct);
    }

    public async Task SendReviewReceivedAsync(
        string toEmail, string toName, string businessName, CancellationToken ct = default)
    {
        var body = await _renderer.RenderAsync("Review/ReviewReceived", new Dictionary<string, string>
        {
            ["ownerName"] = toName,
            ["businessName"] = businessName
        });

        await SendAsync(toEmail, toName, $"New Review Submitted for {businessName}", body, ct);
    }

    public async Task SendAccountSuspendedAsync(
        string toEmail, string toName, string reason, CancellationToken ct = default)
    {
        var body = await _renderer.RenderAsync("User/AccountSuspended", new Dictionary<string, string>
        {
            ["name"] = toName,
            ["reason"] = reason
        });

        await SendAsync(toEmail, toName, "Important Notice About Your Account", body, ct);
    }

    public async Task SendPasswordResetAsync(
        string toEmail, string toName, string code, CancellationToken ct = default)
    {
        var body = await _renderer.RenderAsync("Auth/ResetPassword", new Dictionary<string, string>
        {
            ["name"] = toName,
            ["code"] = code
        });

        await SendAsync(toEmail, toName, "Reset Your Password", body, ct);
    }

    public async Task SendGenericNotificationAsync(
        string toEmail, string toName, string subject, string htmlBody, CancellationToken ct = default)
    {
        await SendAsync(toEmail, toName, subject, htmlBody, ct);
    }

    // ── Core Sender ─────────────────────────────────────────────
    private async Task SendAsync(
        string toEmail,
        string toName,
        string subject,
        string body,
        CancellationToken ct)
    {
        try
        {
            using var client = new SmtpClient(_settings.Host, _settings.Port)
            {
                Credentials = new NetworkCredential(_settings.Username, _settings.Password),
                EnableSsl = _settings.EnableSsl,

                DeliveryMethod = SmtpDeliveryMethod.Network,
                Timeout = 10000
            };

            client.UseDefaultCredentials = false;

            client.Credentials = new NetworkCredential(
                _settings.Username,
                _settings.Password);

            client.EnableSsl = true;

            var message = new MailMessage
            {
                From = new MailAddress(_settings.FromEmail, _settings.FromName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            message.To.Add(new MailAddress(toEmail, toName));

            await client.SendMailAsync(message, ct);

            _logger.LogInformation("Email '{Subject}' sent to {Email}", subject, toEmail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email '{Subject}' to {Email}", subject, toEmail);
            throw;
        }
    }
}
