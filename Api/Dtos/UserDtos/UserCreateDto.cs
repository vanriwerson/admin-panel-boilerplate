namespace Api.Dtos
{
  public class UserCreateDto
  {
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required string FullName { get; set; }
    public required List<int> Permissions { get; set; } = new();
  }
}
