using MediatR;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Domain.Enums;

namespace Review_Guard.Application.Feature.MediaModule.Commands.DeleteAllMedia;

/// <summary>
/// Deletes ALL media assets belonging to an owner entity.
/// Removes every file from storage and every record from the database.
/// Intended for admin use or when the owner entity itself is being deleted.
/// </summary>
public sealed record DeleteAllMediaCommand(
    Guid           OwnerId,
    MediaOwnerType OwnerType
) : IRequest<Result>;
