using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Models
{
  [Table("system_resources")]
  public class SystemResource
  {
    [Key]
    public int Id { get; set; }

    [Required]
    [Column("name")]
    [MaxLength(100)]
    public required string Name { get; set; }

    [Required]
    [Column("exhibition_name")]
    [MaxLength(100)]
    public required string ExhibitionName { get; set; }

    [Required]
    [Column("active")]
    public bool Active { get; set; } = true;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
  }
}
