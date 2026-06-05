using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Review_Guard.Application.Feature.MediaModule.Commands.DeleteAllMedia;
using Review_Guard.Application.Feature.MediaModule.Commands.DeleteMedia;
using Review_Guard.Application.Feature.MediaModule.Commands.ReorderMedia;
using Review_Guard.Application.Feature.MediaModule.Commands.SetPrimaryMedia;
using Review_Guard.Application.Feature.MediaModule.Commands.UploadMedia;
using Review_Guard.Application.Feature.MediaModule.DTOs;
using Review_Guard.Application.Feature.MediaModule.Queries.GetMediaByOwner;
using Review_Guard.Domain.Enums;

namespace Review_Guard.API.Controllers;

/// <summary>
/// Manages media assets (images / attachments) for all entity types.
///
/// All routes accept an <c>ownerType</c> path parameter that tells the system
/// which entity the files belong to:
/// <list type="bullet">
///   <item><c>Business</c>  — business gallery images</item>
///   <item><c>Branch</c>    — branch gallery images</item>
///   <item><c>User</c>      — user profile photo (max 1 file)</item>
///   <item><c>Proof</c>     — proof-of-visit attachments</item>
/// </list>
/// </summary>
[Authorize]
[Route("api/media/{ownerType}/{ownerId:guid}")]
public sealed class MediaController : BaseController
{
    private readonly IMediator _mediator;

    public MediaController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // ────────────────────────────────────────────────────────────────────
    // QUERIES
    // ────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Get all media assets for an owner, ordered by SortOrder.
    /// Publicly accessible — no auth required.
    /// </summary>
    /// <param name="ownerType">Entity type (Business | Branch | User | Proof)</param>
    /// <param name="ownerId">Owner GUID</param>
    [AllowAnonymous]
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<MediaAssetResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByOwner(
        MediaOwnerType ownerType,
        Guid           ownerId,
        CancellationToken ct)
    {
        var query  = new GetMediaByOwnerQuery(ownerId, ownerType);
        var result = await _mediator.Send(query, ct);
        return HandleResult(result);
    }

    // ────────────────────────────────────────────────────────────────────
    // COMMANDS
    // ────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Upload one or more files for an owner entity.
    /// <br/>Rules per owner type:
    /// <list type="bullet">
    ///   <item><b>Business / Branch</b>: up to 10 files per request, max 20 total.</item>
    ///   <item><b>User</b>: exactly 1 file — replaces the existing profile photo.</item>
    ///   <item><b>Proof</b>: PDF or image, max 10 MB.</item>
    /// </list>
    /// </summary>
    [HttpPost("upload")]
    [RequestSizeLimit(50 * 1024 * 1024)] // 50 MB total per request
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(IReadOnlyList<MediaUploadResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Upload(
        MediaOwnerType       ownerType,
        Guid                 ownerId,
        [FromForm] IFormFileCollection files,
        CancellationToken    ct)
    {
        var command = new UploadMediaCommand(ownerId, ownerType, files.ToList());
        var result  = await _mediator.Send(command, ct);
        return HandleResult(result);
    }

    /// <summary>
    /// Mark a specific asset as the primary (cover / profile) image.
    /// Automatically unsets the previous primary.
    /// </summary>
    [HttpPut("{mediaId:guid}/set-primary")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SetPrimary(
        MediaOwnerType ownerType,
        Guid           ownerId,
        Guid           mediaId,
        CancellationToken ct)
    {
        var command = new SetPrimaryMediaCommand(ownerId, ownerType, mediaId);
        var result  = await _mediator.Send(command, ct);
        return HandleResult(result);
    }

    /// <summary>
    /// Delete a single media asset.
    /// If the deleted asset was primary, the next one is auto-promoted.
    /// </summary>
    [HttpDelete("{mediaId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        MediaOwnerType ownerType,
        Guid           ownerId,
        Guid           mediaId,
        CancellationToken ct)
    {
        var command = new DeleteMediaCommand(ownerId, ownerType, mediaId);
        var result  = await _mediator.Send(command, ct);
        return HandleResult(result);
    }

    /// <summary>
    /// Delete ALL media assets for an owner. Irreversible.
    /// Restricted to Admin / SuperAdmin.
    /// </summary>
    [Authorize(Roles = "Admin,SuperAdmin")]
    [HttpDelete("all")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteAll(
        MediaOwnerType ownerType,
        Guid           ownerId,
        CancellationToken ct)
    {
        var command = new DeleteAllMediaCommand(ownerId, ownerType);
        var result  = await _mediator.Send(command, ct);
        return HandleResult(result);
    }

    /// <summary>
    /// Reorder media assets by submitting an ordered list of asset IDs.
    /// Position in the list = SortOrder (0 = first / cover candidate).
    /// Must include all existing IDs for the owner — no partial reorders.
    /// </summary>
    [HttpPut("reorder")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Reorder(
        MediaOwnerType    ownerType,
        Guid              ownerId,
        [FromBody] ReorderMediaDto dto,
        CancellationToken ct)
    {
        var command = new ReorderMediaCommand(ownerId, ownerType, dto.OrderedIds);
        var result  = await _mediator.Send(command, ct);
        return HandleResult(result);
    }
}
