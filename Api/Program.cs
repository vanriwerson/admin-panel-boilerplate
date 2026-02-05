using Api.Data;
using Api.Extensions.DependencyInjection;
using Api.Helpers;
using Api.Middlewares;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Resend;

Env.Load();

var logger = Logger.LogToConsole("Startup");

var builder = WebApplication.CreateBuilder(args);

// --- Kestrel ---
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(int.Parse(EnvLoader.GetEnv("API_PORT")));
});

// --- DbContext ---
builder.Services.AddDbContext<ApiDbContext>(options =>
    options.UseNpgsql(
        $"Host={EnvLoader.GetEnv("DB_HOST")};" +
        $"Port={EnvLoader.GetEnv("DB_PORT")};" +
        $"Username={EnvLoader.GetEnv("DB_USER")};" +
        $"Password={EnvLoader.GetEnv("DB_PASSWORD")};" +
        $"Database={EnvLoader.GetEnv("DB_NAME")}"
    )
);

// --- Resend ---
builder.Services.AddHttpClient<ResendClient>();
builder.Services.Configure<ResendClientOptions>(options =>
{
    options.ApiToken = EnvLoader.GetEnv("RESEND_API_KEY");
});
builder.Services.AddTransient<ResendClient>();

// --- DependencyInjection ---
builder.Services
    .AddRepositories(logger)
    .AddValidators(logger)
    .AddInfrastructure()
    .AddApplicationServices(logger);

// --- Controllers / CORS / Swagger ---
builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
        policy.WithOrigins(EnvLoader.GetEnv("WEB_APP_URL"))
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials()
    );
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// --- Pipeline ---
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandlerMiddleware();
app.UseCors("FrontendPolicy");
app.UseRequireAuthorization();
app.UseValidateUserPermissions();
app.MapControllers();

app.Run();
