using Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
  public void Configure(EntityTypeBuilder<User> builder)
  {
    builder.HasIndex(u => u.Username).IsUnique();
    builder.HasIndex(u => u.Email).IsUnique();

    builder.Property(u => u.Username)
           .IsRequired()
           .HasMaxLength(50);

    builder.Property(u => u.Email)
           .IsRequired()
           .HasMaxLength(120);

    builder.Property(u => u.Password)
           .IsRequired();

    builder.Property(u => u.FullName)
           .IsRequired()
           .HasMaxLength(120);

    builder.HasMany(u => u.AccessPermissions)
           .WithOne(ap => ap.User)
           .HasForeignKey(ap => ap.UserId)
           .OnDelete(DeleteBehavior.Restrict);
  }
}

