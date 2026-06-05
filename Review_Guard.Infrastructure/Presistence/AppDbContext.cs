namespace Review_Guard.Infrastructure.Presistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Business> Businesses => Set<Business>();
    public DbSet<Branch> Branches => Set<Branch>();
    public DbSet<BusinessCategory> BusinessCategories => Set<BusinessCategory>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<Proof> Proofs => Set<Proof>();
    public DbSet<Report> Reports => Set<Report>();
    public DbSet<UserActivity> UserActivities => Set<UserActivity>();
    public DbSet<Admin> Admins => Set<Admin>();
    public DbSet<VerificationCode> VerificationTokens => Set<VerificationCode>();
    public DbSet<RefreshToken> RefreshCodes => Set<RefreshToken>();
    public DbSet<MediaAsset> MediaAssets => Set<MediaAsset>();

    // Add Rewards DbSet
    public DbSet<UserReward> Rewards => Set<UserReward>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}