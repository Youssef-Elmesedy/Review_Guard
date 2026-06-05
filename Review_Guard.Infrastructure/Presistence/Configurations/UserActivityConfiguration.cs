using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Review_Guard.Infrastructure.Presistence.Configurations;

public class UserActivityConfiguration : IEntityTypeConfiguration<UserActivity>
{
    public void Configure(EntityTypeBuilder<UserActivity> builder)
    {
        builder.ToTable("UserActivities");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Description).IsRequired().HasMaxLength(500);

        builder.Property(a => a.IpAddress).HasMaxLength(45);

        builder.Property(a => a.UserAgent).HasMaxLength(500);

        builder.Property(a => a.Location).HasMaxLength(100);

        builder.Property(a => a.SuspicionReason).HasMaxLength(500);

        builder.Ignore(a => a.Metadata);

        builder.Property(a => a.Metadata)
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(v,
                    (System.Text.Json.JsonSerializerOptions?)null) ?? new Dictionary<string, string>())
            .HasColumnType("nvarchar(max)");

        builder.HasIndex(a => a.UserId);

        builder.HasIndex(a => a.AdminId);

        builder.HasIndex(a => a.IsSuspicious);

        builder.HasIndex(a => a.CreatedAt);

    }
}
