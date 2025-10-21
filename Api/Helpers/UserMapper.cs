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
              .Select(ap => new AccessPermissionReadDto
              {
                Id = ap.Id,
                UserId = ap.UserId,
                SystemResourceId = ap.SystemResourceId,
                SystemResource = ap.SystemResource != null
                      ? new SystemResourceOptionDto
                      {
                        Id = ap.SystemResource.Id,
                        Name = ap.SystemResource.Name,
                        ExhibitionName = ap.SystemResource.ExhibitionName
                      }
                      : null
              })
              .ToList() ?? new List<AccessPermissionReadDto>()
      };
    }
  }
}
