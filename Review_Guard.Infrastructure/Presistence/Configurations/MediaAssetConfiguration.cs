using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Review_Guard.Infrastructure.Presistence.Configurations;

public class MediaAssetConfiguration : IEntityTypeConfiguration<MediaAsset>
{
    public void Configure(EntityTypeBuilder<MediaAsset> builder)
    {
        builder.ToTable("MediaAssets");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Url)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.OwnerType).IsRequired();
        builder.Property(x => x.Type).IsRequired();
        builder.Property(x => x.SortOrder).IsRequired();
        builder.Property(x => x.IsPrimary).IsRequired();

        // ── Business Relation ───────────────────────────────────────
        builder.HasOne(x => x.Business)
            .WithMany(b => b.Media)
            .HasForeignKey(x => x.BusinessId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(false);

        // ── Branch Relation ─────────────────────────────────────────
        builder.HasOne(x => x.Branch)
            .WithMany(b => b.Media)
            .HasForeignKey(x => x.BranchId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);

        // ── User Relation ───────────────────────────────────────────
        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(false);

        // ── Proof Relation ──────────────────────────────────────────
        builder.HasOne(x => x.Proof)
            .WithMany()
            .HasForeignKey(x => x.ProofId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(false);

        // ── Indexes ─────────────────────────────────────────────────
        builder.HasIndex(x => new { x.OwnerType, x.BusinessId });
        builder.HasIndex(x => new { x.OwnerType, x.BranchId });
        builder.HasIndex(x => new { x.OwnerType, x.UserId });
        builder.HasIndex(x => new { x.OwnerType, x.ProofId });

        // Unique: one primary per owner (per type)
        builder.HasIndex(x => new { x.BusinessId, x.IsPrimary })
            .HasFilter("[BusinessId] IS NOT NULL AND [IsPrimary] = 1")
            .IsUnique();

        builder.HasIndex(x => new { x.BranchId, x.IsPrimary })
            .HasFilter("[BranchId] IS NOT NULL AND [IsPrimary] = 1")
            .IsUnique();

        builder.HasIndex(x => new { x.UserId, x.IsPrimary })
            .HasFilter("[UserId] IS NOT NULL AND [IsPrimary] = 1")
            .IsUnique();
    }
}