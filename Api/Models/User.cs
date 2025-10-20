using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Models
{
  [Table("users")]
  public class User
  {
    [Key]
    public int Id { get; set; }

    [Required]
    [Column("username")]
    public required string Username { get; set; }

    [Required]
    [Column("email")]
    public required string Email { get; set; }

    [Required]
    [Column("password")]
    public required string Password { get; set; }

    [Required]
    [Column("full_name")]
    public required string FullName { get; set; }

    [Required]
    [Column("active")]
    public bool Active { get; set; } = true;
    public ICollection<AccessPermission> AccessPermissions { get; set; } = new List<AccessPermission>();

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
  }
}
