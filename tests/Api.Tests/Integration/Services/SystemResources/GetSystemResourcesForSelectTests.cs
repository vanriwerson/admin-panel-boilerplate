using System.Security.Claims;
using Api.Data;
using Api.Models;
using Api.Repositories;
using Api.Security.Jwt;
using Api.Security.Permissions;
using Api.Security.Policies;
using Api.Services.SystemResources;
using Api.Tests.Integration.Infrastructure;
using FluentAssertions;
using Microsoft.AspNetCore.Http;

namespace Api.Tests.Integration.Services.SystemResources;

[Collection("PostgreSql")]
public class GetSystemResourcesForSelectTests
{
    private readonly PostgreSqlTestFixture _fixture;

    public GetSystemResourcesForSelectTests(
        PostgreSqlTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task ExecuteAsync_Should_Return_All_Resources_For_Root_User()
    {
        await using var context =
            _fixture.CreateContext();

        await DatabaseCleaner.ResetAsync(context);

        context.SystemResources.AddRange(
            new SystemResource
            {
                Id = BasePermissions.ROOT,
                Name = "ROOT",
                ExhibitionName = "ROOT"
            },
            new SystemResource
            {
                Id = 100,
                Name = "USERS",
                ExhibitionName = "Usuários"
            },
            new SystemResource
            {
                Id = 101,
                Name = "REPORTS",
                ExhibitionName = "Relatórios"
            });

        await context.SaveChangesAsync();

        var currentUser =
            CreateCurrentUserContext(
                authenticated: true,
                isRoot: true);

        var repository =
            new SystemResourceRepository(context);

        var visibility =
            new SystemResourceVisibilityPolicy(currentUser);

        var service =
            new GetSystemResourcesForSelect(
                repository,
                visibility);

        var result =
            (await service.ExecuteAsync()).ToList();

        result.Should().HaveCount(3);

        result.Select(r => r.Id)
            .Should()
            .Contain(BasePermissions.ROOT)
            .And.Contain(100)
            .And.Contain(101);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Hide_Root_For_NonRoot_User()
    {
        await using var context =
            _fixture.CreateContext();

        await DatabaseCleaner.ResetAsync(context);

        context.SystemResources.AddRange(
            new SystemResource
            {
                Id = BasePermissions.ROOT,
                Name = "ROOT",
                ExhibitionName = "ROOT"
            },
            new SystemResource
            {
                Id = 100,
                Name = "USERS",
                ExhibitionName = "Usuários"
            },
            new SystemResource
            {
                Id = 101,
                Name = "REPORTS",
                ExhibitionName = "Relatórios"
            });

        await context.SaveChangesAsync();

        var currentUser =
            CreateCurrentUserContext(
                authenticated: true,
                isRoot: false);

        var repository =
            new SystemResourceRepository(context);

        var visibility =
            new SystemResourceVisibilityPolicy(currentUser);

        var service =
            new GetSystemResourcesForSelect(
                repository,
                visibility);

        var result =
            (await service.ExecuteAsync()).ToList();

        result.Should().HaveCount(2);

        result.Should().NotContain(
            r => r.Id == BasePermissions.ROOT);

        result.Select(r => r.Id)
            .Should()
            .Contain(100)
            .And.Contain(101);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Return_Empty_For_Unauthenticated_User()
    {
        await using var context =
            _fixture.CreateContext();

        await DatabaseCleaner.ResetAsync(context);

        context.SystemResources.AddRange(
            new SystemResource
            {
                Id = BasePermissions.ROOT,
                Name = "ROOT",
                ExhibitionName = "ROOT"
            },
            new SystemResource
            {
                Id = 100,
                Name = "USERS",
                ExhibitionName = "Usuários"
            });

        await context.SaveChangesAsync();

        var currentUser =
            CreateCurrentUserContext(
                authenticated: false,
                isRoot: false);

        var repository =
            new SystemResourceRepository(context);

        var visibility =
            new SystemResourceVisibilityPolicy(currentUser);

        var service =
            new GetSystemResourcesForSelect(
                repository,
                visibility);

        var result =
            await service.ExecuteAsync();

        result.Should().BeEmpty();
    }

    private static CurrentUserContext CreateCurrentUserContext(
        bool authenticated,
        bool isRoot)
    {
        var claims = new List<Claim>();

        if (isRoot)
        {
            claims.Add(
                new Claim(
                    "permission",
                    BasePermissions.ROOT.ToString()));
        }

        var identity = authenticated
            ? new ClaimsIdentity(claims, "TestAuth")
            : new ClaimsIdentity();

        var principal =
            new ClaimsPrincipal(identity);

        var httpContext =
            new DefaultHttpContext
            {
                User = principal
            };

        var accessor =
            new HttpContextAccessor
            {
                HttpContext = httpContext
            };

        return new CurrentUserContext(accessor);
    }
}