using Api.Models;
using Api.Security.Permissions;

namespace Api.Tests.TestData.Entities;

public static class UserFactory
{
    public static User Create(
        Action<User>? overrideValues = null)
    {
        var user = new User
        {
            Username = "bruno",
            Email = "bruno@email.com",
            FullName = "Bruno Silva",
            Password = "hashed_password",
            Active = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            AccessPermissions = []
        };

        overrideValues?.Invoke(user);

        return user;
    }

    public static User RootUser()
        => Create(u =>
        {
            u.AccessPermissions =
            [
                new AccessPermission
                {
                    SystemResourceId = BasePermissions.ROOT
                }
            ];
        });
}