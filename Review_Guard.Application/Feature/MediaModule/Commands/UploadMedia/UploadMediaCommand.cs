using MediatR;
using Microsoft.AspNetCore.Http;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Application.Feature.MediaModule.DTOs;
using Review_Guard.Domain.Enums;

namespace Review_Guard.Application.Feature.MediaModule.Commands.UploadMedia;

/// <summary>
/// Uploads one or more files and attaches them to an owner entity.
/// Handles: Business images | Branch images | User profile photo | Proof attachments.
/// </summary>
public sealed record UploadMediaCommand(
    Guid                    OwnerId,
    MediaOwnerType          OwnerType,
    IReadOnlyList<IFormFile> Files
) : IRequest<Result<IReadOnlyList<MediaUploadResponseDto>>>;
