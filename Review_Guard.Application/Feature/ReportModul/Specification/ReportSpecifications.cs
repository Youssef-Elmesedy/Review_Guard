using Review_Guard.Application.Abstractions.Specifications;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Domain.Entities;
using Review_Guard.Domain.Enums;

namespace Review_Guard.Application.Feature.ReportModul.Specification;

public sealed class ReportByIdSpecification : BaseSpecification<Report>
{
    public ReportByIdSpecification(Guid reportId)
    {
        AddCriteria(r => r.Id == reportId);
        AddInclude(r => r.ReportedByUser);
        AddInclude(r => r.Review);
    }
}

public sealed class AllReportsSpecification : BaseSpecification<Report>
{
    public AllReportsSpecification(PaginationParams paging)
    {
        AddInclude(r => r.ReportedByUser);
        ApplyOrderByDescending(r => r.CreatedAt);
        ApplyPaging(paging.Skip, paging.PageSize);
    }
}

public sealed class OpenReportsSpecification : BaseSpecification<Report>
{
    public OpenReportsSpecification(PaginationParams paging)
    {
        AddCriteria(r => r.Status == ReportStatus.Open);
        AddInclude(r => r.ReportedByUser);
        ApplyOrderByDescending(r => r.CreatedAt);
        ApplyPaging(paging.Skip, paging.PageSize);
    }
}
