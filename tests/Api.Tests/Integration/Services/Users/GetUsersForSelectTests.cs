using Api.Data;
using Api.Models;
using Api.Repositories;
using Api.Services.Users;
using Api.Tests.Integration.Infrastructure;
using FluentAssertions;

namespace Api.Tests.Integration.Services.Users;

[Collection("PostgreSql")]
public class GetUsersForSelectTests
{
    private readonly PostgreSqlTestFixture _fixture;

    public GetUsersForSelectTests(
        PostgreSqlTestFixture fixture)
    {
        _fixture = fixture;
    }

    #region ExecuteAsync

    [Fact]
    public async Task ExecuteAsync_Should_Return_Users_For_Select()
    {
        await using var context =
            _fixture.CreateContext();

        await DatabaseCleaner.ResetAsync(context);

        context.Users.AddRange(
            new User
            {
                Username = "user1",
                Email = "user1@email.com",
                FullName = "User One",
                Password = "hash"
            },
            new User
            {
                Username = "user2",
                Email = "user2@email.com",
                FullName = "User Two",
                Password = "hash"
            });

        await context.SaveChangesAsync();

        var repository =
            new UserRepository(context);

        var service =
            new GetUsersForSelect(repository);

        var result =
            (await service.ExecuteAsync())
            .ToList();

        result.Should().HaveCount(2);

        result.Should().Contain(x =>
            x.FullName == "User One");

        result.Should().Contain(x =>
            x.FullName == "User Two");
    }

    [Fact]
    public async Task ExecuteAsync_Should_Return_Ordered_By_FullName()
    {
        await using var context =
            _fixture.CreateContext();

        await DatabaseCleaner.ResetAsync(context);

        context.Users.AddRange(
            new User
            {
                Username = "charlie",
                Email = "charlie@email.com",
                FullName = "Charlie",
                Password = "hash"
            },
            new User
            {
                Username = "alice",
                Email = "alice@email.com",
                FullName = "Alice",
                Password = "hash"
            },
            new User
            {
                Username = "bob",
                Email = "bob@email.com",
                FullName = "Bob",
                Password = "hash"
            });

        await context.SaveChangesAsync();

        var repository =
            new UserRepository(context);

        var service =
            new GetUsersForSelect(repository);

        var result =
            (await service.ExecuteAsync())
            .ToList();

        result.Select(x => x.FullName)
            .Should()
            .ContainInOrder(
                "Alice",
                "Bob",
                "Charlie");
    }

    [Fact]
    public async Task ExecuteAsync_Should_Not_Return_Inactive_Users()
    {
        await using var context =
            _fixture.CreateContext();

        await DatabaseCleaner.ResetAsync(context);

        context.Users.AddRange(
            new User
            {
                Username = "active",
                Email = "active@email.com",
                FullName = "Active User",
                Password = "hash",
                Active = true
            },
            new User
            {
                Username = "inactive",
                Email = "inactive@email.com",
                FullName = "Inactive User",
                Password = "hash",
                Active = false
            });

        await context.SaveChangesAsync();

        var repository =
            new UserRepository(context);

        var service =
            new GetUsersForSelect(repository);

        var result =
            (await service.ExecuteAsync())
            .ToList();

        result.Should().HaveCount(1);

        result.Single().FullName
            .Should()
            .Be("Active User");
    }

    [Fact]
    public async Task ExecuteAsync_Should_Return_Empty_When_No_Users_Exist()
    {
        await using var context =
            _fixture.CreateContext();

        await DatabaseCleaner.ResetAsync(context);

        var repository =
            new UserRepository(context);

        var service =
            new GetUsersForSelect(repository);

        var result =
            await service.ExecuteAsync();

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task ExecuteAsync_Should_Return_Only_Id_And_FullName()
    {
        await using var context =
            _fixture.CreateContext();

        await DatabaseCleaner.ResetAsync(context);

        var user = new User
        {
            Username = "bruno",
            Email = "bruno@email.com",
            FullName = "Bruno Silva",
            Password = "hash"
        };

        context.Users.Add(user);

        await context.SaveChangesAsync();

        var repository =
            new UserRepository(context);

        var service =
            new GetUsersForSelect(repository);

        var result =
            (await service.ExecuteAsync())
            .Single();

        result.Id.Should().Be(user.Id);

        result.FullName.Should()
            .Be("Bruno Silva");
    }

    #endregion
}