using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Movies.Domain.Entities;

namespace Movies.Infrastructure.Database.Configurations;
public class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        builder.ToTable("Tenants", "dbo");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Code)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(t => t.IsActive)
            .HasDefaultValue(true);
    }
}
