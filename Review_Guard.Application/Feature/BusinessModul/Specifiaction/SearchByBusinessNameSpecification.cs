using Review_Guard.Application.Abstractions.Specifications;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Domain.Entities;

namespace Review_Guard.Application.Feature.BusinessModul.Specifiaction;

public sealed class SearchByBusinessNameSpecification : BaseSpecification<Business>
{
    public SearchByBusinessNameSpecification(PaginationParams paginationParams, string? businessName)
    {
        ApplyPaging(paginationParams.Skip, paginationParams.PageSize);

        if (!string.IsNullOrEmpty(businessName))
        {
            AddCriteria(b => b.Name.Contains(businessName));
        }

        ApplyOrderBy(b => b.Name);
    }
}
