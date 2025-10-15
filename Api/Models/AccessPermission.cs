using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Models
{
  [Table("access_permissions")]
  public class AccessPermission
  {
    [Key]
    public int Id { get; set; }

    [Required]
    [Column("user_id")]
    public required int UserId { get; set; }

    [Required]
    [Column("system_resource_id")]
    public required int SystemResourceId { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey(nameof(UserId))]
    public User? User { get; set; }

    [ForeignKey(nameof(SystemResourceId))]
    public SystemResource? SystemResource { get; set; }
  }
}
