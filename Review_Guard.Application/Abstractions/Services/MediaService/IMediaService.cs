using Microsoft.AspNetCore.Http;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Domain.Enums;

namespace Review_Guard.Application.Abstractions.Services.MediaService;

/// <summary>
/// Generic media service — single entry point for all image/file operations.
/// Handles Business images, Branch images, UserError profile photos, Proof attachments.
/// Storage details are abstracted behind IFileStorageService.
/// </summary>
public interface IMediaService
{
    /// <summary>
    /// Upload one or more files and attach them to an owner entity.
    /// Returns the created MediaAsset IDs.
    /// </summary>
    Task<Result<IReadOnlyList<MediaUploadResultDto>>> UploadAsync(
        Guid ownerId,
        MediaOwnerType ownerType,
        IReadOnlyList<IFormFile> files,
        CancellationToken ct = default);

    /// <summary>
    /// Set a specific media asset as primary for its owner.
    /// Clears the previous primary automatically.
    /// </summary>
    Task<Result> SetPrimaryAsync(
        Guid ownerId,
        MediaOwnerType ownerType,
        Guid mediaId,
        CancellationToken ct = default);

    /// <summary>
    /// Delete a specific media asset (removes file from storage + DB record).
    /// </summary>
    Task<Result> DeleteAsync(
        Guid ownerId,
        MediaOwnerType ownerType,
        Guid mediaId,
        CancellationToken ct = default);

    /// <summary>
    /// Delete ALL media for an owner (used when deleting the owner entity).
    /// </summary>
    Task<Result> DeleteAllAsync(
        Guid ownerId,
        MediaOwnerType ownerType,
        CancellationToken ct = default);

    /// <summary>
    /// List all media for an owner.
    /// </summary>
    Task<Result<IReadOnlyList<MediaAssetDto>>> GetAllAsync(
        Guid ownerId,
        MediaOwnerType ownerType,
        CancellationToken ct = default);

    /// <summary>
    /// Reorder media assets by providing ordered list of asset IDs.
    /// </summary>
    Task<Result> ReorderAsync(
        Guid ownerId,
        MediaOwnerType ownerType,
        IReadOnlyList<Guid> orderedIds,
        CancellationToken ct = default);
}

// ── DTOs ──────────────────────────────────────────────────────────────

public record MediaUploadResultDto(
    Guid Id,
    string Url,
    bool IsPrimary,
    int SortOrder
);

public record MediaAssetDto(
    Guid Id,
    string Url,
    bool IsPrimary,
    int SortOrder,
    DateTime CreatedAt
);
