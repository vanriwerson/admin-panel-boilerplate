using Api.Auditing.Services;
using Api.Middlewares;
using Api.Repositories;
using Api.Tests.Integration.Infrastructure;
using FluentAssertions;

namespace Api.Tests.Integration.Auditing.Services;

[Collection("PostgreSql")]
public class GetSystemLogByIdTests
{
    private readonly PostgreSqlTestFixture _fixture;

    public GetSystemLogByIdTests(
        PostgreSqlTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task ExecuteAsync_Should_Return_Log()
    {
        await using var context =
            _fixture.CreateContext();

        await DatabaseCleaner.ResetAsync(context);

        var log = new Api.Models.SystemLog
        {
            Action = "users.create",
            GeneratedBy = "admin",
            IpAddress = "127.0.0.1",
            Data = "{}"
        };

        context.SystemLogs.Add(log);

        await context.SaveChangesAsync();

        var service =
            new GetSystemLogById(
                new SystemLogRepository(context));

        var result =
            await service.ExecuteAsync(log.Id);

        result.Id.Should().Be(log.Id);
        result.Action.Should().Be("users.create");
        result.GeneratedBy.Should().Be("admin");
        result.IpAddress.Should().Be("127.0.0.1");
    }

    [Fact]
    public async Task ExecuteAsync_Should_Throw_When_NotFound()
    {
        await using var context =
            _fixture.CreateContext();

        await DatabaseCleaner.ResetAsync(context);

        var service =
            new GetSystemLogById(
                new SystemLogRepository(context));

        var act =
            () => service.ExecuteAsync(999);

        await act.Should()
            .ThrowAsync<AppException>()
            .WithMessage("Log não encontrado.");
    }

    [Fact]
    public async Task ExecuteAsync_Should_Throw_When_Id_Is_Invalid()
    {
        await using var context =
            _fixture.CreateContext();

        await DatabaseCleaner.ResetAsync(context);

        var service =
            new GetSystemLogById(
                new SystemLogRepository(context));

        var act =
            () => service.ExecuteAsync(0);

        await act.Should()
            .ThrowAsync<AppException>();
    }
}