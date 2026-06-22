using MediatR;
using Review_Guard.Application.Feature.AdminModule.Services;

namespace Review_Guard.Application.Feature.AdminModul.CQRS.ChangePassword;

internal sealed class ChangePasswordAdminCommandHandler : IRequestHandler<ChangePasswordAdminCommand, Result>
{
    private readonly IAdminService _adminService;

    public ChangePasswordAdminCommandHandler(IAdminService adminService)
     => _adminService = adminService;

    public Task<Result> Handle(ChangePasswordAdminCommand request, CancellationToken ct = default)
    {
        var result = _adminService
            .ChangePasswordAsync(request.AdminId, request.Request, ct);

        return result;
    }
}
