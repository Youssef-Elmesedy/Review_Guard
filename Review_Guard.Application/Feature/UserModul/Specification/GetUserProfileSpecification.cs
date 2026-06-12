using Review_Guard.Application.Abstractions.Specifications;
using Review_Guard.Domain.Entities;

namespace Review_Guard.Application.Feature.UserModul.Specification;

public class GetUserProfileSpecification : BaseSpecification<User>
{
    public GetUserProfileSpecification(Guid userId)
    {
        if (userId != Guid.Empty)
            AddCriteria(u => u.Id == userId);
    }
}
