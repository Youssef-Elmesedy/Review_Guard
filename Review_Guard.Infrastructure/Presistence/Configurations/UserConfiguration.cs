using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Review_Guard.Infrastructure.Presistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.FullName).IsRequired().HasMaxLength(100);

        builder.Property(u => u.Description).HasMaxLength(100);

        builder.Property(u => u.Phone).HasMaxLength(20);

        builder.Property(u => u.Email).IsRequired().HasMaxLength(255);

        builder.Property(u => u.PasswordHash).IsRequired();

        builder.Property(u => u.TrustScoreValue)
            .HasPrecision(5, 2).HasDefaultValue(80m);

        builder.Property(u => u.Status)
            .HasDefaultValue(AccountStatus.PendingVerification)
            .HasConversion<string>();

        builder.Property(u => u.ProfileImageUrl).HasMaxLength(500);

        builder.Property(u => u.SuspensionReason).HasMaxLength(500);

        builder.HasIndex(u => u.Email).IsUnique();

        builder.HasMany(u => u.Reviews)
            .WithOne(r => r.User)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.Proofs)
            .WithOne(p => p.User)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.Businesses)
            .WithOne(b => b.Owner)
            .HasForeignKey(b => b.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.Activities)
            .WithOne(a => a.User)
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Cascade);

    }
}

