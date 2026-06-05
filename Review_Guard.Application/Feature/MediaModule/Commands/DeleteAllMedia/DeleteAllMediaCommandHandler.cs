using MediatR;
using Review_Guard.Application.Abstractions.Services.MediaService;
using Review_Guard.Application.Common.ResultPattern;

namespace Review_Guard.Application.Feature.MediaModule.Commands.DeleteAllMedia;

internal sealed class DeleteAllMediaCommandHandler
    : IRequestHandler<DeleteAllMediaCommand, Result>
{
    private readonly IMediaService _mediaService;

    public DeleteAllMediaCommandHandler(IMediaService mediaService)
    {
        _mediaService = mediaService;
    }

    public async Task<Result> Handle(
        DeleteAllMediaCommand request,
        CancellationToken     cancellationToken)
    {
        return await _mediaService.DeleteAllAsync(
            request.OwnerId,
            request.OwnerType,
            cancellationToken);
    }
}
