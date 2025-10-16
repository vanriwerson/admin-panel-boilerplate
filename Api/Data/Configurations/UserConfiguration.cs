using Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Data.Configurations
{
  public class UserConfiguration : IEntityTypeConfiguration<User>
  {
    public void Configure(EntityTypeBuilder<User> builder)
    {
      builder.HasIndex(u => u.Username).IsUnique();
      builder.HasIndex(u => u.Email).IsUnique();

      builder.Property(u => u.Username).IsRequired();
      builder.Property(u => u.Email).IsRequired();
      builder.Property(u => u.Password).IsRequired();

      builder.HasMany(u => u.AccessPermissions)
             .WithOne(ap => ap.User)
             .HasForeignKey(ap => ap.UserId)
             .OnDelete(DeleteBehavior.Restrict);
    }
  }
}
