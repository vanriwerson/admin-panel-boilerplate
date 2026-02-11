using Api.Security.Jwt;
using Api.Security.Permissions;

namespace Api.Security.Policies;

public class AccessPermissionPolicy
{
    private readonly CurrentUserContext _currentUser;

    public AccessPermissionPolicy(CurrentUserContext currentUser)
    {
        _currentUser = currentUser;
    }

    public void EnsureCanGrant(IEnumerable<int> permissionIds)
    {
        if (!_currentUser.IsAuthenticated)
            throw new UnauthorizedAccessException("Usuário não autenticado.");

        if (permissionIds.Contains(BasePermissions.ROOT)
            && !_currentUser.IsRoot())
        {
            throw new UnauthorizedAccessException(
                "Apenas usuário ROOT pode conceder permissão ROOT.");
        }
    }
}
