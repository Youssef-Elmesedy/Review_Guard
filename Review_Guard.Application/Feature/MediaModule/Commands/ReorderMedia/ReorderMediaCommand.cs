using MediatR;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Domain.Enums;

namespace Review_Guard.Application.Feature.MediaModule.Commands.ReorderMedia;

/// <summary>
/// Reorders the media assets for an owner.
/// The position in <see cref="OrderedIds"/> determines each asset's SortOrder (0 = first).
/// All existing asset IDs for the owner must be included.
/// </summary>
public sealed record ReorderMediaCommand(
    Guid                    OwnerId,
    MediaOwnerType          OwnerType,
    IReadOnlyList<Guid>     OrderedIds
) : IRequest<Result>;
