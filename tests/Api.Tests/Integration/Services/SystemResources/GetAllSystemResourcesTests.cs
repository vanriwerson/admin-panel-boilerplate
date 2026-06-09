using Api.Models;
using Api.Repositories;
using Api.Services.SystemResources;
using Api.Tests.Integration.Infrastructure;
using FluentAssertions;

namespace Api.Tests.Integration.Services.SystemResources;

[Collection("PostgreSql")]
public class GetAllSystemResourcesTests
{
    private readonly PostgreSqlTestFixture _fixture;

    public GetAllSystemResourcesTests(
        PostgreSqlTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task ExecuteAsync_Should_Return_Paged_Result()
    {
        await using var context =
            _fixture.CreateContext();

        await DatabaseCleaner.ResetAsync(context);

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

        await context.SaveChangesAsync();

        var service =
            new GetAllSystemResources(
                new SystemResourceRepository(context));

        var result =
            await service.ExecuteAsync();

        result.TotalItems.Should().Be(2);

        result.Data.Should()
            .HaveCount(2);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Respect_Pagination()
    {
        await using var context =
            _fixture.CreateContext();

        await DatabaseCleaner.ResetAsync(context);

        for (var i = 1; i <= 15; i++)
        {
            context.SystemResources.Add(
                new SystemResource
                {
                    Name = $"RESOURCE_{i}",
                    ExhibitionName = $"Recurso {i}"
                });
        }

        await context.SaveChangesAsync();

        var service =
            new GetAllSystemResources(
                new SystemResourceRepository(context));

        var result =
            await service.ExecuteAsync(
                page: 2,
                pageSize: 10);

        result.Page.Should().Be(2);

        result.Data.Should()
            .HaveCount(5);
    }
}