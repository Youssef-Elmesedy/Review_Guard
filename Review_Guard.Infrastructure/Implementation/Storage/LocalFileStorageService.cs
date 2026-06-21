using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;

namespace Review_Guard.Infrastructure.Implementation.Storage;

public class LocalFileStorageService : IFileStorageService
{
    private readonly FileStorageSettings _settings;
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<LocalFileStorageService> _logger;

    public LocalFileStorageService(
        IOptions<FileStorageSettings> settings, IWebHostEnvironment env,
        ILogger<LocalFileStorageService> logger)
    {
        _settings = settings.Value;
        _env = env;
        _logger = logger;
    }

    public async Task<FileUploadResult> UploadAsync(
        Stream fileStream, string fileName, string contentType, string folder, CancellationToken ct = default)
    {
        try
        {
            var ext = Path.GetExtension(fileName);

            var uniqueName = $"{Guid.NewGuid():N}{ext}";

            var uploadDir = Path.Combine(_env.ContentRootPath, _settings.BasePath, folder);

            Directory.CreateDirectory(uploadDir);

            var fullPath = Path.Combine(uploadDir, uniqueName);

            await using var stream = File.Create(fullPath);

            await fileStream.CopyToAsync(stream, ct);

            var fileSize = new FileInfo(fullPath).Length;

            var url = new Uri(new Uri(_settings.BaseUrl.TrimEnd('/') + "/"), $"{_settings.BasePath}/{folder}/{uniqueName}").ToString();

            _logger.LogInformation("File {FileName} uploaded to {Path}", uniqueName, fullPath);

            return new FileUploadResult(url, uniqueName, fileSize, true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "File upload failed for {FileName}", fileName);

            return new FileUploadResult(string.Empty, fileName, 0, false, ex.Message);
        }
    }

    public Task DeleteAsync(string fileUrl, CancellationToken ct = default)
    {
        try
        {
            var relativePath = fileUrl.Replace(_settings.BaseUrl, string.Empty).TrimStart('/');
            var fullPath = Path.Combine(_env.ContentRootPath, relativePath.Replace('/', Path.DirectorySeparatorChar));
            if (File.Exists(fullPath)) File.Delete(fullPath);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to delete file {Url}", fileUrl);
        }
        return Task.CompletedTask;
    }

    public bool IsAllowedContentType(string? contentType)
    {
        if (string.IsNullOrWhiteSpace(contentType))
            return false;

        var normalized = contentType.ToLowerInvariant();

        return FileStorageSettingsTypes.AllowedTypes
            .Any(x => x.Equals(normalized, StringComparison.OrdinalIgnoreCase));
    }

    public bool IsAllowedFileSize(long fileSize)
    {
        return fileSize > 0 &&
               fileSize <= FileStorageSettingsTypes.MaxFileSizeBytes;
    }
}
