namespace Review_Guard.Application.Abstractions.Storage;

public interface IFileStorageService
{
    Task<FileUploadResult> UploadAsync(Stream fileStream, string fileName, string contentType, string folder, CancellationToken ct = default);
    Task DeleteAsync(string fileUrl, CancellationToken ct = default);
    bool IsAllowedContentType(string contentType);
}

public record FileUploadResult(string FileUrl, string FileName, long FileSizeBytes, bool IsSuccess, string? Error = null);
