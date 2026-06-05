using MediatR;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Application.Feature.MediaModule.DTOs;
using Review_Guard.Domain.Enums;

namespace Review_Guard.Application.Feature.MediaModule.Queries.GetMediaByOwner;

/// <summary>
/// Returns all media assets belonging to a given owner, ordered by SortOrder.
/// </summary>
public sealed record GetMediaByOwnerQuery(
    Guid           OwnerId,
    MediaOwnerType OwnerType
) : IRequest<Result<IReadOnlyList<MediaAssetResponseDto>>>;
