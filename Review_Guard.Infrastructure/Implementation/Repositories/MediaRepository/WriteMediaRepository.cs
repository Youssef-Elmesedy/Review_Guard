using Review_Guard.Application.Abstractions.Repositories.MediaRepository;
using Review_Guard.Infrastructure.Implementation.Repositories.GeneircRepository;

namespace Review_Guard.Infrastructure.Implementation.Repositories.MediaRepository;

internal sealed class WriteMediaRepository : GenericWriteRepository<MediaAsset>, IWriteMediaRepository
{
    public WriteMediaRepository(AppDbContext appDbContext) : base(appDbContext) { }
}
