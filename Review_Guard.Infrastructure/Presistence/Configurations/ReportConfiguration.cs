using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Review_Guard.Domain.Entities;

public class ReportConfiguration : IEntityTypeConfiguration<Report>
{
    public void Configure(EntityTypeBuilder<Report> builder)
    {
        builder.ToTable("Reports");

        // ── Key ─────────────────────────────
        builder.HasKey(r => r.Id);

        // ── Properties ──────────────────────
        builder.Property(r => r.Reason)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(r => r.Status)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(r => r.Description)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(r => r.AdminNote)
            .HasMaxLength(1000);

        // ── Relationships ───────────────────

        // Report → UserError (ReportedBy)
        builder.HasOne(r => r.ReportedByUser)
            .WithMany()
            .HasForeignKey(r => r.ReportedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Report → Review
        builder.HasOne(r => r.Review)
            .WithMany()
            .HasForeignKey(r => r.ReviewId)
            .OnDelete(DeleteBehavior.Cascade);

        // ── Indexes ─────────────────────────
        builder.HasIndex(r => r.ReviewId);
        builder.HasIndex(r => r.ReportedByUserId);
        builder.HasIndex(r => r.Status);
    }
}