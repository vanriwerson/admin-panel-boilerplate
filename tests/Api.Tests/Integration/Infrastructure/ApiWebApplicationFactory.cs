using Api.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Api.Tests.Integration.Infrastructure;

public class ApiWebApplicationFactory
    : WebApplicationFactory<Program>
{
    private readonly string _connectionString;

    public ApiWebApplicationFactory(
        string connectionString)
    {
        _connectionString = connectionString;
        ConfigureEnvironmentVariables();
    }

    private static void ConfigureEnvironmentVariables()
    {
        Environment.SetEnvironmentVariable(
            "API_PORT",
            "5000");

        Environment.SetEnvironmentVariable(
            "WEB_APP_URL",
            "http://localhost:5173");

        Environment.SetEnvironmentVariable(
            "JWT_SECRET_KEY",
            "unit-tests-secret-key-with-32-chars");

        Environment.SetEnvironmentVariable(
            "RESEND_API_KEY",
            "test-resend-key");

        Environment.SetEnvironmentVariable(
            "RESEND_FROM_EMAIL",
            "noreply@test.local");

        Environment.SetEnvironmentVariable(
            "DB_HOST",
            "localhost");

        Environment.SetEnvironmentVariable(
            "DB_PORT",
            "5432");

        Environment.SetEnvironmentVariable(
            "DB_USER",
            "test");

        Environment.SetEnvironmentVariable(
            "DB_PASSWORD",
            "test");

        Environment.SetEnvironmentVariable(
            "DB_NAME",
            "test");
    }

    protected override void ConfigureWebHost(
        IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType ==
                     typeof(DbContextOptions<ApiDbContext>)
            );

            if (descriptor != null)
                services.Remove(descriptor);

            services.AddDbContext<ApiDbContext>(options =>
            {
                options.UseNpgsql(_connectionString);
            });
        });
    }
}