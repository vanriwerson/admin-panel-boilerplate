using Api.Data;
using Api.Extensions.DependencyInjection;
using Api.Helpers;
using Api.Middlewares;
using Api.Security.Jwt;
using Api.Settings;
using DotNetEnv;

Env.Load();

var apiSettings = SettingsFactory.ProvideApiSettings();

var databaseConnectionString =
    SettingsFactory.ProvideDatabaseConnectionString();

var frontendSettings =
    SettingsFactory.ProvideFrontendSettings();

var logger = Logger.LogToConsole("Startup");

var builder = WebApplication.CreateBuilder(args);

// --- Kestrel ---
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(int.Parse(apiSettings.Port));
});

builder.Services.ConnectToDatabase(databaseConnectionString);

// --- DependencyInjection ---
builder.Services
    .AddRepositories(logger)
    .AddValidators(logger)
    .AddInfrastructure()
    .AddApplicationServices(logger);

// --- Controllers / CORS / Swagger ---
builder.Services.AddControllers();
builder.Services.AddFrontendCors(frontendSettings);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// --- Seed Data ---
await app.UseDbInitializerAsync();

// --- Pipeline ---
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandlerMiddleware();
app.UseCors(CorsExtensions.FrontendPolicy);
app.UseJwtAuthentication();
app.UseRequireAuthorization();
app.UseValidateUserPermissions();
app.MapControllers();

app.Run();
