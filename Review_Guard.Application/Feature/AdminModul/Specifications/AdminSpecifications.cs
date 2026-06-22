// FILE: Review_Guard.Application / Feature / AdminModule / Specifications / AdminSpecifications.cs

using Review_Guard.Application.Abstractions.Specifications;
using Review_Guard.Domain.Entities;

namespace Review_Guard.Application.Feature.AdminModule.Specifications;

/// <summary>Fetches a single admin by ID.</summary>
public sealed class AdminByIdSpecification : BaseSpecification<Admin>
{
    public AdminByIdSpecification(Guid adminId)
    {
        AddCriteria(a => a.Id == adminId);
    }
}
