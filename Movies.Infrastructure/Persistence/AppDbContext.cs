using Microsoft.EntityFrameworkCore;
using Movies.Domain.Entities;
using Movies.Infrastructure.Auth.Entities;

namespace Movies.Infrastructure.Persistence
{
   public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
   {
      public DbSet<Movie> Movies => Set<Movie>();

      #region Auth Schema
      public DbSet<AuthUser> AuthUsers => Set<AuthUser>();
      public DbSet<IdentityProvider> IdentityProviders => Set<IdentityProvider>();
      public DbSet<UserIdentity> UserIdentities => Set<UserIdentity>();
      public DbSet<Role> Roles => Set<Role>();
      public DbSet<Permission> Permissions => Set<Permission>();
      public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
      public DbSet<UserRole> UserRoles => Set<UserRole>();
      public DbSet<UserPermission> UserPermissions => Set<UserPermission>();
      public DbSet<AllowedEmailDomain> AllowedEmailDomains => Set<AllowedEmailDomain>();
      public DbSet<AuditLogin> AuditLogins => Set<AuditLogin>();
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

         // ===== auth schema =====
         modelBuilder.Entity<AuthUser>(e =>
         {
            e.ToTable("Users", "auth");
            e.HasKey(x => x.Id);
            e.Property(x => x.Email).HasMaxLength(256);
            e.Property(x => x.DisplayName).HasMaxLength(200);
            e.Property(x => x.PasswordHash);
            e.Property(x => x.IsActive).HasDefaultValue(true);
            e.Property(x => x.EmailVerified).HasDefaultValue(false);
            e.Property(x => x.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
            e.HasIndex(x => x.Email).IsUnique().HasFilter("[Email] IS NOT NULL");
         });

         modelBuilder.Entity<IdentityProvider>(e =>
         {
            e.ToTable("IdentityProviders", "auth");
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).HasMaxLength(100).IsRequired();
            e.Property(x => x.IssuerUri).HasMaxLength(400).IsRequired();
            e.HasIndex(x => x.IssuerUri).IsUnique();
         });

         modelBuilder.Entity<UserIdentity>(e =>
         {
            e.ToTable("UserIdentities", "auth");
            e.HasKey(x => x.Id);
            e.Property(x => x.Subject).HasMaxLength(200).IsRequired();
            e.Property(x => x.EmailAtAuth).HasMaxLength(256);
            e.Property(x => x.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
            e.HasIndex(x => new { x.ProviderId, x.Subject }).IsUnique();
            e.HasOne(x => x.User).WithMany(u => u.Identities).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.Provider).WithMany(p => p.UserIdentities).HasForeignKey(x => x.ProviderId).OnDelete(DeleteBehavior.Cascade);
         });

         modelBuilder.Entity<Role>(e =>
         {
            e.ToTable("Roles", "auth");
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).HasMaxLength(100).IsRequired();
            e.Property(x => x.Description).HasMaxLength(300);
            e.Property(x => x.IsActive).HasDefaultValue(true);
            e.HasIndex(x => x.Name).IsUnique();
         });

         modelBuilder.Entity<Permission>(e =>
         {
            e.ToTable("Permissions", "auth");
            e.HasKey(x => x.Id);
            e.Property(x => x.Key).HasMaxLength(150).IsRequired();
            e.Property(x => x.Name).HasMaxLength(150).IsRequired();
            e.Property(x => x.Description).HasMaxLength(300);
            e.Property(x => x.GroupName).HasMaxLength(100);
            e.HasIndex(x => x.Key).IsUnique();
         });

         modelBuilder.Entity<RolePermission>(e =>
         {
            e.ToTable("RolePermissions", "auth");
            e.HasKey(x => new { x.RoleId, x.PermissionId });
            e.HasOne(x => x.Role).WithMany(r => r.RolePermissions).HasForeignKey(x => x.RoleId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.Permission).WithMany(p => p.RolePermissions).HasForeignKey(x => x.PermissionId).OnDelete(DeleteBehavior.Cascade);
         });

         modelBuilder.Entity<UserRole>(e =>
         {
            e.ToTable("UserRoles", "auth");
            e.HasKey(x => new { x.UserId, x.RoleId });
            e.HasOne(x => x.User).WithMany(u => u.UserRoles).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.Role).WithMany(r => r.UserRoles).HasForeignKey(x => x.RoleId).OnDelete(DeleteBehavior.Cascade);
         });

         modelBuilder.Entity<UserPermission>(e =>
         {
            e.ToTable("UserPermissions", "auth");
            e.HasKey(x => new { x.UserId, x.PermissionId });
            e.Property(x => x.IsGranted).IsRequired();
            e.HasOne(x => x.User).WithMany(u => u.UserPermissions).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.Permission).WithMany(p => p.UserPermissions).HasForeignKey(x => x.PermissionId).OnDelete(DeleteBehavior.Cascade);
         });

         modelBuilder.Entity<AllowedEmailDomain>(e =>
         {
            e.ToTable("AllowedEmailDomains", "auth");
            e.HasKey(x => x.Id);
            e.Property(x => x.Domain).HasMaxLength(200).IsRequired();
            e.Property(x => x.IsActive).HasDefaultValue(true);
            e.HasIndex(x => x.Domain).IsUnique();
         });

         modelBuilder.Entity<AuditLogin>(e =>
         {
            e.ToTable("AuditLogins", "auth");
            e.HasKey(x => x.Id);
            e.Property(x => x.OccurredAt).HasDefaultValueSql("SYSUTCDATETIME()");
            e.Property(x => x.Result).HasMaxLength(50).IsRequired();
         });
      }
   }
}
