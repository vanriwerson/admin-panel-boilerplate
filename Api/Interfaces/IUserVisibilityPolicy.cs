using System.Security.Claims;
using Api.Models;

namespace Api.Interfaces;

public interface IUserVisibilityPolicy
{
    IQueryable<User> ApplyToQuery(
        IQueryable<User> query,
        ClaimsPrincipal currentUser
    );

    bool CanAccess(
        User target,
        ClaimsPrincipal currentUser
    );
}
