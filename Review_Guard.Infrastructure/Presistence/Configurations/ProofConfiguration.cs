using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Review_Guard.Infrastructure.Presistence.Configurations;

public class ProofConfiguration : IEntityTypeConfiguration<Proof>
{
    public void Configure(EntityTypeBuilder<Proof> builder)
    {
        builder.ToTable("Proofs");

        // ── Key ─────────────────────────────
        builder.HasKey(p => p.Id);

        // ── Properties ──────────────────────
        builder.Property(p => p.Type)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(p => p.Status)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(p => p.FileUrl)
            .HasMaxLength(500);

        builder.Property(p => p.OrderId)
            .HasMaxLength(100);

        builder.Property(p => p.AdminNote)
            .HasMaxLength(1000);

        // ── Relationships ───────────────────

        // Proof → User
        builder.HasOne(p => p.User)
            .WithMany(u => u.Proofs)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Restrict);


        // Proof → Branch
        builder.HasOne(p => p.Branch)
            .WithMany()
            .HasForeignKey(p => p.BranchId)
            .OnDelete(DeleteBehavior.Restrict);

        // ── Indexes ─────────────────────────
        builder.HasIndex(p => p.UserId);
        builder.HasIndex(p => p.BranchId);
        builder.HasIndex(p => p.Status);
    }
}
