using Review_Guard.Application.Abstractions.Specifications;
using Review_Guard.Application.Common.ResultPattern;
using Review_Guard.Domain.Entities;
using Review_Guard.Domain.Enums;

namespace Review_Guard.Application.Feature.UserModul.Specification;

public sealed class GetAllUsersSpecification : BaseSpecification<User>
{
    public GetAllUsersSpecification(PaginationParams paging)
    {
        AddCriteria(u => u.Role == Roles.User);
        ApplyOrderByDescending(u => u.CreatedAt);
        ApplyPaging(paging.Skip, paging.PageSize);
    }
}
