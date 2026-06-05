using MediatR;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Domain.Enums;

namespace Review_Guard.Application.Feature.MediaModule.Commands.SetPrimaryMedia;

/// <summary>
/// Marks a specific media asset as the primary (cover/profile) image for an owner.
/// Automatically unsets the previous primary asset.
/// </summary>
public sealed record SetPrimaryMediaCommand(
    Guid           OwnerId,
    MediaOwnerType OwnerType,
    Guid           MediaId
) : IRequest<Result>;
