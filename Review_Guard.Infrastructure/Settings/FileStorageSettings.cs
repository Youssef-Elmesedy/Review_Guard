namespace Review_Guard.Infrastructure.Settings;

public class FileStorageSettings
{
    public string Provider { get; set; } = "Local"; // Local | AzureBlob | S3
    public string BasePath { get; set; } = "uploads";
    public string BaseUrl { get; set; } = "https://localhost:7290";
    public long MaxFileSizeBytes { get; set; } = 10 * 1024 * 1024; //max 20 MB
}