using Review_Guard.Application.Abstractions.Repositories.GenericRepository;
using Review_Guard.Domain.Entities;

namespace Review_Guard.Application.Abstractions.Repositories.AdminRepository;

public interface IReadAdminRepository : IGenericReadRepository<Admin>
{
    /// <summary>Returns all admins — used by NotificationService for broadcasts.</summary>
    Task<IReadOnlyList<Admin>> ListAllAsync(CancellationToken ct = default);
}
