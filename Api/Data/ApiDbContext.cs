using Api.Models;
using Api.Models.Common;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Api.Data;

public class ApiDbContext : DbContext
{
  public ApiDbContext(DbContextOptions<ApiDbContext> options)
      : base(options)
  {
  }

  public DbSet<User> Users => Set<User>();
  public DbSet<SystemResource> SystemResources => Set<SystemResource>();
  public DbSet<AccessPermission> AccessPermissions => Set<AccessPermission>();
  public DbSet<SystemLog> SystemLogs => Set<SystemLog>();

  //Adicione novos DbSets aqui

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);

    // Aplica automaticamente todas as IEntityTypeConfiguration<T>
    modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
  }

  public override int SaveChanges()
  {
    ApplyAuditInfo();
    return base.SaveChanges();
  }

  public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
  {
    ApplyAuditInfo();
    return base.SaveChangesAsync(cancellationToken);
  }

  private void ApplyAuditInfo()
  {
    var entries = ChangeTracker
        .Entries<AuditableEntity>()
        .Where(e =>
            e.State == EntityState.Added ||
            e.State == EntityState.Modified);

    foreach (var entry in entries)
    {
      var now = DateTime.UtcNow;

      if (entry.State == EntityState.Added)
      {
        entry.Entity.CreatedAt = now;
        entry.Entity.UpdatedAt = now;
      }

      if (entry.State == EntityState.Modified)
      {
        // Evita sobrescrever CreatedAt
        entry.Property(p => p.CreatedAt).IsModified = false;
        entry.Entity.UpdatedAt = now;
      }
    }
  }
}

