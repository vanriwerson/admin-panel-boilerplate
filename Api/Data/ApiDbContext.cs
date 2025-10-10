using Microsoft.EntityFrameworkCore;
using Api.Models;
using Api.Helpers;
using System.Reflection;

namespace Api.Data
{
  public class ApiDbContext : DbContext
  {
    public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

      foreach (var entity in modelBuilder.Model.GetEntityTypes())
      {
        entity.SetTableName(SnakeCaseNaming.ToSnakeCase(entity.GetTableName()!));

        foreach (var property in entity.GetProperties())
          property.SetColumnName(SnakeCaseNaming.ToSnakeCase(property.Name));

        foreach (var key in entity.GetKeys())
          key.SetName(SnakeCaseNaming.ToSnakeCase(key.GetName()!));

        foreach (var index in entity.GetIndexes())
          index.SetDatabaseName(SnakeCaseNaming.ToSnakeCase(index.GetDatabaseName()!));

        foreach (var fk in entity.GetForeignKeys())
          fk.SetConstraintName(SnakeCaseNaming.ToSnakeCase(fk.GetConstraintName()!));
      }

      // Aplicar todas as configurações externas IEntityTypeConfiguration
      modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
  }
}
