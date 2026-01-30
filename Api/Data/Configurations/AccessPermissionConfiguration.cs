using Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Data.Configurations;

public class AccessPermissionConfiguration : IEntityTypeConfiguration<AccessPermission>
{
  public void Configure(EntityTypeBuilder<AccessPermission> builder)
  {
    builder.HasIndex(ap => new { ap.UserId, ap.SystemResourceId })
           .IsUnique()
           .HasDatabaseName("ux_user_system_resource");

    builder.HasOne(ap => ap.User)
           .WithMany(u => u.AccessPermissions)
           .HasForeignKey(ap => ap.UserId)
           .OnDelete(DeleteBehavior.Restrict);

    builder.HasOne(ap => ap.SystemResource)
           .WithMany(r => r.AccessPermissions)
           .HasForeignKey(ap => ap.SystemResourceId)
           .OnDelete(DeleteBehavior.Restrict);
  }
}
