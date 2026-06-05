using MediatR;
using Review_Guard.Application.Abstractions.Services.MediaService;
using Review_Guard.Application.Common.ResultPattern;

namespace Review_Guard.Application.Feature.MediaModule.Commands.DeleteMedia;

internal sealed class DeleteMediaCommandHandler
    : IRequestHandler<DeleteMediaCommand, Result>
{
    private readonly IMediaService _mediaService;

    public DeleteMediaCommandHandler(IMediaService mediaService)
    {
        _mediaService = mediaService;
    }

    public async Task<Result> Handle(
        DeleteMediaCommand request,
        CancellationToken  cancellationToken)
    {
        return await _mediaService.DeleteAsync(
            request.OwnerId,
            request.OwnerType,
            request.MediaId,
            cancellationToken);
    }
}
