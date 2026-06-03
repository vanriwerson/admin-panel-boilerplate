using Api.Data;
using Api.Dtos;
using Api.Interfaces.Auditing.Services;
using Api.Middlewares;
using Api.Models;
using Api.Repositories;
using Api.Security.Permissions;
using Api.Security.Policies;
using Api.Services.AccessPermissions;
using Api.Services.Users;
using Api.Services.Users.Orchestrators;
using Api.Tests.Integration.Helpers;
using Api.Tests.Integration.Infrastructure;
using Api.Tests.TestData.Entities;
using FluentAssertions;
using Moq;

namespace Api.Tests.Integration.Services.Users.Orchestrators;

[Collection("PostgreSql")]
public class UpdateUserWithAccessGrantedTests
{
    private readonly PostgreSqlTestFixture _fixture;

    public UpdateUserWithAccessGrantedTests(
        PostgreSqlTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task ExecuteAsync_Should_Update_User()
    {
        await using var context =
            _fixture.CreateContext();

        await DatabaseCleaner.ResetAsync(context);
        await DatabaseSeeder.SeedPermissionsAsync(context);

        var user =
            await CreateUserAsync(context);

        var service =
            CreateService(
                context,
                true,
                out _);

        var dto =
            new UserUpdateDto
            {
                Id = user.Id,
                Username = "updated-user",
                Email = "updated@email.com",
                FullName = "Updated Name"
            };

        await service.ExecuteAsync(dto);

        var updated =
            await context.Users.FindAsync(user.Id);

        updated!.Username.Should().Be(dto.Username);
        updated.Email.Should().Be(dto.Email);
        updated.FullName.Should().Be(dto.FullName);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Replace_Permissions()
    {
        await using var context =
            _fixture.CreateContext();

        await DatabaseCleaner.ResetAsync(context);
        await DatabaseSeeder.SeedPermissionsAsync(context);

        var user =
            await CreateUserAsync(
                context,
                BasePermissions.USERS);

        var service =
            CreateService(
                context,
                true,
                out _);

        var dto =
            new UserUpdateDto
            {
                Id = user.Id,
                PermissionIds =
                [
                    BasePermissions.REPORTS,
                    BasePermissions.RESOURCES
                ]
            };

        await service.ExecuteAsync(dto);

        var permissions =
            context.AccessPermissions
                .Where(x => x.UserId == user.Id)
                .Select(x => x.SystemResourceId)
                .ToList();

        permissions.Should()
            .BeEquivalentTo(dto.PermissionIds);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Call_CreateSystemLog()
    {
        await using var context =
            _fixture.CreateContext();

        await DatabaseCleaner.ResetAsync(context);
        await DatabaseSeeder.SeedPermissionsAsync(context);

        var user =
            await CreateUserAsync(context);

        var service =
            CreateService(
                context,
                true,
                out var logMock);

        await service.ExecuteAsync(
            new UserUpdateDto
            {
                Id = user.Id,
                FullName = "Updated"
            });

        logMock.Verify(
            x => x.ExecuteAsync(
                It.IsAny<string>(),
                It.IsAny<int?>(),
                It.IsAny<string?>(),
                It.IsAny<SystemLogDataDto>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Throw_When_NonRoot_GrantsRoot()
    {
        await using var context =
            _fixture.CreateContext();

        await DatabaseCleaner.ResetAsync(context);
        await DatabaseSeeder.SeedPermissionsAsync(context);

        var user =
            await CreateUserAsync(context);

        var service =
            CreateService(
                context,
                false,
                out _);

        Func<Task> act =
            () => service.ExecuteAsync(
                new UserUpdateDto
                {
                    Id = user.Id,
                    PermissionIds =
                    [
                        BasePermissions.ROOT
                    ]
                });

        await act.Should()
            .ThrowAsync<AppException>()
            .WithMessage(
                "Apenas usuário ROOT pode conceder permissão ROOT.");
    }

    private UpdateUserWithAccessGranted CreateService(
        ApiDbContext context,
        bool isRoot,
        out Mock<ICreateSystemLog> logMock)
    {
        var userRepository =
            new UserRepository(context);

        var accessPermissionRepository =
            new AccessPermissionRepository(context);

        var validator =
            new Api.Validations.UserValidator(
                userRepository);

        var updateUser =
            new UpdateUser(
                userRepository,
                validator);

        var createAccessPermissions =
            new CreateAccessPermissions(
                accessPermissionRepository);

        logMock =
            new Mock<ICreateSystemLog>();

        var currentUser =
            CurrentUserContextFactory.Create(
                userId: 1,
                username: "root",
                permissions: isRoot
                    ? [BasePermissions.ROOT]
                    : []);

        var policy =
            new AccessPermissionPolicy(
                currentUser);

        return new UpdateUserWithAccessGranted(
            context,
            updateUser,
            userRepository,
            accessPermissionRepository,
            createAccessPermissions,
            logMock.Object,
            policy);
    }

    private static async Task<User> CreateUserAsync(
        ApiDbContext context,
        params int[] permissions)
    {
        var user = new User
        {
            Username = Guid.NewGuid().ToString(),
            Email = $"{Guid.NewGuid()}@email.com",
            FullName = "Test User",
            Password = "hash",
            Active = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();

        if (permissions.Any())
        {
            context.AccessPermissions.AddRange(
                permissions.Select(p =>
                    new AccessPermission
                    {
                        UserId = user.Id,
                        SystemResourceId = p,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    }));
        }

        await context.SaveChangesAsync();

        return user;
    }

    [Fact]
    public async Task ExecuteAsync_Should_Remove_All_Permissions_When_Empty_List_Is_Provided()
    {
        await using var context =
            _fixture.CreateContext();

        await DatabaseCleaner.ResetAsync(context);
        await DatabaseSeeder.SeedPermissionsAsync(context);

        var user = UserFactory.Create();

        context.Users.Add(user);

        await context.SaveChangesAsync();

        context.AccessPermissions.AddRange(
            new Api.Models.AccessPermission
            {
                UserId = user.Id,
                SystemResourceId = BasePermissions.USERS
            },
            new Api.Models.AccessPermission
            {
                UserId = user.Id,
                SystemResourceId = BasePermissions.REPORTS
            });

        await context.SaveChangesAsync();

        var service =
            CreateService(
                context,
                isRoot: true,
                out _);

        var dto = new UserUpdateDto
        {
            Id = user.Id,
            PermissionIds = []
        };

        await service.ExecuteAsync(dto);

        var permissions =
            context.AccessPermissions
                .Where(x => x.UserId == user.Id)
                .ToList();

        permissions.Should().BeEmpty();
    }

    [Fact]
    public async Task ExecuteAsync_Should_Not_Change_Permissions_When_PermissionIds_Is_Null()
    {
        await using var context =
            _fixture.CreateContext();

        await DatabaseCleaner.ResetAsync(context);
        await DatabaseSeeder.SeedPermissionsAsync(context);

        var user = UserFactory.Create();

        context.Users.Add(user);

        await context.SaveChangesAsync();

        context.AccessPermissions.Add(
            new Api.Models.AccessPermission
            {
                UserId = user.Id,
                SystemResourceId = BasePermissions.USERS
            });

        await context.SaveChangesAsync();

        var service =
            CreateService(
                context,
                isRoot: true,
                out _);

        var dto = new UserUpdateDto
        {
            Id = user.Id,
            FullName = "Nome Atualizado",
            PermissionIds = null
        };

        await service.ExecuteAsync(dto);

        var permissions =
            context.AccessPermissions
                .Where(x => x.UserId == user.Id)
                .ToList();

        permissions.Should().HaveCount(1);

        permissions[0]
            .SystemResourceId
            .Should()
            .Be(BasePermissions.USERS);
    }
}