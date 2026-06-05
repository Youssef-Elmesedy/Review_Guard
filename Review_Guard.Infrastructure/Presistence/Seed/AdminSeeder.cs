using Review_Guard.Domain.Enums;

namespace Review_Guard.Infrastructure.Presistence.Seed;

#region Admin Seeder

public static class AdminSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var logger = scope.ServiceProvider
            .GetRequiredService<ILoggerFactory>()
            .CreateLogger("AdminSeeder");

        try
        {
            if (await context.Admins.AnyAsync())
            {
                logger.LogInformation("Admin already seeded");
                return;
            }

            var passwordHasher = new PasswordHasher();

            var admin = Admin.Create(
                "Youssef Elmesedy",
                "admin@reviewguard.com",
                passwordHasher.HashPassword("Admin@123#"),
                Roles.SuperAdmin);

            await context.Admins.AddAsync(admin);

            await context.SaveChangesAsync();

            logger.LogInformation("Admin seeded successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while seeding admin");
            throw;
        }
    }
}

#endregion
