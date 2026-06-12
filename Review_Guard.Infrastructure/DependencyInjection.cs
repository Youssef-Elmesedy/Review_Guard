using Review_Guard.Application.Feature.ReviewModul.Services;
using Review_Guard.Application.Feature.ProofModul.Services;
using Review_Guard.Application.Feature.ReportModul.Services;
using Review_Guard.Infrastructure.Implementation.Servcices.ReviewService;
using Review_Guard.Infrastructure.Implementation.Servcices.ProofService;
using Review_Guard.Infrastructure.Implementation.Servcices.ReportService;
using Review_Guard.Infrastructure.Implementation.Servcices.NotificationService;
using Review_Guard.Infrastructure.Implementation.Repositories.NotificationRepository;
using Review_Guard.Application.Abstractions.Services.NotificationService;
using Review_Guard.Application.Abstractions.Repositories.NotificationRepository;
using Review_Guard.Application.Abstractions.Repositories.MediaRepository;
using Review_Guard.Application.Abstractions.Services.MediaService;
using Review_Guard.Application.Feature.BusinessModul.Services;
using Review_Guard.Application.Feature.UserModul.UserService;
using Review_Guard.Infrastructure.Implementation.Repositories.MediaRepository;
using Review_Guard.Infrastructure.Implementation.Servcices.BusinessService;
using Review_Guard.Infrastructure.Implementation.Servcices.MediaService;
using Review_Guard.Infrastructure.Implementation.Servcices.UserService;

namespace Review_Guard.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services, IConfiguration configuration)
    {
        // ── Database ───────────────────────────────────────────────────────────
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                sql => sql
                    .EnableRetryOnFailure(maxRetryCount: 3,
                        maxRetryDelay: TimeSpan.FromSeconds(5),
                        errorNumbersToAdd: null)
                    .CommandTimeout(30)));

        //// ── Unit of Work ───────────────────────────────────────────────────────
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // ---- caching services ----------------------------------------------------
        // Memory Cache
        services.AddMemoryCache();

        // Distributed Cache (Redis أو In-Memory مؤقت)
        services.AddDistributedMemoryCache(); // 👈 مؤقت (بدل Redis)

        // Hybrid Cache Service
        services.AddSingleton<ICacheService, MemoryCacheService>();

        //// ── Core Repositories ──────────────────────────────────────────────────
        services.AddScoped(typeof(IGenericReadRepository<>), typeof(GenericReadRepository<>));
        services.AddScoped(typeof(IGenericWriteRepository<>), typeof(GenericWriteRepository<>));

        services.AddScoped<IRewardService, RewardService>();

        services.AddScoped<IReadAdminRepository, ReadAdminRepository>();
        services.AddScoped<IWriteAdminRepository, WriteAdminRepository>();

        services.AddScoped<IReadBranchRepository, ReadBranchRepository>();
        services.AddScoped<IWriteBranchRepository, WriteBranchRepository>();

        services.AddScoped<IReadBusinessRepository, ReadBusinessRepository>();
        services.AddScoped<IWriteBusinessRepository, WriteBusinessRepository>();

        services.AddScoped<IReadProofRepository, ReadProofRepository>();
        services.AddScoped<IWriteProofRepository, WriteProofRepository>();

        services.AddScoped<IReadReportRepository, ReadReportRepository>();
        services.AddScoped<IWriteReportRepository, WriteReportRepository>();

        services.AddScoped<IReadReviewRepository, ReadReviewRepository>();
        services.AddScoped<IWriteReviewRepository, WriteReviewRepository>();

        services.AddScoped<IReadUserRepository, ReadUserRepository>();
        services.AddScoped<IWriteUserRepository, WriteUserRepository>();

        services.AddScoped<IReadUserActivityRepository, ReadUserActivityRepository>();
        services.AddScoped<IWriteUserActivityRepository, WriteUserActivityRepository>();

        services.AddScoped<IReadVerificationTokenRepository, ReadVerificationTokenRepository>();
        services.AddScoped<IWriteVerificationTokenRepository, WriteVerificationTokenRepository>();

        services.AddScoped<IReadMediaRepository, ReadMediaRepository>();
        services.AddScoped<IWriteMediaRepository, WriteMediaRepository>();

        // ── Notification ──────────────────────────────────────────────────────
        services.AddScoped<IReadNotificationRepository,  ReadNotificationRepository>();
        services.AddScoped<IWriteNotificationRepository, WriteNotificationRepository>();
        services.AddScoped<INotificationService,         NotificationService>();

        //// ── Dashboard Repositories (read-model / CQRS read side) ───────────────
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IVerificationCodeService, VerificationCodeService>();

        services.AddScoped<IReadBusinessService, ReadBusinessService>();
        services.AddScoped<IWriteBusinessService, WriteBusinessService>();

        services.AddScoped<IMediaService, MediaService>();

        services.AddScoped<IReadUserService, RseadUserService>();
        services.AddScoped<IWriteUserService, WriteUserService>();

        // ── Review ────────────────────────────────────────────────────────────
        services.AddScoped<IReadReviewService,  ReadReviewService>();
        services.AddScoped<IWriteReviewService, WriteReviewService>();

        // ── Proof ─────────────────────────────────────────────────────────────
        services.AddScoped<IReadProofService,  ReadProofService>();
        services.AddScoped<IWriteProofService, WriteProofService>();

        // ── Report ────────────────────────────────────────────────────────────
        services.AddScoped<IReadReportService,  ReadReportService>();
        services.AddScoped<IWriteReportService, WriteReportService>();

        //services.AddScoped<IUserDashboardRepository, UserDashboardRepository>();
        //services.AddScoped<IOwnerDashboardRepository, OwnerDashboardRepository>();
        //services.AddScoped<IAdminDashboardRepository, AdminDashboardRepository>();

        //// ── Settings ───────────────────────────────────────────────────────────
        services.Configure<SmtpSettings>(configuration.GetSection("Smtp"));
        services.Configure<JwtSettings>(configuration.GetSection("Jwt"));
        services.Configure<FileStorageSettings>(configuration.GetSection("FileStorage"));

        //// ── Application + Infrastructure Services ─────────────────────────────
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IEmailTemplateRenderer, EmailTemplateRenderer>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IFileStorageService, LocalFileStorageService>();
        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
        //services.AddScoped<IRiskScoreService, RiskScoreService>();
        //services.AddScoped<IWeightedRatingService, WeightedRatingService>();

        // GeoLocation Service with HttpClient and resilience policies
        services.AddHttpClient<IGeoLocationService, GeoLocationService>()
            .SetHandlerLifetime(TimeSpan.FromMinutes(5))
            .AddStandardResilienceHandler();

        return services;
    }

}

// Migration helper to apply pending migrations on application startup
public static class MigrationExtensions
{
    public static void ApplyMigrations(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Database.Migrate();
    }
}
