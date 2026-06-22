// FILE: Review_Guard.Application / Feature / AdminModule / CQRS /
//       UpdateAdminProfile / UpdateAdminProfileCommand.cs

using MediatR;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Application.Feature.AdminModule.DTOs;

namespace Review_Guard.Application.Feature.AdminModule.CQRS.UpdateAdminProfile;

public sealed record UpdateAdminProfileCommand(
    Guid AdminId,
    UpdateAdminProfileRequest Request)
    : IRequest<Result>;
