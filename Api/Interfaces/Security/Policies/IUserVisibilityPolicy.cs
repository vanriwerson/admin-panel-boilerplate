using Api.Models;

namespace Api.Interfaces.Security.Policies;

public interface IUserVisibilityPolicy
{
    IQueryable<User> ApplyToQuery(IQueryable<User> query);

    bool CanAccess(User target);
}