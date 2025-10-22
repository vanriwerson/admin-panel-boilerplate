using Api.Dtos;

namespace Api.Dtos
{
  public class SystemLogReadDto
  {
    public int Id { get; set; }

    public int UserId { get; set; }

    public string Action { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public UserLogReadDto? User { get; set; }
  }
}
