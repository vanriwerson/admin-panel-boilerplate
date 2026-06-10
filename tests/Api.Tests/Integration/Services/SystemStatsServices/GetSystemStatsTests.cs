using Api.Models;
using Api.Repositories;
using Api.Services.SystemStatsServices;
using Api.Tests.Integration.Infrastructure;
using FluentAssertions;

namespace Api.Tests.Integration.Services.SystemStatsServices;

[Collection("PostgreSql")]
public class GetSystemStatsTests
{
    private readonly PostgreSqlTestFixture _fixture;

    public GetSystemStatsTests(
        PostgreSqlTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task ExecuteAsync_Should_Return_Zero_When_Database_Is_Empty()
    {
        await using var context =
            _fixture.CreateContext();

        await DatabaseCleaner.ResetAsync(context);

        var service = new GetSystemStats(
            new UserRepository(context),
            new SystemResourceRepository(context),
            new SystemLogRepository(context));

        var result = await service.ExecuteAsync();

        result.UsersCount.Should().Be(0);
        result.SystemResourcesCount.Should().Be(0);
        result.MonthlyReportsCount.Should().Be(0);

        result.MonthlyReportsCountReference
            .Should()
            .EndWith($"/{DateTime.UtcNow.Year}");
    }

    [Fact]
    public async Task ExecuteAsync_Should_Return_Correct_Counts()
    {
        await using var context =
            _fixture.CreateContext();

        await DatabaseCleaner.ResetAsync(context);

        var now = DateTime.UtcNow;

        context.Users.AddRange(
            new User
            {
                Username = "user1",
                Email = "user1@email.com",
                FullName = "User 1",
                Password = "hash"
            },
            new User
            {
                Username = "user2",
                Email = "user2@email.com",
                FullName = "User 2",
                Password = "hash"
            });

        context.SystemResources.AddRange(
            new SystemResource
            {
                Name = "USERS",
                ExhibitionName = "Usuários"
            },
            new SystemResource
            {
                Name = "REPORTS",
                ExhibitionName = "Relatórios"
            });

        context.SystemLogs.AddRange(
            new SystemLog
            {
                Action = "log 1",
                CreatedAt = now.AddDays(-1)
            },
            new SystemLog
            {
                Action = "log 2",
                CreatedAt = now.AddDays(-2)
            });

        await context.SaveChangesAsync();

        var service = new GetSystemStats(
            new UserRepository(context),
            new SystemResourceRepository(context),
            new SystemLogRepository(context));

        var result = await service.ExecuteAsync();

        result.UsersCount.Should().Be(2);
        result.SystemResourcesCount.Should().Be(2);
        result.MonthlyReportsCount.Should().Be(2);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Count_Only_Current_Month_Logs()
    {
        await using var context =
            _fixture.CreateContext();

        await DatabaseCleaner.ResetAsync(context);

        var now = DateTime.UtcNow;

        var startOfMonth = new DateTime(
            now.Year,
            now.Month,
            1,
            0, 0, 0,
            DateTimeKind.Utc);

        context.SystemLogs.AddRange(
            new SystemLog
            {
                Action = "current-month-1",
                CreatedAt = startOfMonth.AddDays(1)
            },
            new SystemLog
            {
                Action = "current-month-2",
                CreatedAt = startOfMonth.AddDays(2)
            },
            new SystemLog
            {
                Action = "previous-month",
                CreatedAt = startOfMonth.AddDays(-1)
            });

        await context.SaveChangesAsync();

        var service = new GetSystemStats(
            new UserRepository(context),
            new SystemResourceRepository(context),
            new SystemLogRepository(context));

        var result = await service.ExecuteAsync();

        result.MonthlyReportsCount.Should().Be(2);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Generate_Month_Reference()
    {
        await using var context =
            _fixture.CreateContext();

        await DatabaseCleaner.ResetAsync(context);

        var service = new GetSystemStats(
            new UserRepository(context),
            new SystemResourceRepository(context),
            new SystemLogRepository(context));

        var result = await service.ExecuteAsync();

        result.MonthlyReportsCountReference
            .Should()
            .MatchRegex(@"^[A-ZÀ-Ú][a-zà-ú]+\/\d{4}$");
    }
}