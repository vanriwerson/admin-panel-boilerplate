using Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Data.Configurations
{
  public class SystemLogConfiguration : IEntityTypeConfiguration<SystemLog>
  {
    public void Configure(EntityTypeBuilder<SystemLog> builder)
    {
      builder.HasOne(sl => sl.User)
          .WithMany()
          .HasForeignKey(sl => sl.UserId)
          .OnDelete(DeleteBehavior.Restrict)
          .IsRequired();

      builder.Property(sl => sl.Action)
          .IsRequired()
          .HasMaxLength(255);

      builder.Property(sl => sl.CreatedAt)
          .IsRequired();
    }
  }
}
