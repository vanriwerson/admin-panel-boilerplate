using Api.Interfaces.Security.Policies;
using Api.Models;
using Api.Repositories;
using Api.Services.Users;
using Api.Tests.Integration.Infrastructure;
using Api.Tests.TestData.Entities;
using FluentAssertions;
using Moq;

namespace Api.Tests.Integration.Services.Users;

[Collection("PostgreSql")]
public class GetAllUsersTests
{
    private readonly PostgreSqlTestFixture _fixture;
    private readonly Mock<IUserVisibilityPolicy> _visibilityMock;

    public GetAllUsersTests(
        PostgreSqlTestFixture fixture)
    {
        _fixture = fixture;

        _visibilityMock =
            new Mock<IUserVisibilityPolicy>();
    }

    #region ExecuteAsync

    [Fact]
    public async Task ExecuteAsync_ShouldReturnPagedUsers()
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
            new GetAllUsers(
                repository,
                _visibilityMock.Object);

        var result =
            await service.ExecuteAsync();

        result.Should().NotBeNull();

        result.TotalItems.Should().Be(2);

        result.Data.Should().HaveCount(2);

        result.Data.Should()
            .Contain(u => u.Username == "bruno");

        result.Data.Should()
            .Contain(u => u.Username == "maria");
    }

    #endregion
}