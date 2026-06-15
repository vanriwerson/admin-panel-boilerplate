using Api.Data;
using Api.Dtos;
using Api.Interfaces.Auditing.Services;
using Api.Repositories;
using Api.Services.SystemResources;
using Api.Tests.Integration.Infrastructure;
using FluentAssertions;
using Moq;

namespace Api.Tests.Integration.Services.SystemResources;

[Collection("PostgreSql")]
public class CreateSystemResourceTests
{
    private readonly PostgreSqlTestFixture _fixture;

    public CreateSystemResourceTests(
        PostgreSqlTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task ExecuteAsync_Should_Create_Resource()
    {
        await using var context =
            _fixture.CreateContext();

        await DatabaseCleaner.ResetAsync(context);

        var repository =
            new SystemResourceRepository(context);

        var logMock =
            new Mock<ICreateSystemLog>();

        var service =
            new CreateSystemResource(
                repository,
                context,
                logMock.Object);

        var dto =
            new SystemResourceCreateDto
            {
                Name = "USERS",
                ExhibitionName = "Usuários"
            };

        var result =
            await service.ExecuteAsync(dto);

        result.Id.Should().BeGreaterThan(0);

        var resource =
            await repository.GetByIdAsync(result.Id);

        resource.Should().NotBeNull();

        resource!.Name.Should().Be("USERS");
    }

    [Fact]
    public async Task ExecuteAsync_Should_Throw_When_Name_Already_Exists()
    {
        await using var context =
            _fixture.CreateContext();

        await DatabaseCleaner.ResetAsync(context);

        context.SystemResources.Add(
            new Api.Models.SystemResource
            {
                Name = "USERS",
                ExhibitionName = "Usuários"
            });

        await context.SaveChangesAsync();

        var repository =
            new SystemResourceRepository(context);

        var logMock =
            new Mock<ICreateSystemLog>();

        var service =
            new CreateSystemResource(
                repository,
                context,
                logMock.Object);

        var dto =
            new SystemResourceCreateDto
            {
                Name = "USERS",
                ExhibitionName = "Outro"
            };

        var act =
            () => service.ExecuteAsync(dto);

        await act.Should()
            .ThrowAsync<Api.Middlewares.AppException>()
            .WithMessage(
                "Já existe um recurso com esse nome.");
    }

    [Fact]
    public async Task ExecuteAsync_Should_Call_SystemLog()
    {
        await using var context =
            _fixture.CreateContext();

        await DatabaseCleaner.ResetAsync(context);

        var repository =
            new SystemResourceRepository(context);

        var logMock =
            new Mock<ICreateSystemLog>();

        var service =
            new CreateSystemResource(
                repository,
                context,
                logMock.Object);

        await service.ExecuteAsync(
            new SystemResourceCreateDto
            {
                Name = "USERS",
                ExhibitionName = "Usuários"
            });

        logMock.Verify(
            x => x.ExecuteAsync(
                It.IsAny<string>(),
                It.IsAny<int?>(),
                It.IsAny<string?>(),
                It.IsAny<SystemLogDataDto>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Throw_When_Dto_Is_Null()
    {
        await using var context =
            _fixture.CreateContext();

        var repository =
            new SystemResourceRepository(context);

        var logMock =
            new Mock<ICreateSystemLog>();

        var service =
            new CreateSystemResource(
                repository,
                context,
                logMock.Object);

        var act =
            () => service.ExecuteAsync(null!);

        await act.Should()
            .ThrowAsync<Exception>();
    }
}