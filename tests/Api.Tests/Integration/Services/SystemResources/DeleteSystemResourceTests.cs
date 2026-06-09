using Api.Dtos;
using Api.Interfaces.Auditing.Services;
using Api.Models;
using Api.Repositories;
using Api.Services.SystemResources;
using Api.Tests.Integration.Infrastructure;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Api.Tests.Integration.Services.SystemResources;

[Collection("PostgreSql")]
public class DeleteSystemResourceTests
{
    private readonly PostgreSqlTestFixture _fixture;

    public DeleteSystemResourceTests(
        PostgreSqlTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task ExecuteAsync_Should_SoftDelete_Resource()
    {
        await using var context =
            _fixture.CreateContext();

        await DatabaseCleaner.ResetAsync(context);

        var resource =
            new SystemResource
            {
                Name = "USERS",
                ExhibitionName = "Usuários"
            };

        context.SystemResources.Add(resource);

        await context.SaveChangesAsync();

        var repository =
            new SystemResourceRepository(context);

        var logMock =
            new Mock<ICreateSystemLog>();

        var service =
            new DeleteSystemResource(
                repository,
                logMock.Object);

        await service.ExecuteAsync(resource.Id);

        var deleted =
            await context.SystemResources
                .IgnoreQueryFilters()
                .FirstAsync(x => x.Id == resource.Id);

        deleted.Active.Should().BeFalse();
    }

    [Fact]
    public async Task ExecuteAsync_Should_Throw_When_NotFound()
    {
        await using var context =
            _fixture.CreateContext();

        await DatabaseCleaner.ResetAsync(context);

        var repository =
            new SystemResourceRepository(context);

        var service =
            new DeleteSystemResource(
                repository,
                Mock.Of<ICreateSystemLog>());

        var act =
            () => service.ExecuteAsync(999);

        await act.Should()
            .ThrowAsync<Api.Middlewares.AppException>()
            .WithMessage("Recurso não encontrado.");
    }

    [Fact]
    public async Task ExecuteAsync_Should_Call_SystemLog()
    {
        await using var context =
            _fixture.CreateContext();

        await DatabaseCleaner.ResetAsync(context);

        var resource =
            new SystemResource
            {
                Name = "USERS",
                ExhibitionName = "Usuários"
            };

        context.SystemResources.Add(resource);

        await context.SaveChangesAsync();

        var logMock =
            new Mock<ICreateSystemLog>();

        var service =
            new DeleteSystemResource(
                new SystemResourceRepository(context),
                logMock.Object);

        await service.ExecuteAsync(resource.Id);

        logMock.Verify(
            x => x.ExecuteAsync(
                It.IsAny<string>(),
                It.IsAny<int?>(),
                It.IsAny<string?>(),
                It.IsAny<SystemLogDataDto>()),
            Times.Once);
    }
}