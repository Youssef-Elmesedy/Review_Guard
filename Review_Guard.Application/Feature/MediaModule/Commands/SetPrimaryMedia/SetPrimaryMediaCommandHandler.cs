using MediatR;
using Review_Guard.Application.Abstractions.Services.MediaService;
using Review_Guard.Application.Common.ResultPattern;

namespace Review_Guard.Application.Feature.MediaModule.Commands.SetPrimaryMedia;

internal sealed class SetPrimaryMediaCommandHandler
    : IRequestHandler<SetPrimaryMediaCommand, Result>
{
    private readonly IMediaService _mediaService;

    public SetPrimaryMediaCommandHandler(IMediaService mediaService)
    {
        _mediaService = mediaService;
    }

    public async Task<Result> Handle(
        SetPrimaryMediaCommand request,
        CancellationToken      cancellationToken)
    {
        return await _mediaService.SetPrimaryAsync(
            request.OwnerId,
            request.OwnerType,
            request.MediaId,
            cancellationToken);
    }
}
