using Api.Auditing.Services;
using Api.Middlewares;
using Api.Models;
using Api.Repositories;
using Api.Tests.Integration.Infrastructure;
using FluentAssertions;

namespace Api.Tests.Integration.Auditing.Services;

[Collection("PostgreSql")]
public class GetAllSystemLogsTests
{
    private readonly PostgreSqlTestFixture _fixture;

    public GetAllSystemLogsTests(
        PostgreSqlTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task ExecuteAsync_Should_Return_All_Logs()
    {
        await using var context =
            _fixture.CreateContext();

        await DatabaseCleaner.ResetAsync(context);

        context.SystemLogs.AddRange(
            new SystemLog
            {
                Action = "users.create",
                GeneratedBy = "admin",
                Data = "{}"
            },
            new SystemLog
            {
                Action = "users.update",
                GeneratedBy = "admin",
                Data = "{}"
            });

        await context.SaveChangesAsync();

        var service =
            new GetAllSystemLogs(
                new SystemLogRepository(context));

        var result =
            await service.ExecuteAsync();

        result.TotalItems.Should().Be(2);
        result.Data.Should().HaveCount(2);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Filter_By_Action()
    {
        await using var context =
            _fixture.CreateContext();

        await DatabaseCleaner.ResetAsync(context);

        context.SystemLogs.AddRange(
            new SystemLog
            {
                Action = "users.create",
                GeneratedBy = "admin",
                Data = "{}"
            },
            new SystemLog
            {
                Action = "reports.generate",
                GeneratedBy = "admin",
                Data = "{}"
            });

        await context.SaveChangesAsync();

        var service =
            new GetAllSystemLogs(
                new SystemLogRepository(context));

        var result =
            await service.ExecuteAsync(
                action: "reports");

        result.TotalItems.Should().Be(1);

        result.Data.Single()
            .Action.Should()
            .Be("reports.generate");
    }

    [Fact]
    public async Task ExecuteAsync_Should_Filter_By_UserId()
    {
        await using var context =
            _fixture.CreateContext();

        await DatabaseCleaner.ResetAsync(context);

        var user1 = new User
        {
            Username = "user1",
            FullName = "User One",
            Email = "user1@email.com",
            Password = "123"
        };

        var user2 = new User
        {
            Username = "user2",
            FullName = "User Two",
            Email = "user2@email.com",
            Password = "123"
        };

        context.Users.AddRange(user1, user2);

        await context.SaveChangesAsync();

        context.SystemLogs.AddRange(
            new SystemLog
            {
                UserId = user1.Id,
                GeneratedBy = user1.Username,
                Action = "users.create",
                Data = "{}"
            },
            new SystemLog
            {
                UserId = user2.Id,
                GeneratedBy = user2.Username,
                Action = "users.update",
                Data = "{}"
            });

        await context.SaveChangesAsync();

        var service =
            new GetAllSystemLogs(
                new SystemLogRepository(context));

        var result =
            await service.ExecuteAsync(
                userId: user2.Id);

        result.TotalItems.Should().Be(1);

        result.Data.Single()
            .GeneratedBy.Should()
            .Be("user2");
    }

    [Fact]
    public async Task ExecuteAsync_Should_Filter_By_Date_Range()
    {
        await using var context =
            _fixture.CreateContext();

        await DatabaseCleaner.ResetAsync(context);

        context.SystemLogs.AddRange(
            new SystemLog
            {
                Action = "january-log",
                GeneratedBy = "admin",
                Data = "{}",
                CreatedAt = new DateTime(
                    2025, 1, 15, 12, 0, 0,
                    DateTimeKind.Utc)
            },
            new SystemLog
            {
                Action = "february-log",
                GeneratedBy = "admin",
                Data = "{}",
                CreatedAt = new DateTime(
                    2025, 2, 15, 12, 0, 0,
                    DateTimeKind.Utc)
            });

        await context.SaveChangesAsync();

        var service =
            new GetAllSystemLogs(
                new SystemLogRepository(context));

        var result =
            await service.ExecuteAsync(
                startDate: "2025-02-01",
                endDate: "2025-02-28");

        result.TotalItems.Should().Be(1);

        result.Data.Single()
            .Action.Should()
            .Be("february-log");
    }

    [Fact]
    public async Task ExecuteAsync_Should_Return_Paginated_Result()
    {
        await using var context =
            _fixture.CreateContext();

        await DatabaseCleaner.ResetAsync(context);

        for (int i = 1; i <= 15; i++)
        {
            context.SystemLogs.Add(
                new SystemLog
                {
                    Action = $"log-{i}",
                    GeneratedBy = "admin",
                    Data = "{}"
                });
        }

        await context.SaveChangesAsync();

        var service =
            new GetAllSystemLogs(
                new SystemLogRepository(context));

        var result =
            await service.ExecuteAsync(
                page: 2,
                pageSize: 5);

        result.TotalItems.Should().Be(15);
        result.Page.Should().Be(2);
        result.PageSize.Should().Be(5);
        result.Data.Should().HaveCount(5);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Throw_When_StartDate_Is_Future()
    {
        await using var context =
            _fixture.CreateContext();

        await DatabaseCleaner.ResetAsync(context);

        var service =
            new GetAllSystemLogs(
                new SystemLogRepository(context));

        var futureDate =
            DateTime.UtcNow
                .AddDays(1)
                .ToString("yyyy-MM-dd");

        var act =
            () => service.ExecuteAsync(
                startDate: futureDate);

        await act.Should()
            .ThrowAsync<AppException>()
            .WithMessage(
                "Data inicial não pode ser uma data futura.");
    }

    [Fact]
    public async Task ExecuteAsync_Should_Throw_When_EndDate_Is_Before_StartDate()
    {
        await using var context =
            _fixture.CreateContext();

        await DatabaseCleaner.ResetAsync(context);

        var service =
            new GetAllSystemLogs(
                new SystemLogRepository(context));

        var act =
            () => service.ExecuteAsync(
                startDate: "2025-02-10",
                endDate: "2025-02-01");

        await act.Should()
            .ThrowAsync<AppException>()
            .WithMessage(
                "Data final não pode ser anterior à data inicial.");
    }

    [Fact]
    public async Task ExecuteAsync_Should_Return_Empty_Result_When_No_Logs_Match()
    {
        await using var context =
            _fixture.CreateContext();

        await DatabaseCleaner.ResetAsync(context);

        context.SystemLogs.Add(
            new SystemLog
            {
                Action = "users.create",
                GeneratedBy = "admin",
                Data = "{}"
            });

        await context.SaveChangesAsync();

        var service =
            new GetAllSystemLogs(
                new SystemLogRepository(context));

        var result =
            await service.ExecuteAsync(
                action: "non-existent-action");

        result.TotalItems.Should().Be(0);
        result.Data.Should().BeEmpty();
    }
}