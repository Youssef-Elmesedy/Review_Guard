using Review_Guard.Application.Abstractions.Specifications;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Domain.Entities;

namespace Review_Guard.Application.Feature.UserModul.Specification;

public sealed class GetUserActivitiesSpecification : BaseSpecification<UserActivity>
{
    public GetUserActivitiesSpecification(Guid userId, PaginationParams paging)
    {
        AddCriteria(a => a.UserId == userId);
        ApplyOrderByDescending(a => a.CreatedAt);
        ApplyPaging(paging.Skip, paging.PageSize);
    }
}
