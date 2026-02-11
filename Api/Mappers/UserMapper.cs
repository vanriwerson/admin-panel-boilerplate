using Api.Dtos;
using Api.Models;

namespace Api.Mappers;

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
            Permissions = user.AccessPermissions?
                .Select(ap => new SystemResourceSelectDto
                {
                    Id = ap.SystemResource.Id,
                    ExhibitionName = ap.SystemResource.ExhibitionName
                })
                .ToList() ?? new List<SystemResourceSelectDto>()
        };
    }
}
