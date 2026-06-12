using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Review_Guard.Infrastructure.Presistence.Configurations;

public sealed class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("Notifications");
        builder.HasKey(n => n.Id);

        builder.Property(n => n.Title).IsRequired().HasMaxLength(200);
        builder.Property(n => n.Message).IsRequired().HasMaxLength(1000);
        builder.Property(n => n.Type).IsRequired().HasConversion<string>();
        builder.Property(n => n.Target).IsRequired();
        builder.Property(n => n.ReferenceId).HasMaxLength(100);
        builder.Property(n => n.ReferenceType).HasMaxLength(50);

        // User FK
        builder.HasOne(n => n.User)
            .WithMany()
            .HasForeignKey(n => n.UserId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(false);

        // Admin FK
        builder.HasOne(n => n.Admin)
            .WithMany()
            .HasForeignKey(n => n.AdminId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(false);

        // Indexes
        builder.HasIndex(n => new { n.UserId, n.IsRead, n.CreatedAt });
        builder.HasIndex(n => new { n.AdminId, n.IsRead, n.CreatedAt });
        builder.HasIndex(n => n.CreatedAt);
    }
}
