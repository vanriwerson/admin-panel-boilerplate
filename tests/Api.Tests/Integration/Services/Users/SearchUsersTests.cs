using Api.Interfaces.Security.Policies;
using Api.Middlewares;
using Api.Models;
using Api.Repositories;
using Api.Services.Users;
using Api.Tests.Integration.Infrastructure;
using Api.Tests.TestData.Entities;
using FluentAssertions;
using Moq;

namespace Api.Tests.Integration.Services.Users;

[Collection("PostgreSql")]
public class SearchUsersTests
{
    private readonly PostgreSqlTestFixture _fixture;
    private readonly Mock<IUserVisibilityPolicy> _visibilityMock;

    public SearchUsersTests(
        PostgreSqlTestFixture fixture)
    {
        _fixture = fixture;

        _visibilityMock =
            new Mock<IUserVisibilityPolicy>();
    }

    #region ExecuteAsync

    [Fact]
    public async Task ExecuteAsync_ShouldThrow_WhenKeyIsEmpty()
    {
        await using var context =
            _fixture.CreateContext();

        await DatabaseCleaner.ResetAsync(context);

        var repository =
            new UserRepository(context);

        var service =
            new SearchUsers(
                repository,
                _visibilityMock.Object);

        Func<Task> act =
            () => service.ExecuteAsync("");

        await act.Should()
            .ThrowAsync<AppException>()
            .WithMessage("'key' é obrigatório.");
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnFilteredUsers()
    {
        await using var context =
            _fixture.CreateContext();

        await DatabaseCleaner.ResetAsync(context);

        context.Users.AddRange(
            UserFactory.Create(u =>
            {
                u.Username = "bruno";
                u.Email = "bruno@email.com";
                u.FullName = "Bruno Silva";
            }),
            UserFactory.Create(u =>
            {
                u.Username = "maria";
                u.Email = "maria@email.com";
                u.FullName = "Maria Souza";
            }));

        await context.SaveChangesAsync();

        var repository =
            new UserRepository(context);

        _visibilityMock
            .Setup(v =>
                v.ApplyToQuery(It.IsAny<IQueryable<User>>()))
            .Returns<IQueryable<User>>(q => q);

        var service =
            new SearchUsers(
                repository,
                _visibilityMock.Object);

        var result =
            await service.ExecuteAsync("bruno");

        result.Should().NotBeNull();

        result.TotalItems.Should().Be(1);

        result.Data.Should().HaveCount(1);

        result.Data.First().Username
            .Should().Be("bruno");
    }

    #endregion
}