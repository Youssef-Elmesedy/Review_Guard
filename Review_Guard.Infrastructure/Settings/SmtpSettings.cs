namespace Review_Guard.Infrastructure.Settings;

public class SmtpSettings
{
    public string Host { get; set; } = "smtp.gmail.com";
    public int Port { get; set; } = 587;
    public string Username { get; set; } = default!;
    public string Password { get; set; } = default!;
    public bool EnableSsl { get; set; } = true;
    public string FromEmail { get; set; } = default!;
    public string FromName { get; set; } = "VerifiedReviews";
}