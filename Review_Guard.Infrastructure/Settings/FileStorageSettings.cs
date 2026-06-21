namespace Review_Guard.Infrastructure.Settings;

public class FileStorageSettings
{
    public string Provider { get; set; } = "Local"; // Local | AzureBlob | S3
    public string BasePath { get; set; } = "uploads";
    public string BaseUrl { get; set; } = "https://localhost:7290";
}
public static class FileStorageSettingsTypes
{
    public static readonly string[] AllowedTypes =
        { "application/pdf", "image/jpeg", "image/jpg", "image/png", "image/webp" };

    public static long MaxFileSizeBytes { get; set; } = 10 * 1024 * 1024;
}