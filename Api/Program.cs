using Api.Data;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;

Env.Load();

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

// --- Serviços ---
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// --- Testar conexão com DB ---
try
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ApiDbContext>();
    db.Database.CanConnect();
    Console.WriteLine("Conexão com DB ok");
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
