namespace Api.Dtos
{
  public class SystemResourceReadDto
  {
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string ExhibitionName { get; set; }
    public bool Active { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
  }
}
