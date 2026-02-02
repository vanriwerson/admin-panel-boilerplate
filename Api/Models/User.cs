using Api.Models.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Models;

public class User : AuditableEntity
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string FullName { get; set; } = null!;

    public bool Active { get; set; } = true;

    public ICollection<AccessPermission> AccessPermissions { get; set; } = new List<AccessPermission>();
}
