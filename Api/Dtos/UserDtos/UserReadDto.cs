using System;
using System.Collections.Generic;

namespace Api.Dtos
{
  public class UserReadDto
  {
    public int Id { get; set; }
    public string Username { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string FullName { get; set; } = default!;
    public bool Active { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public List<SystemResourceOptionDto> Permissions { get; set; } = new();
  }
}
