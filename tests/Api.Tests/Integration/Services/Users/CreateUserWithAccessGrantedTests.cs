using Api.Data;
using Api.Dtos;
using Api.Interfaces.Auditing.Services;
using Api.Middlewares;
using Api.Repositories;
using Api.Security.Permissions;
using Api.Security.Policies;
using Api.Services.AccessPermissions;
using Api.Services.Users;
using Api.Services.Users.Orchestrators;
using Api.Tests.Integration.Helpers;
using Api.Tests.Integration.Infrastructure;
using Api.Tests.TestData.Dtos;
using FluentAssertions;
using Moq;

namespace Api.Tests.Integration.Services.Users.Orchestrators;

[Collection("PostgreSql")]
public class CreateUserWithAccessGrantedTests
{
    private readonly PostgreSqlTestFixture _fixture;

    public CreateUserWithAccessGrantedTests(
        PostgreSqlTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task ExecuteAsync_Should_Create_User()
    {
        await using var context =
            _fixture.CreateContext();

        await DatabaseCleaner.ResetAsync(context);
        await DatabaseSeeder.SeedPermissionsAsync(context);

        var service =
            CreateService(
                context,
                isRoot: true,
                out _);

        var dto =
            UserCreateDtoFactory.Create();

        dto.PermissionIds =
        [
            BasePermissions.USERS
        ];

        var result =
            await service.ExecuteAsync(dto);

        var user =
            await context.Users.FindAsync(result.Id);

        user.Should().NotBeNull();

        user!.Username.Should().Be(dto.Username);
        user.Email.Should().Be(dto.Email);
        user.FullName.Should().Be(dto.FullName);
        user.Active.Should().BeTrue();
    }

    [Fact]
    public async Task ExecuteAsync_Should_Create_AccessPermissions()
    {
        await using var context =
            _fixture.CreateContext();

        await DatabaseCleaner.ResetAsync(context);
        await DatabaseSeeder.SeedPermissionsAsync(context);

        var service =
            CreateService(
                context,
                true,
                out _);

        var dto =
            UserCreateDtoFactory.Create();

        dto.PermissionIds =
        [
            BasePermissions.USERS,
            BasePermissions.REPORTS
        ];

        var result =
            await service.ExecuteAsync(dto);

        var permissions =
            context.AccessPermissions
                .Where(x => x.UserId == result.Id)
                .ToList();

        permissions.Should().HaveCount(2);

        permissions
            .Select(x => x.SystemResourceId)
            .Should()
            .BeEquivalentTo(dto.PermissionIds);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Call_CreateSystemLog()
    {
        await using var context =
            _fixture.CreateContext();

        await DatabaseCleaner.ResetAsync(context);
        await DatabaseSeeder.SeedPermissionsAsync(context);

        var service =
            CreateService(
                context,
                true,
                out var logMock);

        var dto =
            UserCreateDtoFactory.Create();

        dto.PermissionIds =
        [
            BasePermissions.USERS
        ];

        await service.ExecuteAsync(dto);

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

        var service =
            CreateService(
                context,
                false,
                out _);

        var dto =
            UserCreateDtoFactory.Create();

        dto.PermissionIds =
        [
            BasePermissions.ROOT
        ];

        Func<Task> act =
            () => service.ExecuteAsync(dto);

        await act.Should()
            .ThrowAsync<AppException>()
            .WithMessage(
                "Apenas usuário ROOT pode conceder permissão ROOT.");
    }

    private CreateUserWithAccessGranted CreateService(
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

        var createUser =
            new CreateUser(
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

        return new CreateUserWithAccessGranted(
            context,
            createUser,
            createAccessPermissions,
            userRepository,
            logMock.Object,
            policy);
    }
}