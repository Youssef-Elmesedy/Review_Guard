using MediatR;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Application.Feature.ReportModul.Dto;
namespace Review_Guard.Application.Feature.ReportModul.Query.GetAllReports;
public sealed record GetAllReportsQuery(int PageNumber, int PageSize, bool OpenOnly = false) : IRequest<Result<PagedResult<ReportListItemDto>>>;
