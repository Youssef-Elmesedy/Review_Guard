// FILE: Review_Guard.Application / Feature / AdminModule / CQRS /
//       GetAdminDashboard / GetAdminDashboardQueryHandler.cs

using MediatR;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Application.Feature.AdminModule.DTOs;
using Review_Guard.Application.Feature.AdminModule.Services;

namespace Review_Guard.Application.Feature.AdminModule.CQRS.GetAdminDashboard;

internal sealed class GetAdminDashboardQueryHandler
    : IRequestHandler<GetAdminDashboardQuery, Result<AdminDashboardResponse>>
{
    private readonly IAdminService _service;

    public GetAdminDashboardQueryHandler(IAdminService service) => _service = service;

    public async Task<Result<AdminDashboardResponse>> Handle(
        GetAdminDashboardQuery request, CancellationToken ct)
        => await _service.GetDashboardAsync(request.AdminId, ct);
}
