using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Review_Guard.Domain.Entities;

namespace Review_Guard.Infrastructure.Presistence.Configurations;

public class BranchConfiguration : IEntityTypeConfiguration<Branch>
{
    public void Configure(EntityTypeBuilder<Branch> builder)
    {
        builder.ToTable("Branches");

        // ── Key ─────────────────────────────
        builder.HasKey(b => b.Id);

        // ── Properties ──────────────────────
        builder.Property(b => b.Address)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(b => b.City)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(b => b.Country)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(b => b.PhoneNumber)
            .IsRequired()
            .HasMaxLength(20);

        // ── Rating Fields ───────────────────
        builder.Property(b => b.SimpleAverageRating)
            .HasPrecision(5, 2);

        builder.Property(b => b.WeightedAverageRating)
            .HasPrecision(5, 2);

        builder.Property(b => b.TotalReviews)
            .IsRequired();

        builder.Property(b => b.ApprovedReviewCount)
            .IsRequired();

        builder.Property(b => b.PendingReviewCount)
            .IsRequired();

        // ── Relationships ───────────────────

        // Branch → Business (Many-to-One)
        builder.HasOne(b => b.Business)
            .WithMany(bu => bu.Branches)
            .HasForeignKey(b => b.BusinessId)
            .OnDelete(DeleteBehavior.Cascade);

        // Branch → Manager (UserError)
        builder.HasOne(b => b.Manager)
            .WithMany()
            .HasForeignKey(b => b.ManagerId)
            .OnDelete(DeleteBehavior.Restrict);

        // Branch → Reviews
        builder.HasMany(r => r.Reviews)
            .WithOne()
            .HasForeignKey(r => r.BranchId)
            .OnDelete(DeleteBehavior.Cascade);

        // ── Backing Field Config (DDD Important) ──
        builder.Metadata
            .FindNavigation(nameof(Branch.Reviews))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
