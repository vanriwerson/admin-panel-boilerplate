using Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
       public void Configure(EntityTypeBuilder<User> builder)
       {
              builder.ToTable("users");

              builder.HasKey(u => u.Id);

              builder.Property(u => u.Id)
                     .HasColumnName("id");

              builder.Property(u => u.Username)
                     .HasColumnName("username")
                     .IsRequired()
                     .HasMaxLength(50);

              builder.Property(u => u.Email)
                     .HasColumnName("email")
                     .IsRequired()
                     .HasMaxLength(120);

              builder.Property(u => u.Password)
                     .HasColumnName("password")
                     .IsRequired();

              builder.Property(u => u.FullName)
                     .HasColumnName("full_name")
                     .IsRequired()
                     .HasMaxLength(120);

              builder.Property(u => u.Active)
                     .HasColumnName("active")
                     .IsRequired();

              builder.Property(u => u.CreatedAt)
                     .HasColumnName("created_at")
                     .IsRequired();

              builder.Property(u => u.UpdatedAt)
                     .HasColumnName("updated_at")
                     .IsRequired();

              builder.HasIndex(u => u.Username)
                     .IsUnique()
                     .HasDatabaseName("ux_users_username");

              builder.HasIndex(u => u.Email)
                     .IsUnique()
                     .HasDatabaseName("ux_users_email");

              builder.HasMany(u => u.AccessPermissions)
                     .WithOne(ap => ap.User)
                     .HasForeignKey(ap => ap.UserId)
                     .OnDelete(DeleteBehavior.Restrict);
       }
}
