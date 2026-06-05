using Review_Guard.Domain.Common;
using Review_Guard.Domain.Enums;
using Review_Guard.Domain.Exceptions;

namespace Review_Guard.Domain.Entities;

public class BusinessCategory : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public CategoryStatus Status { get; private set; } = CategoryStatus.Pending;

    // Navigation property for related businesses (if needed)
    private readonly List<Business> _businesses = new();
    public IReadOnlyCollection<Business> Businesses => _businesses.AsReadOnly();

    public static BusinessCategory Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Category name is required.", DomainMessagies.FullNameRequired);

        return new BusinessCategory
        {
            Name = name.Trim(),
            Status = CategoryStatus.Pending,
        };
    }

    public void Approve()
    {
        Status = CategoryStatus.Approved;
    }

    public void Reject()
    {
        Status = CategoryStatus.Rejected;
    }
}

