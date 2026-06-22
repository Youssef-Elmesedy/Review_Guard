// FILE: Review_Guard.Application / Feature / AdminModule / CQRS /
//       GetAdminProfile / GetAdminProfileQuery.cs

using MediatR;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Application.Feature.AdminModule.DTOs;

namespace Review_Guard.Application.Feature.AdminModule.CQRS.GetAdminProfile;

public sealed record GetAdminProfileQuery(Guid AdminId)
    : IRequest<Result<AdminProfileResponse>>;
