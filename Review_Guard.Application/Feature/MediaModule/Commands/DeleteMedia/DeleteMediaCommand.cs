using MediatR;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Domain.Enums;

namespace Review_Guard.Application.Feature.MediaModule.Commands.DeleteMedia;

/// <summary>
/// Deletes a single media asset.
/// Removes the file from storage and the record from the database.
/// If the deleted asset was the primary, the next asset is promoted automatically.
/// </summary>
public sealed record DeleteMediaCommand(
    Guid           OwnerId,
    MediaOwnerType OwnerType,
    Guid           MediaId
) : IRequest<Result>;
