using Microsoft.EntityFrameworkCore;
using Movies.Domain.Entities;

namespace Movies.Infrastructure.Persistence
{
   public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
   {
      public DbSet<Movie> Movies => Set<Movie>();
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
      }
   }
}
