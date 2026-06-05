using Review_Guard.Application.Abstractions.Specifications;
using Review_Guard.Domain.Entities;

namespace Review_Guard.Application.Feature.BusinessModul.Specifiaction;

public sealed class BusinessWithBranchsSpecification : BaseSpecification<Business>
{
    public BusinessWithBranchsSpecification(Guid? businessId)
    {
        if (businessId.HasValue)
        {
            AddCriteria(b => b.Id == businessId.Value);
        }

        ApplyOrderBy(b => b.Name);
    }
}
