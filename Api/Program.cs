using Api.Data;
using Api.Interfaces;
using Api.Repositories;
using Api.Services.UsersServices;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

Env.Load();

var logger = LoggerFactory.Create(logging => logging.AddConsole()).CreateLogger("Startup");

// --- Variáveis obrigatórias ---
string GetEnvOrThrow(string key)
{
    var value = Environment.GetEnvironmentVariable(key);
    if (string.IsNullOrWhiteSpace(value))
        throw new InvalidOperationException($"Variável '{key}' não configurada.");
    return value;
}

var apiPort = GetEnvOrThrow("API_PORT");
var dbHost = GetEnvOrThrow("DB_HOST");
var dbPort = GetEnvOrThrow("DB_PORT");
var dbUser = GetEnvOrThrow("DB_USER");
var dbPassword = GetEnvOrThrow("DB_PASSWORD");
var dbName = GetEnvOrThrow("DB_NAME");

// --- Configurar Kestrel ---
var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(int.Parse(apiPort));
});

// --- Configurar DbContext ---
var connectionString = $"Host={dbHost};Port={dbPort};Username={dbUser};Password={dbPassword};Database={dbName}";
builder.Services.AddDbContext<ApiDbContext>(options =>
    options.UseNpgsql(connectionString));

// --- Registrar repositório genérico ---
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
Console.WriteLine("Repositório genérico registrado.");

// --- Registro automático de Services ---
var assembly = Assembly.GetExecutingAssembly();
int servicesRegistrados = 0;

try
{
    foreach (var type in assembly.GetTypes()
                 .Where(t => t.IsClass && t.Namespace != null && t.Namespace.StartsWith("Api.Services")))
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

// --- Serviços do Swagger ---
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// --- Testar conexão com DB ---
try
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ApiDbContext>();
    if (db.Database.CanConnect())
    {
        Console.WriteLine("Conexão com DB ok");
    }
    else
    {
        Console.WriteLine("Falha ao conectar no DB");
    }
}
catch (Exception ex)
{
    Console.WriteLine("Falha na conexão com DB: " + ex.Message);
    throw;
}

// --- Pipeline HTTP ---
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.Run();
