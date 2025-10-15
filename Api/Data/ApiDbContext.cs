using Microsoft.EntityFrameworkCore;
using Api.Models;
using System.Reflection;

namespace Api.Data
{
  public class ApiDbContext : DbContext
  {
    public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<SystemResource> SystemResources { get; set; } = null!;
    public DbSet<AccessPermission> AccessPermissions { get; set; } = null!;
    public DbSet<SystemLog> SystemLogs { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

      modelBuilder.Entity<AccessPermission>()
          .HasOne(ap => ap.User)
          .WithMany(u => u.AccessPermissions)
          .HasForeignKey(ap => ap.UserId)
          .OnDelete(DeleteBehavior.Cascade);

      modelBuilder.Entity<AccessPermission>()
          .HasOne(ap => ap.SystemResource)
          .WithMany()
          .HasForeignKey(ap => ap.SystemResourceId)
          .OnDelete(DeleteBehavior.Cascade);

      modelBuilder.Entity<SystemLog>()
          .HasOne(sl => sl.User)
          .WithMany()
          .HasForeignKey(sl => sl.UserId)
          .OnDelete(DeleteBehavior.Restrict);

      modelBuilder.Entity<User>()
          .HasIndex(u => u.Username)
          .IsUnique();

      modelBuilder.Entity<User>()
          .HasIndex(u => u.Email)
          .IsUnique();

      modelBuilder.Entity<SystemResource>()
          .HasIndex(r => r.Name)
          .IsUnique();

      modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
  }
}
