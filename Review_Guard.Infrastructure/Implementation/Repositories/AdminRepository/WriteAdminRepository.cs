namespace Review_Guard.Infrastructure.Implementation.Repositories.AdminRepository;

internal sealed class WriteAdminRepository : GenericWriteRepository<Admin>, IWriteAdminRepository
{
    public WriteAdminRepository(AppDbContext context) : base(context)
    {
    }
}
