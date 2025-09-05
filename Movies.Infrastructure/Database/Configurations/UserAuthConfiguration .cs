using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Movies.Domain.Entities.Auth;

namespace Movies.Infrastructure.Database.Configurations;

public class UserAuthConfiguration : IEntityTypeConfiguration<UserAuth>
{
    public void Configure(EntityTypeBuilder<UserAuth> builder)
    {
        builder.ToTable("UserAuth", "auth");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasIndex(u => u.Email)
            .IsUnique();

        builder.Property(u => u.PasswordHash)
            .IsRequired();

        builder.Property(u => u.CreatedAt)
            .IsRequired();
    }
}
