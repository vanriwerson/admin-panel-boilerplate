namespace Api.Dtos
{
  public class AccessPermissionReadDto
  {
    public int Id { get; set; }

    public required int UserId { get; set; }
    public required int SystemResourceId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
  }
}
