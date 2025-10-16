using Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Data.Configurations
{
  public class AccessPermissionConfiguration : IEntityTypeConfiguration<AccessPermission>
  {
    public void Configure(EntityTypeBuilder<AccessPermission> builder)
    {
      builder.HasOne(ap => ap.User)
             .WithMany(u => u.AccessPermissions)
             .HasForeignKey(ap => ap.UserId)
             .OnDelete(DeleteBehavior.Restrict);

      builder.HasOne(ap => ap.SystemResource)
             .WithMany()
             .HasForeignKey(ap => ap.SystemResourceId)
             .OnDelete(DeleteBehavior.Restrict);

      builder.HasIndex(ap => new { ap.UserId, ap.SystemResourceId })
             .IsUnique();
    }
  }
}
