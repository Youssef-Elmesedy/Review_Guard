using MediatR;
using Review_Guard.Application.Feature.AdminModule.DTOs;

namespace Review_Guard.Application.Feature.AdminModul.CQRS.ChangePassword;

public record ChangePasswordAdminCommand(
    Guid AdminId,
    ChangeAdminPasswordRequest Request
) : IRequest<Result>;
