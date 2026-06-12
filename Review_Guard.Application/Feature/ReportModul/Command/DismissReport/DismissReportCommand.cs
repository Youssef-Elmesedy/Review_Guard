using MediatR;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Application.Feature.ReportModul.Dto;
namespace Review_Guard.Application.Feature.ReportModul.Command.DismissReport;
public sealed record DismissReportCommand(Guid AdminId, Guid ReportId, AdminReportActionRequest Request) : IRequest<Result>;
