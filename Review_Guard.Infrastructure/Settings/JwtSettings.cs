namespace Review_Guard.Infrastructure.Settings;

public class JwtSettings
{
    public string Secret { get; set; } = default!;
    public string Issuer { get; set; } = "VerifiedReviews";
    public string Audience { get; set; } = "VerifiedReviewsUsers";
    public int AccessTokenExpiryMinutes { get; set; } = 60; // 1 hour
    public int AdminAccessTokenExpiryMinutes { get; set; } = 30;  // 30 minutes
}
