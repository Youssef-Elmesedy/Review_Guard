// FILE: Review_Guard.Application / Feature / AdminModule / CQRS /
//       GetAdminDashboard / GetAdminDashboardQuery.cs

using MediatR;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Application.Feature.AdminModule.DTOs;

namespace Review_Guard.Application.Feature.AdminModule.CQRS.GetAdminDashboard;

public sealed record GetAdminDashboardQuery(Guid AdminId)
    : IRequest<Result<AdminDashboardResponse>>;
