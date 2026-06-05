using Review_Guard.Application.Abstractions.Specifications;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Domain.Entities;

namespace Review_Guard.Application.Feature.BusinessModul.Specifiaction;

public sealed class BusinessListWithBranchsSpecification : BaseSpecification<Business>
{
    public BusinessListWithBranchsSpecification(PaginationParams paginationParams)
    {
        ApplyPaging(paginationParams.Skip, paginationParams.PageSize);


        ApplyOrderBy(b => b.Name);
    }

}
