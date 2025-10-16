using System.Reflection;
using Api.Data;
using Api.Helpers;
using Api.Interfaces;
using Api.Middlewares;
using Api.Repositories;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;

Env.Load();

var logger = Logger.LogToConsole("Startup");

// --- Variáveis da connection string ---
var apiPort = EnvLoader.GetEnv("API_PORT");
var dbHost = EnvLoader.GetEnv("DB_HOST");
var dbPort = EnvLoader.GetEnv("DB_PORT");
var dbUser = EnvLoader.GetEnv("DB_USER");
var dbPassword = EnvLoader.GetEnv("DB_PASSWORD");
var dbName = EnvLoader.GetEnv("DB_NAME");

// --- Configurar Kestrel ---
var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(int.Parse(apiPort));
});

// --- Configurar DbContext ---
var connectionString =
    $"Host={dbHost};Port={dbPort};Username={dbUser};Password={dbPassword};Database={dbName}";
builder.Services.AddDbContext<ApiDbContext>(options => options.UseNpgsql(connectionString));

// --- Registrar repositório genérico ---
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
Console.WriteLine("Repositório genérico registrado.");

// --- Registrar controllers ---
builder.Services.AddControllers();

// --- Configurar CORS ---
var frontendUrl = EnvLoader.GetEnv("WEB_APP_URL");
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        policy.WithOrigins(frontendUrl)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // necessário se usar cookies ou JWT no header
    });
});

// --- Registro automático de Services ---
var assembly = Assembly.GetExecutingAssembly();
int servicesRegistrados = 0;

try
{
    foreach (
        var type in assembly
            .GetTypes()
            .Where(t => t.IsClass && t.Namespace != null && t.Namespace.StartsWith("Api.Services"))
    )
    {
        builder.Services.AddTransient(type);
        servicesRegistrados++;
    }

    logger.LogInformation("{count} services registrados automaticamente.", servicesRegistrados);
}
catch (Exception ex)
{
    logger.LogError(ex, "Erro ao registrar services automaticamente.");
    throw;
}

// --- Swagger ---
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// --- Testar conexão com DB e executar seed ---
try
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ApiDbContext>();

    if (db.Database.CanConnect())
        Console.WriteLine("Conexão com DB ok");
    else
        Console.WriteLine("Falha ao conectar no DB");

    // Executar seeds
    await DbInitializer.SeedAllAsync(db);
}
catch (Exception ex)
{
    Console.WriteLine("Falha na execução do seed: " + ex.Message);
    throw;
}

// --- Middleware de exceção ---
app.UseExceptionHandlerMiddleware();

// --- Habilitar CORS ---
app.UseCors("FrontendPolicy");

// --- Pipeline HTTP ---
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

app.UseRequireAuthorization();

app.MapControllers();
app.Run();
