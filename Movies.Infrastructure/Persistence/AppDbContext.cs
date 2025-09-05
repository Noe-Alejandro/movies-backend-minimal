using Microsoft.EntityFrameworkCore;
using Movies.Domain.Entities;
using Movies.Domain.Entities.Auth;

namespace Movies.Infrastructure.Persistence;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Movie> Movies => Set<Movie>();

    #region Auth Schema
    public DbSet<UserAuth> Users { get; set; } = default!;
    public DbSet<UserSession> UserSessions { get; set; } = default!;
    public DbSet<Role> Roles { get; set; } = default!;
    public DbSet<Permission> Permissions { get; set; } = default!;
    public DbSet<UserRole> UserRoles { get; set; } = default!;
    public DbSet<UserPermission> UserPermissions { get; set; } = default!;
    public DbSet<Tenant> Tenants { get; set; } = default!;
    public DbSet<AllowedEmailDomain> AllowedEmailDomains { get; set; } = default!;
    #endregion

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Movie>(e =>
        {
            e.ToTable("Movies");
            e.HasKey(x => x.Id);

            e.Property(x => x.Title)
          .HasMaxLength(200)
          .IsRequired();

            e.Property(x => x.Genres)
          .HasMaxLength(2000);

            e.Property(x => x.DurationMinutes)
          .IsRequired();

            e.Property(x => x.Synopsis)
          .HasColumnType("nvarchar(max)");

            e.HasIndex(x => x.Year)
          .HasDatabaseName("IX_Movies_Year");
        });

        modelBuilder.Entity<UserAuth>().ToTable("UserAuth", "auth");
        modelBuilder.Entity<UserSession>().ToTable("UserSessions", "auth");

        modelBuilder.Entity<UserTenant>().ToTable("UserTenants", "auth")
            .HasKey(ut => new { ut.UserId, ut.TenantId });

        modelBuilder.Entity<UserRole>().ToTable("UserRoles", "auth")
            .HasKey(ur => new { ur.UserId, ur.RoleId });

        modelBuilder.Entity<UserPermission>().ToTable("UserPermissions", "auth")
            .HasKey(up => new { up.UserId, up.PermissionId });

        modelBuilder.Entity<RolePermission>().ToTable("RolePermissions", "auth")
            .HasKey(rp => new { rp.RoleId, rp.PermissionId });

        modelBuilder.Entity<UserPermission>().ToTable("UserPermissions", "auth")
            .HasKey(up => new { up.UserId, up.PermissionId });

        modelBuilder.Entity<Role>().ToTable("Roles", "auth");
        modelBuilder.Entity<Permission>().ToTable("Permissions", "auth");
        modelBuilder.Entity<Tenant>().ToTable("Tenants", "dbo");

        modelBuilder.Entity<AllowedEmailDomain>().ToTable("AllowedEmailDomains", "auth");
        modelBuilder.Entity<AllowedEmailDomain>()
            .HasIndex(d => new { d.TenantId, d.Domain })
            .IsUnique();
    }
}
