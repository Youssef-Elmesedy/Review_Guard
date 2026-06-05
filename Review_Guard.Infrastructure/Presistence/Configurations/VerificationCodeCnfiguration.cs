using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Review_Guard.Infrastructure.Presistence.Configurations;

public class VerificationCodeCnfiguration : IEntityTypeConfiguration<VerificationCode>
{
    public void Configure(EntityTypeBuilder<VerificationCode> builder)
    {
        builder.ToTable("VerificationCodes");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Code)
            .IsRequired()
            .HasMaxLength(6);

        builder.Property(x => x.Type)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(x => x.ExpiresAtUtc)
            .IsRequired();

        builder.Property(x => x.IsUsed)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(x => x.UsedAtUtc);

        // ─────────────────────────────────────────
        // Indexes
        // ─────────────────────────────────────────
        builder.HasIndex(x => x.Code);

        builder.HasIndex(x => new
        {
            x.UserId,
            x.Type,
            x.Code
        });

        // ─────────────────────────────────────────
        // Relationships
        // ─────────────────────────────────────────
        builder.HasOne(x => x.User)
               .WithMany(x => x.VerificationTokens)
               .HasForeignKey(x => x.UserId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
