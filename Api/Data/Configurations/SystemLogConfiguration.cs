using Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Data.Configurations;

public class SystemLogConfiguration : IEntityTypeConfiguration<SystemLog>
{
  public void Configure(EntityTypeBuilder<SystemLog> builder)
  {
    builder.Property(sl => sl.Action)
           .IsRequired()
           .HasMaxLength(255);

    builder.Property(sl => sl.GeneratedBy)
           .HasMaxLength(100);

    builder.Property(sl => sl.IpAddress)
           .HasMaxLength(45); // IPv6 safe

    builder.Property(sl => sl.CreatedAt)
           .IsRequired();

    builder.HasOne(sl => sl.User)
           .WithMany()
           .HasForeignKey(sl => sl.UserId)
           .OnDelete(DeleteBehavior.SetNull);
  }
}
