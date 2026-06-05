using MediatR;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Application.Feature.BusinessModul.Dto;
using Review_Guard.Application.Feature.BusinessModul.Services;

namespace Review_Guard.Application.Feature.BusinessModul.Common.Command;

internal sealed class CreateBusinessCommandHandler : IRequestHandler<CreateBusinessCommand, Result<CreateBusinessResponse>>
{
    private readonly IWriteBusinessService _writeBusinessService;

    public CreateBusinessCommandHandler(IWriteBusinessService writeBusinessService)
    {
        _writeBusinessService = writeBusinessService;
    }

    public async Task<Result<CreateBusinessResponse>> Handle(CreateBusinessCommand request, CancellationToken cancellationToken)
    {
        var result = await _writeBusinessService.CreateBusinessAsync(request.Response, cancellationToken);

        return result;
    }
}
