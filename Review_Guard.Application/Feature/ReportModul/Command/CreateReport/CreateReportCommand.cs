using MediatR;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Application.Feature.ReportModul.Dto;
namespace Review_Guard.Application.Feature.ReportModul.Command.CreateReport;
public sealed record CreateReportCommand(Guid UserId, CreateReportRequest Request) : IRequest<Result<ReportResponseDto>>;
