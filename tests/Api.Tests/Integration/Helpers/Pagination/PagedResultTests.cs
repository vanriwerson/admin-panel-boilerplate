using Api.Helpers.Pagination;
using Api.Tests.Integration.Infrastructure;
using Api.Tests.TestData.Entities;
using FluentAssertions;

namespace Api.Tests.Integration.Helpers.Pagination;

[Collection("PostgreSql")]
public class PagedResultTests
{
    private readonly PostgreSqlTestFixture _fixture;

    public PagedResultTests(
        PostgreSqlTestFixture fixture)
    {
        _fixture = fixture;
    }

    #region CreateAsync

    [Fact]
    public async Task CreateAsync_ShouldReturnCorrectPagination()
    {
        await using var context =
            _fixture.CreateContext();

        await DatabaseCleaner.ResetAsync(context);

        var users = Enumerable
            .Range(1, 25)
            .Select(i =>
                UserFactory.Create(u =>
                {
                    u.Username = $"user{i}";
                    u.Email = $"user{i}@email.com";
                    u.FullName = $"User {i}";
                }))
            .ToList();

        context.Users.AddRange(users);

        await context.SaveChangesAsync();

        var result = await PagedResult<Api.Models.User>
            .CreateAsync(
                context.Users,
                page: 2,
                pageSize: 10);

        result.Should().NotBeNull();
        result.TotalItems.Should().Be(25);
        result.Page.Should().Be(2);
        result.PageSize.Should().Be(10);
        result.TotalPages.Should().Be(3);
        result.Data.Should().HaveCount(10);
    }

    [Fact]
    public async Task CreateAsync_ShouldDefaultPageToOne_WhenPageIsLessThanOne()
    {
        await using var context =
            _fixture.CreateContext();

        await DatabaseCleaner.ResetAsync(context);

        context.Users.AddRange(
            Enumerable.Range(1, 5)
                .Select(i =>
                    UserFactory.Create(u =>
                    {
                        u.Username = $"user{i}";
                        u.Email = $"user{i}@email.com";
                    })));

        await context.SaveChangesAsync();

        var result = await PagedResult<Api.Models.User>
            .CreateAsync(
                context.Users,
                page: 0,
                pageSize: 10);

        result.Page.Should().Be(1);
    }

    [Fact]
    public async Task CreateAsync_ShouldDefaultPageSizeToTen_WhenPageSizeIsLessThanOne()
    {
        await using var context =
            _fixture.CreateContext();

        await DatabaseCleaner.ResetAsync(context);

        context.Users.AddRange(
            Enumerable.Range(1, 5)
                .Select(i =>
                    UserFactory.Create(u =>
                    {
                        u.Username = $"user{i}";
                        u.Email = $"user{i}@email.com";
                    })));

        await context.SaveChangesAsync();

        var result = await PagedResult<Api.Models.User>
            .CreateAsync(
                context.Users,
                page: 1,
                pageSize: 0);

        result.PageSize.Should().Be(10);
        result.Data.Should().HaveCount(5);
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnEmptyCollection_WhenQueryHasNoItems()
    {
        await using var context =
            _fixture.CreateContext();

        await DatabaseCleaner.ResetAsync(context);

        var result = await PagedResult<Api.Models.User>
            .CreateAsync(
                context.Users,
                page: 1,
                pageSize: 10);

        result.TotalItems.Should().Be(0);
        result.TotalPages.Should().Be(0);
        result.Data.Should().BeEmpty();
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnCorrectItemsForRequestedPage()
    {
        await using var context =
            _fixture.CreateContext();

        await DatabaseCleaner.ResetAsync(context);

        context.Users.AddRange(
            Enumerable.Range(1, 25)
                .Select(i =>
                    UserFactory.Create(u =>
                    {
                        u.Username = $"user{i:D2}";
                        u.Email = $"user{i}@email.com";
                        u.FullName = $"User {i}";
                    })));

        await context.SaveChangesAsync();

        var result = await PagedResult<Api.Models.User>
            .CreateAsync(
                context.Users.OrderBy(u => u.Username),
                page: 3,
                pageSize: 10);

        result.Data.Should().HaveCount(5);
        result.Page.Should().Be(3);
        result.TotalPages.Should().Be(3);
    }

    #endregion
}