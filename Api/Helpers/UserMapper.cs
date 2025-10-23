using System.Collections.Generic;
using System.Linq;
using Api.Dtos;
using Api.Models;

namespace Api.Helpers
{
  public static class UserMapper
  {
    public static UserReadDto MapToUserReadDto(User user)
    {
      return new UserReadDto
      {
        Id = user.Id,
        Username = user.Username,
        Email = user.Email,
        FullName = user.FullName,
        Active = user.Active,
        CreatedAt = user.CreatedAt,
        UpdatedAt = user.UpdatedAt,

        Permissions = user.AccessPermissions?
            .Where(ap => ap.SystemResource != null && ap.SystemResource.Active)
            .Select(ap => new SystemResourceOptionDto
            {
              Id = ap.SystemResource!.Id,
              Name = ap.SystemResource.Name,
              ExhibitionName = ap.SystemResource.ExhibitionName
            })
            .ToList() ?? new List<SystemResourceOptionDto>()
      };
    }
  }
}
