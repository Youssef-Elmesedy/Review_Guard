namespace Review_Guard.Application.Abstractions.Services.GeoLocation;

public interface IGeoLocationService
{
    Task<string?> GetLocationAsync(
        string? ipAddress,
        CancellationToken ct = default);
}
