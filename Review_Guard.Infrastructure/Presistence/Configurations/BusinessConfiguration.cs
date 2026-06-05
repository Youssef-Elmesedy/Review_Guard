using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Review_Guard.Infrastructure.Presistence.Configurations;

public class BusinessConfiguration : IEntityTypeConfiguration<Business>
{
    public void Configure(EntityTypeBuilder<Business> builder)
    {
        builder.ToTable("Businesses");

        // ── Key ─────────────────────────────
        builder.HasKey(b => b.Id);

        // ── Properties ──────────────────────
        builder.Property(b => b.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(b => b.Description)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(b => b.Status)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(b => b.AdminNote)
            .HasMaxLength(1000);

        // ── Relationships ───────────────────

        // Business → Owner (User)
        builder.HasOne(b => b.Owner)
            .WithMany(u => u.Businesses)
            .HasForeignKey(b => b.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);

        // Business → Category
        builder.HasOne(b => b.BusinessCategory)
            .WithMany(c => c.Businesses)
            .HasForeignKey(b => b.BusinessCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // Business → Branches (BACKING FIELD 🔥)
        builder.HasMany(b => b.Branches)
            .WithOne(b => b.Business)
            .HasForeignKey(b => b.BusinessId)
            .OnDelete(DeleteBehavior.Cascade);

        // ── Indexes ─────────────────────────
        builder.HasIndex(b => b.OwnerId);
        builder.HasIndex(b => b.BusinessCategoryId);
        builder.HasIndex(b => b.Status);
    }
}