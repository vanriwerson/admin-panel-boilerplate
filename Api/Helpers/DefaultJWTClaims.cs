using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using Api.Dtos;
using Api.Models;

namespace Api.Helpers
{
  public static class DefaultJWTClaims
  {
    public static Claim[] Generate(User user)
    {
      if (user == null)
        throw new ArgumentNullException(nameof(user));

      var claims = new List<Claim>
            {
                new Claim("id", user.Id.ToString()),
                new Claim("username", user.Username),
                new Claim("fullName", user.FullName),
                new Claim("email", user.Email)
            };

      if (user.AccessPermissions != null && user.AccessPermissions.Any())
      {
        var permissionDtos = user.AccessPermissions
            .Where(p => p.SystemResource != null)
            .Select(p => new SystemResourceOptionDto
            {
              Id = p.SystemResource!.Id,
              Name = p.SystemResource!.Name,
              ExhibitionName = p.SystemResource!.ExhibitionName
            })
            .ToArray();

        claims.Add(new Claim("permissions", JsonSerializer.Serialize(permissionDtos)));
      }

      return claims.ToArray();
    }
  }
}
