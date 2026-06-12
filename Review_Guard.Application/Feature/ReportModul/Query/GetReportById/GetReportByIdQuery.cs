using MediatR;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Application.Feature.ReportModul.Dto;
namespace Review_Guard.Application.Feature.ReportModul.Query.GetReportById;
public sealed record GetReportByIdQuery(Guid ReportId) : IRequest<Result<ReportResponseDto>>;
