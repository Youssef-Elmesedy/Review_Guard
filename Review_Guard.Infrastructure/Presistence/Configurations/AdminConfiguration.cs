using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Review_Guard.Infrastructure.Presistence.Configurations;

public class AdminConfiguration : IEntityTypeConfiguration<Admin>
{
    public void Configure(EntityTypeBuilder<Admin> builder)
    {
        builder.ToTable("Admins");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.FullName).IsRequired().HasMaxLength(100);

        builder.Property(i => i.Role).HasMaxLength(50);

        builder.Property(a => a.Email).HasMaxLength(255);

        builder.Property(a => a.PasswordHash).IsRequired();

        builder.HasIndex(a => a.Email).IsUnique();

        builder.HasMany(a => a.Activities)
            .WithOne(a => a.Admin)
            .HasForeignKey(a => a.AdminId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
