using Api.Dtos;

namespace Api.Dtos
{
  public class AccessPermissionReadDto
  {
    public int Id { get; set; }
    public int UserId { get; set; }
    public int SystemResourceId { get; set; }
    public SystemResourceOptionDto? SystemResource { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
  }
}
