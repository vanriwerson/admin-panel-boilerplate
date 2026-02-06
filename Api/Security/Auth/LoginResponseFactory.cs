using Api.Dtos;
using Api.Models;
using Api.Security.Jwt;

namespace Api.Security.Auth;

public class LoginResponseFactory
{
    public LoginResponseDto CreateResponse(User user)
    {
        var claims = JwtClaimsFactory.FromUser(user);
        var token = JwtServices.Create(claims);

        var permissions = (user.AccessPermissions ?? [])
            .Where(ap => ap.SystemResource?.Active == true)
            .Select(ap => new SystemResourceReadDto
            {
                Id = ap.SystemResource!.Id,
                Name = ap.SystemResource.Name,
                ExhibitionName = ap.SystemResource.ExhibitionName
            })
            .ToList();

        return new LoginResponseDto
        {
            Token = token,
            Id = user.Id,
            Username = user.Username,
            FullName = user.FullName,
            Permissions = permissions
        };
    }
}
