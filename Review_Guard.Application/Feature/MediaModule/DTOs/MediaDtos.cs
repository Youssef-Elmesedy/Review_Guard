using Review_Guard.Domain.Enums;

namespace Review_Guard.Application.Feature.MediaModule.DTOs;

/// <summary>Response DTO for a single media asset.</summary>
public sealed record MediaAssetResponseDto(
    Guid   Id,
    string Url,
    bool   IsPrimary,
    int    SortOrder,
    MediaOwnerType OwnerType,
    DateTime CreatedAt
);

/// <summary>Result returned after a successful upload.</summary>
public sealed record MediaUploadResponseDto(
    Guid   Id,
    string Url,
    bool   IsPrimary,
    int    SortOrder
);

/// <summary>Request body for reordering media assets.</summary>
public sealed record ReorderMediaDto(
    IReadOnlyList<Guid> OrderedIds
);
