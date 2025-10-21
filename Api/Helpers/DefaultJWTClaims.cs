using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
        foreach (var permission in user.AccessPermissions)
        {
          if (permission.SystemResource != null)
          {
            claims.Add(new Claim("permission", permission.SystemResource.Id.ToString()));
          }
        }
      }

      return claims.ToArray();
    }
  }
}
