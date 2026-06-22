// FILE: Review_Guard.Application / Feature / AdminModule / CQRS /
//       GetAdminProfile / GetAdminProfileQueryHandler.cs

using MediatR;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Application.Feature.AdminModule.DTOs;
using Review_Guard.Application.Feature.AdminModule.Services;

namespace Review_Guard.Application.Feature.AdminModule.CQRS.GetAdminProfile;

internal sealed class GetAdminProfileQueryHandler
    : IRequestHandler<GetAdminProfileQuery, Result<AdminProfileResponse>>
{
    private readonly IAdminService _service;

    public GetAdminProfileQueryHandler(IAdminService service) => _service = service;

    public async Task<Result<AdminProfileResponse>> Handle(
        GetAdminProfileQuery request, CancellationToken ct)
        => await _service.GetProfileAsync(request.AdminId, ct);
}
