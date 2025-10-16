using Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Data.Configurations
{
  public class SystemResourceConfiguration : IEntityTypeConfiguration<SystemResource>
  {
    public void Configure(EntityTypeBuilder<SystemResource> builder)
    {
      builder.HasIndex(r => r.Name).IsUnique();

      builder.Property(r => r.Name)
             .IsRequired()
             .HasMaxLength(40);

      builder.Property(r => r.ExhibitionName)
             .IsRequired()
             .HasMaxLength(120);

      builder.HasMany<AccessPermission>()
             .WithOne(ap => ap.SystemResource)
             .HasForeignKey(ap => ap.SystemResourceId)
             .OnDelete(DeleteBehavior.Restrict);
    }
  }
}
