using Api.Auditing;
using Api.Dtos;
using Api.Models;
using Api.Repositories;
using Api.Tests.Integration.Infrastructure;
using FluentAssertions;

namespace Api.Tests.Integration.Repositories;

[Collection("PostgreSql")]
public class SystemLogRepositoryTests
{
    private readonly PostgreSqlTestFixture _fixture;

    public SystemLogRepositoryTests(
        PostgreSqlTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task CreateAsync_Should_Persist_Log()
    {
        await using var context =
            _fixture.CreateContext();

        await DatabaseCleaner.ResetAsync(context);

        var repository =
            new SystemLogRepository(context);

        var log =
            new SystemLog
            {
                Action = "create user",
                GeneratedBy = "admin",
                CreatedAt = DateTime.UtcNow
            };

        await repository.CreateAsync(log);

        log.Id.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Log()
    {
        await using var context =
            _fixture.CreateContext();

        await DatabaseCleaner.ResetAsync(context);

        var log =
            new SystemLog
            {
                Action = "update user",
                GeneratedBy = "admin",
                CreatedAt = DateTime.UtcNow
            };

        context.SystemLogs.Add(log);

        await context.SaveChangesAsync();

        var repository =
            new SystemLogRepository(context);

        var result =
            await repository.GetByIdAsync(log.Id);

        result.Should().NotBeNull();

        result!.Action.Should().Be(log.Action);
    }

    [Fact]
    public async Task GetByIdAsync_Should_Deserialize_Data()
    {
        await using var context =
            _fixture.CreateContext();

        await DatabaseCleaner.ResetAsync(context);

        var dto =
            new SystemLogDataDto
            {
                Type = "create"
            };

        var log =
            new SystemLog
            {
                Action = "create user",
                Data = SystemLogDataSerializer.Serialize(dto),
                CreatedAt = DateTime.UtcNow
            };

        context.SystemLogs.Add(log);

        await context.SaveChangesAsync();

        var repository =
            new SystemLogRepository(context);

        var result =
            await repository.GetByIdAsync(log.Id);

        result!.Data.Should().NotBeNull();

        result.Data!.Type.Should().Be("create");
    }

    [Fact]
    public async Task GetPagedAsync_Should_Return_Paged_Logs()
    {
        await using var context =
            _fixture.CreateContext();

        await DatabaseCleaner.ResetAsync(context);

        context.SystemLogs.AddRange(
            new SystemLog
            {
                Action = "action1",
                CreatedAt = DateTime.UtcNow
            },
            new SystemLog
            {
                Action = "action2",
                CreatedAt = DateTime.UtcNow
            });

        await context.SaveChangesAsync();

        var repository =
            new SystemLogRepository(context);

        var result =
            await repository.GetPagedAsync(
                null,
                null,
                null,
                null,
                1,
                10);

        result.Data.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetPagedAsync_Should_Filter_By_Action()
    {
        await using var context =
            _fixture.CreateContext();

        await DatabaseCleaner.ResetAsync(context);

        context.SystemLogs.AddRange(
            new SystemLog
            {
                Action = "create user",
                CreatedAt = DateTime.UtcNow
            },
            new SystemLog
            {
                Action = "delete user",
                CreatedAt = DateTime.UtcNow
            });

        await context.SaveChangesAsync();

        var repository =
            new SystemLogRepository(context);

        var result =
            await repository.GetPagedAsync(
                null,
                "create",
                null,
                null,
                1,
                10);

        result.Data.Should().ContainSingle();

        result.Data.First().Action
            .Should()
            .Contain("create");
    }

    [Fact]
    public async Task Query_Should_Return_AsQueryable()
    {
        await using var context =
            _fixture.CreateContext();

        await DatabaseCleaner.ResetAsync(context);

        context.SystemLogs.Add(
            new SystemLog
            {
                Action = "query test",
                CreatedAt = DateTime.UtcNow
            });

        await context.SaveChangesAsync();

        var repository =
            new SystemLogRepository(context);

        var result =
            repository.Query();

        result.Should().HaveCount(1);
    }

}
