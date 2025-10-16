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

      // Aplica configurações de IEntityTypeConfiguration<T>
      modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
  }
}
