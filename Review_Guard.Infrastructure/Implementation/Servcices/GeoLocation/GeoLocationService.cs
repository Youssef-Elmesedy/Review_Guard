namespace Review_Guard.Infrastructure.Implementation.Servcices.GeoLocation;

internal sealed class GeoLocationService : IGeoLocationService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<GeoLocationService> _logger;

    public GeoLocationService(
        HttpClient httpClient,
        ILogger<GeoLocationService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<string?> GetLocationAsync(
        string? ipAddress,
        CancellationToken ct = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(ipAddress))
                return null;

            if (ipAddress == "::1" || ipAddress == "127.0.0.1")
                return "Local Development";

            var url = $"http://ip-api.com/json/{ipAddress}";

            _logger.LogInformation(
                "Fetching GeoLocation for IP: {Ip}",
                ipAddress);

            var response = await _httpClient.GetAsync(
                                url,
                                ct);

            var content = await response.Content.ReadAsStringAsync(ct);

            _logger.LogInformation(
                "GeoLocation Response: {Response}",
                content);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning(
                    "GeoLocation API failed with status: {StatusCode}",
                    response.StatusCode);

                return null;
            }

            using var document = JsonDocument.Parse(content);

            var root = document.RootElement;

            var city =
                root.TryGetProperty("city", out var cityProp)
                    ? cityProp.GetString()
                    : null;

            var country =
                root.TryGetProperty("country", out var countryProp)
                    ? countryProp.GetString()
                    : null;

            var region =
                root.TryGetProperty("regionName", out var regionProp)
                    ? regionProp.GetString()
                    : null;

            var isp =
                root.TryGetProperty("isp", out var ispProp)
                    ? ispProp.GetString()
                    : null;


            if (string.IsNullOrWhiteSpace(city) &&
                string.IsNullOrWhiteSpace(region) &&
                string.IsNullOrWhiteSpace(country) &&
                string.IsNullOrWhiteSpace(isp))
            {
                return "Unknown Location";
            }

            return $"{city}, {region}, {country}, {isp}";
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to fetch GeoLocation");

            return null;
        }
    }
}
