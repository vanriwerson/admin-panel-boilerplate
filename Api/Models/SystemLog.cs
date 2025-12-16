using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Models
{
  [Table("system_logs")]
  public class SystemLog
  {
    [Key]
    public int Id { get; set; }

    [Required]
    [Column("user_id")]
    public required int UserId { get; set; }

    [Required]
    [Column("action")]
    [MaxLength(255)]
    public required string Action { get; set; }

    [Column("used_payload")]
    public string? UsedPayload { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey(nameof(UserId))]
    public User? User { get; set; }
  }
}
