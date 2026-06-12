using Microsoft.AspNetCore.Http;
using Review_Guard.Application.Abstractions.Repositories.MediaRepository;
using Review_Guard.Application.Abstractions.Services.MediaService;
using Review_Guard.Application.Common;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Domain.Enums;

namespace Review_Guard.Infrastructure.Implementation.Servcices.MediaService;

/// <summary>
/// The single brain for all media operations.
/// Handles Business, Branch, UserError profile, and Proof attachments generically.
/// </summary>
internal sealed class MediaService : IMediaService
{
    private readonly IReadMediaRepository _readRepo;
    private readonly IWriteMediaRepository _writeRepo;
    private readonly IFileStorageService _storage;
    private readonly IUnitOfWork _uow;
    private readonly ILogger<MediaService> _logger;

    // Max files per upload and per owner
    private const int MaxFilesPerUpload = 10;
    private const int MaxFilesPerOwner = 20;

    // Folders mapped by owner type (used as sub-folder in storage)
    private static readonly Dictionary<MediaOwnerType, string> FolderMap = new()
    {
        { MediaOwnerType.Business, "businesses" },
        { MediaOwnerType.Branch,   "branches"   },
        { MediaOwnerType.User,     "users"       },
        { MediaOwnerType.Proof,    "proofs"      },
    };

    public MediaService(
        IReadMediaRepository readRepo,
        IWriteMediaRepository writeRepo,
        IFileStorageService storage,
        IUnitOfWork uow,
        ILogger<MediaService> logger)
    {
        _readRepo = readRepo;
        _writeRepo = writeRepo;
        _storage = storage;
        _uow = uow;
        _logger = logger;
    }

    // ────────────────────────────────────────────────────────────────────
    // UPLOAD
    // ────────────────────────────────────────────────────────────────────

    public async Task<Result<IReadOnlyList<MediaUploadResultDto>>> UploadAsync(
        Guid ownerId,
        MediaOwnerType ownerType,
        IReadOnlyList<IFormFile> files,
        CancellationToken ct = default)
    {
        if (files.Count == 0)
            return Result<IReadOnlyList<MediaUploadResultDto>>.Failure(
                AppErrorCodes.Media.NoFilesProvided);

        if (files.Count > MaxFilesPerUpload)
            return Result<IReadOnlyList<MediaUploadResultDto>>.Failure(
                AppErrorCodes.Media.TooManyFiles(MaxFilesPerUpload));

        // Validate content types upfront
        foreach (var file in files)
        {
            if (!_storage.IsAllowedContentType(file.ContentType))
                return Result<IReadOnlyList<MediaUploadResultDto>>.Failure(
                    AppErrorCodes.Media.InvalidContentType(file.ContentType));
        }

        // Check owner won't exceed max
        var existing = await _readRepo.GetByOwnerAsync(ownerId, ownerType, ct);
        if (existing.Count + files.Count > MaxFilesPerOwner)
            return Result<IReadOnlyList<MediaUploadResultDto>>.Failure(
                AppErrorCodes.Media.MaxFilesExceeded(MaxFilesPerOwner));

        var folder = FolderMap[ownerType];

        var results = new List<MediaUploadResultDto>();

        var uploadedUrls = new List<string>(); // for rollback on partial failure

        // Determine starting sort order
        var nextSortOrder = existing.Count > 0
            ? existing.Max(m => m.SortOrder) + 1
            : 0;

        // For UserError profile: only 1 allowed, replace existing
        if (ownerType == MediaOwnerType.User)
        {
            if (files.Count > 1)
                return Result<IReadOnlyList<MediaUploadResultDto>>.Failure(
                    AppErrorCodes.Media.ProfileImageSingleOnly);

            await DeleteAllInternalAsync(existing, ct);
        }

        try
        {
            for (int i = 0; i < files.Count; i++)
            {
                var file = files[i];
                await using var stream = file.OpenReadStream();

                var uploadResult = await _storage.UploadAsync(
                    stream, file.FileName, file.ContentType, folder, ct);

                if (!uploadResult.IsSuccess)
                {
                    _logger.LogError("Upload failed for {FileName}: {Error}",
                        file.FileName, uploadResult.Error);

                    // Rollback already-uploaded files
                    foreach (var url in uploadedUrls)
                        await _storage.DeleteAsync(url, ct);

                    return Result<IReadOnlyList<MediaUploadResultDto>>.Failure(
                        AppErrorCodes.Media.UploadFailed(file.FileName));
                }

                uploadedUrls.Add(uploadResult.FileUrl);

                // First file is primary if no existing media
                var isPrimary = existing.Count == 0 && i == 0;

                var asset = MediaAsset.Create(ownerId, ownerType, uploadResult.FileUrl,
                    nextSortOrder + i, isPrimary);

                await _writeRepo.AddAsync(asset, ct);

                results.Add(new MediaUploadResultDto(
                    asset.Id, asset.Url, asset.IsPrimary, asset.SortOrder));
            }

            await _uow.SaveChangesAsync(ct);

            _logger.LogInformation(
                "Uploaded {Count} file(s) for {OwnerType}:{OwnerId}",
                files.Count, ownerType, ownerId);

            return Result<IReadOnlyList<MediaUploadResultDto>>.Success(results);
        }
        catch (Exception ex)
        {
            // Rollback uploaded files
            foreach (var url in uploadedUrls)
                await _storage.DeleteAsync(url, ct);

            _logger.LogError(ex, "Unexpected error during media upload for {OwnerType}:{OwnerId}",
                ownerType, ownerId);

            return Result<IReadOnlyList<MediaUploadResultDto>>.Failure(
                AppErrorCodes.Media.Unexpected);
        }
    }

    // ────────────────────────────────────────────────────────────────────
    // SET PRIMARY
    // ────────────────────────────────────────────────────────────────────

    public async Task<Result> SetPrimaryAsync(
        Guid ownerId, MediaOwnerType ownerType, Guid mediaId, CancellationToken ct = default)
    {
        var allMedia = await _readRepo.GetByOwnerAsync(ownerId, ownerType, ct);

        var target = allMedia.FirstOrDefault(m => m.Id == mediaId);
        if (target is null)
            return Result.Failure(AppErrorCodes.Media.NotFound);

        // Need tracked versions for EF to pick up changes
        foreach (var m in allMedia)
        {
            if (m.IsPrimary)
            {
                m.UnsetPrimary();
                await _writeRepo.UpdateAsync(m, ct);
            }
        }

        target.SetPrimary();
        await _writeRepo.UpdateAsync(target, ct);
        await _uow.SaveChangesAsync(ct);

        return Result.Success();
    }

    // ────────────────────────────────────────────────────────────────────
    // DELETE ONE
    // ────────────────────────────────────────────────────────────────────

    public async Task<Result> DeleteAsync(
        Guid ownerId, MediaOwnerType ownerType, Guid mediaId, CancellationToken ct = default)
    {
        var allMedia = await _readRepo.GetByOwnerAsync(ownerId, ownerType, ct);
        var target = allMedia.FirstOrDefault(m => m.Id == mediaId);

        if (target is null)
            return Result.Failure(AppErrorCodes.Media.NotFound);

        await _storage.DeleteAsync(target.Url, ct);
        await _writeRepo.DeleteAsync(target, ct);

        // If deleted was primary, promote the next one
        if (target.IsPrimary)
        {
            var next = allMedia
                .Where(m => m.Id != mediaId)
                .OrderBy(m => m.SortOrder)
                .FirstOrDefault();

            if (next is not null)
            {
                next.SetPrimary();
                await _writeRepo.UpdateAsync(next, ct);
            }
        }

        await _uow.SaveChangesAsync(ct);
        return Result.Success();
    }

    // ────────────────────────────────────────────────────────────────────
    // DELETE ALL
    // ────────────────────────────────────────────────────────────────────

    public async Task<Result> DeleteAllAsync(
        Guid ownerId, MediaOwnerType ownerType, CancellationToken ct = default)
    {
        var allMedia = await _readRepo.GetByOwnerAsync(ownerId, ownerType, ct);
        await DeleteAllInternalAsync(allMedia, ct);
        await _uow.SaveChangesAsync(ct);
        return Result.Success();
    }

    // ────────────────────────────────────────────────────────────────────
    // GET ALL
    // ────────────────────────────────────────────────────────────────────

    public async Task<Result<IReadOnlyList<MediaAssetDto>>> GetAllAsync(
        Guid ownerId, MediaOwnerType ownerType, CancellationToken ct = default)
    {
        var media = await _readRepo.GetByOwnerAsync(ownerId, ownerType, ct);

        var dtos = media.Select(m => new MediaAssetDto(
            m.Id, m.Url, m.IsPrimary, m.SortOrder, m.CreatedAt))
            .ToList();

        return Result<IReadOnlyList<MediaAssetDto>>.Success(dtos);
    }

    // ────────────────────────────────────────────────────────────────────
    // REORDER
    // ────────────────────────────────────────────────────────────────────

    public async Task<Result> ReorderAsync(
        Guid ownerId, MediaOwnerType ownerType,
        IReadOnlyList<Guid> orderedIds, CancellationToken ct = default)
    {
        var allMedia = await _readRepo.GetByOwnerAsync(ownerId, ownerType, ct);

        if (orderedIds.Count != allMedia.Count)
            return Result.Failure(AppErrorCodes.Media.ReorderCountMismatch);

        if (orderedIds.Any(id => allMedia.All(m => m.Id != id)))
            return Result.Failure(AppErrorCodes.Media.ReorderInvalidId);

        for (int i = 0; i < orderedIds.Count; i++)
        {
            var asset = allMedia.First(m => m.Id == orderedIds[i]);
            asset.UpdateSortOrder(i);
            await _writeRepo.UpdateAsync(asset, ct);
        }

        await _uow.SaveChangesAsync(ct);
        return Result.Success();
    }

    // ────────────────────────────────────────────────────────────────────
    // PRIVATE HELPERS
    // ────────────────────────────────────────────────────────────────────

    private async Task DeleteAllInternalAsync(
        IReadOnlyList<MediaAsset> assets, CancellationToken ct)
    {
        foreach (var asset in assets)
        {
            await _storage.DeleteAsync(asset.Url, ct);
            await _writeRepo.DeleteAsync(asset, ct);
        }
    }
}
