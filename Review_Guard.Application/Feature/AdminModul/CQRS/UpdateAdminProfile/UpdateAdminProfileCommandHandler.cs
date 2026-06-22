// FILE: Review_Guard.Application / Feature / AdminModule / CQRS /
//       UpdateAdminProfile / UpdateAdminProfileCommandHandler.cs

using MediatR;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Application.Feature.AdminModule.Services;

namespace Review_Guard.Application.Feature.AdminModule.CQRS.UpdateAdminProfile;

internal sealed class UpdateAdminProfileCommandHandler
    : IRequestHandler<UpdateAdminProfileCommand, Result>
{
    private readonly IAdminService _service;

    public UpdateAdminProfileCommandHandler(IAdminService service) => _service = service;

    public async Task<Result> Handle(
        UpdateAdminProfileCommand request, CancellationToken ct)
        => await _service.UpdateProfileAsync(request.AdminId, request.Request, ct);
}
