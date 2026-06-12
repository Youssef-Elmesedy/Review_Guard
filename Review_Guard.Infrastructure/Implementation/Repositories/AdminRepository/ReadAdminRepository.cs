using Review_Guard.Application.Abstractions.Repositories.AdminRepository;
using Review_Guard.Infrastructure.Implementation.Repositories.GeneircRepository;

namespace Review_Guard.Infrastructure.Implementation.Repositories.AdminRepository;

internal sealed class ReadAdminRepository : GenericReadRepository<Admin>, IReadAdminRepository
{
    public ReadAdminRepository(AppDbContext appDbContext) : base(appDbContext) { }

    public async Task<IReadOnlyList<Admin>> ListAllAsync(CancellationToken ct = default)
        => await _appDbContext.Admins
            .AsNoTracking()
            .ToListAsync(ct);
}
