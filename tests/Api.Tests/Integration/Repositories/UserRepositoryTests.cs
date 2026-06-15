using Api.Models;
using Api.Repositories;
using Api.Tests.Integration.Infrastructure;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Api.Tests.Integration.Repositories;

[Collection("PostgreSql")]
public class UserRepositoryTests
{
    private readonly PostgreSqlTestFixture _fixture;

    public UserRepositoryTests(
        PostgreSqlTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task CreateAsync_Should_Add_User()
    {
        await using var context =
            _fixture.CreateContext();

        await DatabaseCleaner.ResetAsync(context);

        var repository =
            new UserRepository(context);

        var user = new User
        {
            Username = "bruno",
            Email = "bruno@email.com",
            FullName = "Bruno Silva",
            Password = "hash"
        };

        await repository.CreateAsync(user);
        await context.SaveChangesAsync();

        var created =
            await context.Users
                .FirstOrDefaultAsync();

        created.Should().NotBeNull();
        created!.Username.Should().Be("bruno");
    }

    [Fact]
    public async Task UpdateAsync_Should_Update_User()
    {
        await using var context =
            _fixture.CreateContext();

        await DatabaseCleaner.ResetAsync(context);

        var user = new User
        {
            Username = "old",
            Email = "old@email.com",
            FullName = "Old User",
            Password = "hash"
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();

        var repository =
            new UserRepository(context);

        user.FullName = "New User";

        await repository.UpdateAsync(user);
        await context.SaveChangesAsync();

        var updated =
            await context.Users.FindAsync(user.Id);

        updated!.FullName
            .Should()
            .Be("New User");
    }

    [Fact]
    public async Task SoftDeleteAsync_Should_Deactivate_User()
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

        context.Users.Add(user);
        await context.SaveChangesAsync();

        var repository =
            new UserRepository(context);

        var result =
            await repository.SoftDeleteAsync(user.Id);

        result.Should().BeTrue();

        var stored =
            await context.Users.FindAsync(user.Id);

        stored!.Active.Should().BeFalse();
    }

    [Fact]
    public async Task SoftDeleteAsync_Should_Return_False_When_NotFound()
    {
        await using var context =
            _fixture.CreateContext();

        var repository =
            new UserRepository(context);

        var result =
            await repository.SoftDeleteAsync(999);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_User_With_Permissions()
    {
        await using var context =
            _fixture.CreateContext();

        await DatabaseCleaner.ResetAsync(context);

        var resource = new SystemResource
        {
            Name = "USERS",
            ExhibitionName = "Usuários"
        };

        context.SystemResources.Add(resource);
        await context.SaveChangesAsync();

        var user = new User
        {
            Username = "bruno",
            Email = "bruno@email.com",
            FullName = "Bruno",
            Password = "hash"
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();

        context.AccessPermissions.Add(
            new AccessPermission
            {
                UserId = user.Id,
                SystemResourceId = resource.Id
            });

        await context.SaveChangesAsync();

        var repository =
            new UserRepository(context);

        var result =
            await repository.GetByIdAsync(user.Id);

        result.Should().NotBeNull();

        result!.AccessPermissions
            .Should()
            .ContainSingle();
    }

    [Fact]
    public async Task ExistsByUsernameAsync_Should_Return_True()
    {
        await using var context =
            _fixture.CreateContext();

        await DatabaseCleaner.ResetAsync(context);

        context.Users.Add(
            new User
            {
                Username = "bruno",
                Email = "email@test.com",
                FullName = "Bruno",
                Password = "hash"
            });

        await context.SaveChangesAsync();

        var repository =
            new UserRepository(context);

        var exists =
            await repository.ExistsByUsernameAsync("bruno");

        exists.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsByEmailAsync_Should_Return_True()
    {
        await using var context =
            _fixture.CreateContext();

        await DatabaseCleaner.ResetAsync(context);

        context.Users.Add(
            new User
            {
                Username = "bruno",
                Email = "bruno@email.com",
                FullName = "Bruno",
                Password = "hash"
            });

        await context.SaveChangesAsync();

        var repository =
            new UserRepository(context);

        var exists =
            await repository.ExistsByEmailAsync(
                "bruno@email.com");

        exists.Should().BeTrue();
    }

    [Fact]
    public async Task SearchQuery_Should_Filter_Users()
    {
        await using var context =
            _fixture.CreateContext();

        await DatabaseCleaner.ResetAsync(context);

        context.Users.AddRange(
            new User
            {
                Username = "bruno",
                Email = "bruno@email.com",
                FullName = "Bruno Silva",
                Password = "hash"
            },
            new User
            {
                Username = "maria",
                Email = "maria@email.com",
                FullName = "Maria",
                Password = "hash"
            });

        await context.SaveChangesAsync();

        var repository =
            new UserRepository(context);

        var result =
            await repository
                .SearchQuery("bruno")
                .ToListAsync();

        result.Should().HaveCount(1);
        result[0].Username.Should().Be("bruno");
    }
}