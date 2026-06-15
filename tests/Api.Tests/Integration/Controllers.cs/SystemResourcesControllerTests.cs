using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Api.Dtos;
using Api.Models;
using Api.Tests.Integration.Infrastructure;
using FluentAssertions;

namespace Api.Tests.Integration.Controllers;

[Collection("PostgreSql")]
public class SystemResourcesControllerTests
{
    private readonly PostgreSqlTestFixture _fixture;

    public SystemResourcesControllerTests(
        PostgreSqlTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Create_Should_Return_201()
    {
        await using var context =
            _fixture.CreateContext();

        await DatabaseCleaner.ResetAsync(context);

        await using var factory =
            new ApiWebApplicationFactory(
                _fixture.ConnectionString);

        var client =
            factory.CreateClient();

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                TestJwtFactory.CreateRootToken());

        var dto =
            new SystemResourceCreateDto
            {
                Name = "TESTE",
                ExhibitionName = "Recurso de Teste"
            };

        var response =
            await client.PostAsJsonAsync(
                "/api/resources",
                dto);

        if (response.StatusCode != HttpStatusCode.Created)
        {
            var body = await response.Content.ReadAsStringAsync();
            Console.WriteLine("Response body: " + body);
        }

        response.StatusCode
            .Should()
            .Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Update_Should_Return_200()
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

        await using var factory =
            new ApiWebApplicationFactory(
                _fixture.ConnectionString);

        var client =
            factory.CreateClient();

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                TestJwtFactory.CreateRootToken());

        var dto =
            new SystemResourceUpdateDto
            {
                Id = resource.Id,
                Name = "USERS_UPDATED",
                ExhibitionName = "Usuários Atualizado"
            };

        var response =
            await client.PutAsJsonAsync(
                $"/api/resources/{resource.Id}",
                dto);

        response.StatusCode
            .Should()
            .Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Delete_Should_Return_204()
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

        await using var factory =
            new ApiWebApplicationFactory(
                _fixture.ConnectionString);

        var client =
            factory.CreateClient();

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                TestJwtFactory.CreateRootToken());

        var response =
            await client.DeleteAsync(
                $"/api/resources/{resource.Id}");

        response.StatusCode
            .Should()
            .Be(HttpStatusCode.NoContent);
    }
}