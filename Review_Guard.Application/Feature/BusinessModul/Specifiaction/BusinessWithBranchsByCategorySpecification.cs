using Review_Guard.Application.Abstractions.Specifications;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Domain.Entities;

namespace Review_Guard.Application.Feature.BusinessModul.Specifiaction;

public class BusinessWithBranchsByCategorySpecification : BaseSpecification<Business>
{
    public BusinessWithBranchsByCategorySpecification(PaginationParams paginationParams, Guid? categoryId = null)
    {
        ApplyPaging(paginationParams.Skip, paginationParams.PageSize);

        if (categoryId.HasValue)
            AddCriteria(b => b.BusinessCategoryId == categoryId.Value);

        ApplyOrderBy(b => b.Name);
    }
}