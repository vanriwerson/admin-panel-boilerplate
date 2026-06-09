using Api.Models;
using Api.Repositories;
using Api.Tests.Integration.Infrastructure;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Api.Tests.Integration.Repositories;

[Collection("PostgreSql")]
public class AccessPermissionRepositoryTests
{
    private readonly PostgreSqlTestFixture _fixture;

    public AccessPermissionRepositoryTests(
        PostgreSqlTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task AddRangeAsync_Should_Add_Permissions()
    {
        await using var context =
            _fixture.CreateContext();

        await DatabaseCleaner.ResetAsync(context);

        var user = new User
        {
            Username = "user",
            Email = "user@email.com",
            FullName = "User",
            Password = "hash"
        };

        var resource = new SystemResource
        {
            Name = "USERS",
            ExhibitionName = "Usuários"
        };

        context.Users.Add(user);
        context.SystemResources.Add(resource);

        await context.SaveChangesAsync();

        var repository =
            new AccessPermissionRepository(context);

        await repository.AddRangeAsync(
        [
            new AccessPermission
            {
                UserId = user.Id,
                SystemResourceId = resource.Id
            }
        ]);

        await context.SaveChangesAsync();

        var count =
            await context.AccessPermissions.CountAsync();

        count.Should().Be(1);
    }

    [Fact]
    public async Task RemoveByUserIdAsync_Should_Remove_All_User_Permissions()
    {
        await using var context =
            _fixture.CreateContext();

        await DatabaseCleaner.ResetAsync(context);

        var user = new User
        {
            Username = "user",
            Email = "user@email.com",
            FullName = "User",
            Password = "hash"
        };

        var resource1 = new SystemResource
        {
            Name = "USERS",
            ExhibitionName = "Usuários"
        };

        var resource2 = new SystemResource
        {
            Name = "REPORTS",
            ExhibitionName = "Relatórios"
        };

        context.Users.Add(user);

        context.SystemResources.AddRange(
            resource1,
            resource2);

        await context.SaveChangesAsync();

        context.AccessPermissions.AddRange(
            new AccessPermission
            {
                UserId = user.Id,
                SystemResourceId = resource1.Id
            },
            new AccessPermission
            {
                UserId = user.Id,
                SystemResourceId = resource2.Id
            });

        await context.SaveChangesAsync();

        var repository =
            new AccessPermissionRepository(context);

        await repository.RemoveByUserIdAsync(user.Id);

        await context.SaveChangesAsync();

        var count =
            await context.AccessPermissions.CountAsync();

        count.Should().Be(0);
    }
}