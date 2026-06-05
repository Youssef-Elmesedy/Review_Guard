using Review_Guard.Application.Abstractions.Specifications;
using Review_Guard.Domain.Entities;

namespace Review_Guard.Application.Feature.BusinessModul.Specifiaction;

public sealed class BranchWithReviewsSpecification : BaseSpecification<Branch>
{
    public BranchWithReviewsSpecification(Guid? branchId)
    {
        if (branchId.HasValue)
            AddCriteria(b => b.Id == branchId.Value);

        ApplyOrderBy(b => b.Reviews.Count());
    }
}