using System.Collections.Generic;

namespace Api.Dtos
{
  public class LoginResponseDto
  {
    public string Token { get; set; } = default!;
    public int Id { get; set; } = default!;
    public string Username { get; set; } = default!;
    public string FullName { get; set; } = default!;
    public IEnumerable<SystemResourceOptionDto> Permissions { get; set; } = [];
  }
}
