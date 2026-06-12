using MediatR;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Application.Feature.ReportModul.Dto;
namespace Review_Guard.Application.Feature.ReportModul.Command.ResolveReport;
public sealed record ResolveReportCommand(Guid AdminId, Guid ReportId, AdminReportActionRequest Request) : IRequest<Result>;
