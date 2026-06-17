using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Review_Guard.Infrastructure.Presistence.Configurations;

public class UserActivityConfiguration : IEntityTypeConfiguration<UserActivity>
{
    public void Configure(EntityTypeBuilder<UserActivity> builder)
    {
        builder.ToTable("UserActivities");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Description)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(a => a.IpAddress)
            .HasMaxLength(45);

        builder.Property(a => a.UserAgent)
            .HasMaxLength(500);

        builder.Property(a => a.Location)
            .HasMaxLength(100);

        builder.Property(a => a.SuspicionReason)
            .HasMaxLength(500);

        var comparer = new ValueComparer<Dictionary<string, string>>(
            (d1, d2) =>
                JsonSerializer.Serialize(d1, (JsonSerializerOptions?)null) ==
                JsonSerializer.Serialize(d2, (JsonSerializerOptions?)null),

            d => JsonSerializer.Serialize(d, (JsonSerializerOptions?)null)
                    .GetHashCode(),

            d => d.ToDictionary(x => x.Key, x => x.Value)
        );

        builder.Property<Dictionary<string, string>>("_metadata")
            .HasColumnName("Metadata")
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<Dictionary<string, string>>(
                        v,
                        (JsonSerializerOptions?)null)
                     ?? new Dictionary<string, string>())
            .Metadata.SetValueComparer(comparer);

        builder.HasIndex(a => a.UserId);
        builder.HasIndex(a => a.AdminId);
        builder.HasIndex(a => a.IsSuspicious);
        builder.HasIndex(a => a.CreatedAt);
    }
}
