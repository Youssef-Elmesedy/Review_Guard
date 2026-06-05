namespace Review_Guard.Infrastructure.Implementation.Repositories.BusinessReppository;

internal class WriteBusinessRepository : GenericWriteRepository<Business>, IWriteBusinessRepository
{
    public WriteBusinessRepository(AppDbContext context) : base(context)
    {
    }


}
