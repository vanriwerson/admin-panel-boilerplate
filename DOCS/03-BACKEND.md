# Backend - API Documentation

Documentação completa do backend .NET 8 da aplicação.

## Índice

1. [Visão Geral](#visão-geral)
2. [Estrutura de Pastas](#estrutura-de-pastas)
3. [Controllers](#controllers)
4. [Services](#services)
5. [Models](#models)
6. [Data Transfer Objects (DTOs)](#data-transfer-objects-dtos)
7. [Middlewares](#middlewares)
8. [Helpers](#helpers)
9. [Repository Pattern](#repository-pattern)
10. [Configuração e Bootstrap](#configuração-e-bootstrap)
11. [Banco de Dados](#banco-de-dados)

## Visão Geral

O backend é uma API REST construída com ASP.NET Core 8, seguindo princípios de Clean Architecture e separação de responsabilidades.

**Características:**

- RESTful API
- Autenticação JWT
- RBAC (Role-Based Access Control)
- Entity Framework Core
- Repository Pattern
- Service Layer Pattern
- DTO Pattern
- Middleware Pipeline
- Swagger/OpenAPI

**Porta padrão:** DEFINA A PORTA PADRÃO -- PARA APLICAÇÕES NO DTI PMA, USE A SEQUÊNCIA 521X (Veja quais estão sendo utilizadas)

**Base URL:** `http://localhost:{PORT}/api`

## Estrutura de Pastas

```
Api/
├── Controllers/              # Endpoints da API
├── Services/                 # Lógica de negócio
├── Models/                   # Entidades do banco
├── Dtos/                     # Data Transfer Objects
├── Data/                     # DbContext e configurações
├── Repositories/             # Acesso a dados
├── Middlewares/              # Pipeline HTTP
├── Helpers/                  # Utilitários
├── Validations/              # Validações customizadas
├── Migrations/               # Migrations do EF Core
├── Properties/               # Configurações do projeto
├── Program.cs                # Entry point
├── Api.csproj                # Arquivo do projeto
├── appsettings.json          # Configurações
├── .env.example              # Template de variáveis
├── Dockerfile                # Container Docker
└── README.md                 # Documentação
```

## Controllers

Controllers são responsáveis apenas por receber requisições HTTP, validar entrada básica e delegar para services.

### AuthController

**Arquivo:** `Api/Controllers/AuthController.cs:1`

**Rota base:** `/api/auth`

**Endpoints:**

| Método | Rota                    | Descrição                | Auth |
| ------ | ----------------------- | ------------------------ | ---- |
| POST   | `/login`                | Login local              | Não  |
| POST   | `/external`             | Login via token externo  | Não  |
| POST   | `/password/request-new` | Solicitar reset de senha | Não  |
| POST   | `/password/reset`       | Redefinir senha          | Não  |

**Exemplo de implementação:**

```csharp
[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly LoginService _loginService;

    [HttpPost("login")]
    public async Task<ActionResult> Login([FromBody] LoginDto dto)
    {
        var result = await _loginService.ExecuteAsync(dto);
        return Ok(result);
    }
}
```

### UsersController

**Arquivo:** `Api/Controllers/UsersController.cs:1`

**Rota base:** `/api/users`

**Permissão requerida:** ID 2 (users)

**Endpoints:**

| Método | Rota                  | Descrição        | Paginação |
| ------ | --------------------- | ---------------- | --------- |
| GET    | `/`                   | Lista usuários   | Sim       |
| GET    | `/search?key={busca}` | Busca usuários   | Sim       |
| GET    | `/{id}`               | Busca por ID     | Não       |
| GET    | `/options`            | Lista resumida   | Não       |
| POST   | `/`                   | Cria usuário     | Não       |
| PUT    | `/{id}`               | Atualiza usuário | Não       |
| DELETE | `/{id}`               | Desativa usuário | Não       |

**Query Parameters (paginação):**

- `page`: Número da página (padrão: 1)
- `limit`: Itens por página (padrão: 10)

### SystemResourcesController

**Arquivo:** `Api/Controllers/SystemResourcesController.cs:1`

**Rota base:** `/api/resources`

**Permissão requerida:** ID 3 (resources)

**Endpoints:**

| Método | Rota                  | Descrição        | Paginação |
| ------ | --------------------- | ---------------- | --------- |
| GET    | `/`                   | Lista recursos   | Sim       |
| GET    | `/search?key={busca}` | Busca recursos   | Sim       |
| GET    | `/{id}`               | Busca por ID     | Não       |
| GET    | `/options`            | Lista resumida   | Não       |
| POST   | `/`                   | Cria recurso     | Não       |
| PUT    | `/{id}`               | Atualiza recurso | Não       |
| DELETE | `/{id}`               | Desativa recurso | Não       |

### SystemLogsController

**Arquivo:** `Api/Controllers/SystemLogsController.cs:1`

**Rota base:** `/api/reports`

**Permissão requerida:** ID 4 (reports)

**Endpoints:**

| Método | Rota | Descrição          | Filtros |
| ------ | ---- | ------------------ | ------- |
| GET    | `/`  | Relatórios de logs | Sim     |

**Query Parameters:**

- `page`: Número da página
- `limit`: Itens por página
- `userId`: Filtrar por usuário (opcional)
- `action`: Filtrar por ação (opcional)
- `startDate`: Data inicial (opcional)
- `endDate`: Data final (opcional)

## Services

Services contêm toda a lógica de negócio. Cada operação tem seu próprio service.

### AuthServices

**Pasta:** `Api/Services/AuthServices/`

#### LoginService

**Arquivo:** `Api/Services/AuthServices/LoginService.cs:1`

**Responsabilidades:**

- Validar credenciais (email/username + senha)
- Verificar se usuário está ativo
- Comparar senha com BCrypt
- Gerar JWT com claims
- Registrar log de login
- Retornar token + dados do usuário

**Método principal:**

```csharp
public async Task<LoginResponseDto> ExecuteAsync(LoginDto dto)
```

**Lógica:**

1. Busca usuário por email ou username
2. Valida se usuário existe e está ativo
3. Compara senha com BCrypt
4. Gera JWT com DefaultJWTClaims
5. Registra log de auditoria
6. Retorna LoginResponseDto

#### ExternalTokenService

**Arquivo:** `Api/Services/AuthServices/ExternalTokenService.cs:1`

**Responsabilidades:**

- Autenticar via token externo (SSO corporativo)
- Validar token externo
- Gerar JWT interno
- Registrar log

**Método principal:**

```csharp
public async Task<LoginResponseDto> ExecuteAsync(ExternalLoginDto dto)
```

#### PasswordServices

**Arquivo:** `Api/Services/AuthServices/PasswordServices.cs:1`

**Responsabilidades:**

**1. RequestPasswordReset:**

- Validar email
- Gerar token JWT temporário (expira em 30min)
- Enviar email via EmailService
- Registrar log

**2. ResetPassword:**

- Validar token JWT
- Validar nova senha
- Hashear nova senha com BCrypt
- Atualizar no banco
- Registrar log

#### EmailService

**Arquivo:** `Api/Services/AuthServices/EmailService.cs:1`

**Responsabilidades:**

- Enviar emails via Resend API
- Template de redefinição de senha

**Método principal:**

```csharp
public async Task SendPasswordResetEmail(string email, string token)
```

### UsersServices

**Pasta:** `Api/Services/UsersServices/`

#### CreateUser

**Arquivo:** `Api/Services/UsersServices/CreateUser.cs:1`

**Validações:**

- Email único
- Username único
- Permissões válidas
- Senha forte (mínimo 6 caracteres)

**Processo:**

1. Valida email/username únicos
2. Hasheia senha com BCrypt
3. Cria entidade User
4. Salva no banco via repository
5. Cria permissões (AccessPermissions)
6. Registra log de auditoria
7. Retorna UserResponseDto

#### GetAllUsers

**Arquivo:** `Api/Services/UsersServices/GetAllUsers.cs:1`

**Responsabilidades:**

- Buscar usuários com paginação
- Incluir permissões relacionadas
- Aplicar filtro de ativos
- Retornar UserResponseDto[]

#### GetUserById

**Arquivo:** `Api/Services/UsersServices/GetUserById.cs:1`

**Responsabilidades:**

- Buscar usuário por ID
- Incluir permissões
- Validar se existe e está ativo

#### UpdateUser

**Arquivo:** `Api/Services/UsersServices/UpdateUser.cs:1`

**Validações:**

- Usuário existe
- Email único (se alterado)
- Username único (se alterado)
- Permissões válidas
- Não pode editar usuário root (ID 1) se não for root
- Não pode atribuir permissão root/resources se não for root

**Processo:**

1. Busca usuário existente
2. Valida unicidade de email/username
3. Atualiza campos
4. Atualiza senha se fornecida (hasheia com BCrypt)
5. Remove permissões antigas
6. Cria novas permissões
7. Registra log
8. Retorna UserResponseDto

#### DeleteUser

**Arquivo:** `Api/Services/UsersServices/DeleteUser.cs:1`

**Tipo:** Soft Delete (marca `active = false`)

**Validações:**

- Usuário existe
- Não pode deletar o próprio usuário
- Não pode deletar usuário root (ID 1)

**Processo:**

1. Valida restrições
2. Chama repository.DeleteAsync (soft delete)
3. Registra log

#### SearchUsers

**Arquivo:** `Api/Services/UsersServices/SearchUsers.cs:1`

**Responsabilidades:**

- Buscar usuários por texto (username, email ou fullName)
- Paginação
- Retornar UserResponseDto[]

### SystemResourcesServices

**Pasta:** `Api/Services/SystemResourcesServices/`

Estrutura similar aos UsersServices:

- `CreateSystemResource.cs`
- `GetAllSystemResources.cs`
- `GetSystemResourceById.cs`
- `UpdateSystemResource.cs`
- `DeleteSystemResource.cs` (soft delete)
- `SearchSystemResources.cs`

**Validações específicas:**

- `name` único (identificador interno)
- `name` não pode conter espaços
- Não pode deletar recursos com permissões ativas

### SystemLogsServices

**Pasta:** `Api/Services/SystemLogsServices/`

#### CreateSystemLog

**Arquivo:** `Api/Services/SystemLogsServices/CreateSystemLog.cs:1`

**Responsabilidades:**

- Registrar ação no sistema
- Criar entrada em system_logs

**Método:**

```csharp
public async Task ExecuteAsync(int userId, string action)
```

**Ações padrão:**

- "Login efetuado"
- "Usuário criado: {username}"
- "Usuário atualizado: {username}"
- "Usuário deletado: {username}"
- "Recurso criado: {name}"
- "Recurso atualizado: {name}"
- "Recurso deletado: {name}"

#### GetLogsReport

**Arquivo:** `Api/Services/SystemLogsServices/GetLogsReport.cs:1`

**Responsabilidades:**

- Buscar logs com filtros
- Paginação
- Retornar SystemLogDto[]

**Filtros:**

- userId (opcional)
- action (contém texto, opcional)
- startDate (opcional)
- endDate (opcional)

## Models

Entidades do banco de dados.

### User

**Arquivo:** `Api/Models/User.cs:1`

**Tabela:** `users`

```csharp
public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string Password { get; set; } // Hash BCrypt
    public string FullName { get; set; }
    public bool Active { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navegação
    public ICollection<AccessPermission> AccessPermissions { get; set; }
    public ICollection<SystemLog> SystemLogs { get; set; }
}
```

**Índices:**

- `Email`: UNIQUE
- `Username`: UNIQUE

### SystemResource

**Arquivo:** `Api/Models/SystemResource.cs:1`

**Tabela:** `system_resources`

```csharp
public class SystemResource
{
    public int Id { get; set; }
    public string Name { get; set; } // Identificador único (root, users, resources, reports)
    public string ExhibitionName { get; set; } // Nome para exibição
    public bool Active { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navegação
    public ICollection<AccessPermission> AccessPermissions { get; set; }
}
```

**Índices:**

- `Name`: UNIQUE

### AccessPermission

**Arquivo:** `Api/Models/AccessPermission.cs:1`

**Tabela:** `access_permissions`

```csharp
public class AccessPermission
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int SystemResourceId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navegação
    public User User { get; set; }
    public SystemResource SystemResource { get; set; }
}
```

**Índices:**

- `UserId, SystemResourceId`: UNIQUE (combinação única)

### SystemLog

**Arquivo:** `Api/Models/SystemLog.cs:1`

**Tabela:** `system_logs`

```csharp
public class SystemLog
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Action { get; set; } // Descrição da ação
    public DateTime CreatedAt { get; set; }

    // Navegação
    public User User { get; set; }
}
```

## Data Transfer Objects (DTOs)

DTOs são usados para entrada/saída da API, separando o modelo de domínio da API.

### Auth DTOs

**Pasta:** `Api/Dtos/Auth/`

#### LoginDto (Input)

```csharp
public class LoginDto
{
    [Required]
    public string Identifier { get; set; } // Email ou Username

    [Required]
    public string Password { get; set; }
}
```

#### LoginResponseDto (Output)

```csharp
public class LoginResponseDto
{
    public string Token { get; set; }
    public UserResponseDto User { get; set; }
}
```

#### RequestPasswordResetDto (Input)

```csharp
public class RequestPasswordResetDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
}
```

#### ResetPasswordDto (Input)

```csharp
public class ResetPasswordDto
{
    [Required]
    public string Token { get; set; }

    [Required]
    [MinLength(6)]
    public string NewPassword { get; set; }
}
```

### Users DTOs

**Pasta:** `Api/Dtos/Users/`

#### CreateUserDto (Input)

```csharp
public class CreateUserDto
{
    [Required]
    public string Username { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [MinLength(6)]
    public string Password { get; set; }

    [Required]
    public string FullName { get; set; }

    public List<int> PermissionsIds { get; set; } = new();
}
```

#### UpdateUserDto (Input)

```csharp
public class UpdateUserDto
{
    public string? Username { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; } // Opcional
    public string? FullName { get; set; }
    public List<int>? PermissionsIds { get; set; }
}
```

#### UserResponseDto (Output)

```csharp
public class UserResponseDto
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string FullName { get; set; }
    public bool Active { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<SystemResourceDto> Permissions { get; set; }
}
```

### SystemResources DTOs

**Pasta:** `Api/Dtos/SystemResources/`

#### CreateSystemResourceDto (Input)

```csharp
public class CreateSystemResourceDto
{
    [Required]
    public string Name { get; set; }

    [Required]
    public string ExhibitionName { get; set; }
}
```

#### SystemResourceDto (Output)

```csharp
public class SystemResourceDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string ExhibitionName { get; set; }
    public bool Active { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
```

### SystemLogs DTOs

**Pasta:** `Api/Dtos/SystemLogs/`

#### SystemLogDto (Output)

```csharp
public class SystemLogDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; } // Username do usuário
    public string Action { get; set; }
    public DateTime CreatedAt { get; set; }
}
```

## Middlewares

### RequireAuthorization

**Arquivo:** `Api/Middlewares/RequireAuthorization.cs:1`

**Responsabilidades:**

- Validar presença do header `Authorization`
- Verificar formato `Bearer {token}`
- Validar assinatura JWT
- Verificar expiração do token
- Permitir acesso sem autenticação para rotas `/api/auth/*`

**Implementação:**

```csharp
public class RequireAuthorization
{
    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value;

        // Permite rotas de autenticação sem token
        if (path.StartsWith("/api/auth"))
        {
            await _next(context);
            return;
        }

        // Valida token JWT
        var token = context.Request.Headers["Authorization"]
                           .FirstOrDefault()?.Split(" ").Last();

        if (token == null || !JsonWebToken.Validate(token))
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Unauthorized");
            return;
        }

        await _next(context);
    }
}
```

### ValidateUserPermissions

**Arquivo:** `Api/Middlewares/ValidateUserPermissions.cs:1`

**Responsabilidades:**

- Extrair permissões do JWT
- Mapear endpoint → permissão requerida
- Validar se usuário tem a permissão
- Regra especial: usuário root (ID 1) tem acesso total
- Impedir atribuição de permissões root/resources por não-root

**Mapeamento de Permissões:**

```csharp
var endpointPermissions = new Dictionary<string, int>
{
    { "/api/users", 2 },      // Requer permissão "users"
    { "/api/resources", 3 },  // Requer permissão "resources"
    { "/api/reports", 4 }     // Requer permissão "reports"
};
```

**Validação de Atribuição de Permissões:**

```csharp
// Usuários não-root não podem atribuir permissões root ou resources
if (!isRootUser && (POST/PUT /api/users))
{
    var dto = await ReadBodyAs<CreateUserDto>();
    if (dto.PermissionsIds.Contains(1) || dto.PermissionsIds.Contains(3))
    {
        return 403 Forbidden;
    }
}
```

### ExceptionHandler

**Arquivo:** `Api/Middlewares/ExceptionHandler.cs:1`

**Responsabilidades:**

- Capturar exceções não tratadas
- Retornar respostas padronizadas
- Logar erros no console

**Formato de Resposta:**

```json
{
  "error": "Mensagem de erro",
  "statusCode": 500
}
```

## Helpers

### JsonWebToken

**Arquivo:** `Api/Helpers/JsonWebToken.cs:1`

**Métodos:**

```csharp
// Gera um JWT com claims
public static string Generate(List<Claim> claims)

// Valida assinatura e expiração
public static bool Validate(string token)

// Decodifica e retorna claims
public static JwtSecurityToken? Decode(string token)
```

**Configuração:**

- Algoritmo: HS256
- Chave: `JWT_SECRET_KEY` (variável de ambiente)
- Expiração: 7 dias

### PasswordHashing

**Arquivo:** `Api/Helpers/PasswordHashing.cs:1`

**Métodos:**

```csharp
// Gera hash BCrypt com work factor 12
public static string Hash(string password)

// Verifica senha contra hash
public static bool Verify(string password, string hash)
```

### DefaultJWTClaims

**Arquivo:** `Api/Helpers/DefaultJWTClaims.cs:1`

**Método:**

```csharp
public static List<Claim> Generate(User user)
```

**Claims gerados:**

- `id`: User.Id
- `username`: User.Username
- `email`: User.Email
- `permissions`: IDs de permissões (comma-separated)

### CurrentAuthUser

**Arquivo:** `Api/Helpers/CurrentAuthUser.cs:1`

**Método:**

```csharp
public static int GetId(HttpContext context)
```

Extrai o ID do usuário autenticado do token JWT presente no header Authorization.

### EndpointPermissions

**Arquivo:** `Api/Helpers/EndpointPermissions.cs:1`

**Mapeamento:**

```csharp
public static readonly Dictionary<string, int> Map = new()
{
    { "/api/users", 2 },
    { "/api/resources", 3 },
    { "/api/reports", 4 }
};
```

## Repository Pattern

### IGenericRepository

**Arquivo:** `Api/Repositories/IGenericRepository.cs:1`

**Interface:**

```csharp
public interface IGenericRepository<T> where T : class
{
    Task<T> CreateAsync(T entity);
    Task<List<T>> GetAllAsync(int page, int limit);
    Task<T?> GetByIdAsync(int id);
    Task<List<T>> SearchAsync(string searchKey, int page, int limit);
    Task<T> UpdateAsync(T entity);
    Task DeleteAsync(int id);
}
```

### GenericRepository

**Arquivo:** `Api/Repositories/GenericRepository.cs:1`

**Características:**

- Implementação genérica para todas as entidades
- Suporta soft delete (verifica propriedade `Active`)
- Paginação integrada
- Busca textual (SearchAsync)

**Implementação de Soft Delete:**

```csharp
public async Task DeleteAsync(int id)
{
    var entity = await _dbSet.FindAsync(id);
    if (entity == null) return;

    // Verifica se tem propriedade Active
    var activeProperty = typeof(T).GetProperty("Active");
    if (activeProperty != null && activeProperty.PropertyType == typeof(bool))
    {
        activeProperty.SetValue(entity, false); // Soft delete
        _context.Entry(entity).State = EntityState.Modified;
    }
    else
    {
        _dbSet.Remove(entity); // Hard delete
    }

    await _context.SaveChangesAsync();
}
```

## Configuração e Bootstrap

### Program.cs

**Arquivo:** `Api/Program.cs:1`

**Configurações:**

#### 1. Kestrel (Servidor HTTP)

```csharp
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(int.Parse(Environment.GetEnvironmentVariable("API_PORT")));
});
```

#### 2. Banco de Dados

```csharp
builder.Services.AddDbContext<ApiDbContext>(options =>
    options.UseNpgsql(connectionString));
```

#### 3. CORS

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowWebApp", builder =>
    {
        builder.WithOrigins(Environment.GetEnvironmentVariable("WEB_APP_URL"))
               .AllowAnyHeader()
               .AllowAnyMethod();
    });
});
```

#### 4. Registro Automático de Services

```csharp
// Registra todos os services automaticamente
var serviceTypes = Assembly.GetExecutingAssembly()
    .GetTypes()
    .Where(t => t.Namespace?.StartsWith("Api.Services") == true)
    .Where(t => t.IsClass && !t.IsAbstract);

foreach (var type in serviceTypes)
{
    builder.Services.AddScoped(type);
}
```

#### 5. Repository Genérico

```csharp
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
```

#### 6. Swagger

```csharp
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
```

#### 7. Pipeline de Middlewares

```csharp
app.UseMiddleware<ExceptionHandler>();
app.UseCors("AllowWebApp");
app.UseMiddleware<RequireAuthorization>();
app.UseMiddleware<ValidateUserPermissions>();
app.MapControllers();
```

#### 8. Seed do Banco de Dados

```csharp
if (Environment.GetEnvironmentVariable("RUN_USERS_SEED") == "true")
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ApiDbContext>();
    // Seeds root user + system resources; dummy users are added when
    // RUN_USERS_SEED=true.
    DbInitializer.Initialize(context);
}
```

## Banco de Dados

### ApiDbContext

**Arquivo:** `Api/Data/ApiDbContext.cs:1`

```csharp
public class ApiDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<SystemResource> SystemResources { get; set; }
    public DbSet<AccessPermission> AccessPermissions { get; set; }
    public DbSet<SystemLog> SystemLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Aplica configurações de IEntityTypeConfiguration
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
```

### DbInitializer (Seeds)

**Arquivo:** `Api/Data/DbInitializer.cs:1`

**Dados criados automaticamente:**

#### 1. Usuário Root

```csharp
Username: "root"
Email: "root@admin.com"
Password: BCrypt.Hash("root1234")
FullName: "Root User"
```

#### 2. Recursos do Sistema

```csharp
1. { Name: "root", ExhibitionName: "Administrador" }
2. { Name: "users", ExhibitionName: "Usuários" }
3. { Name: "resources", ExhibitionName: "Recursos" }
4. { Name: "reports", ExhibitionName: "Relatórios" }
```

#### 3. Permissões do Root

- Usuário root recebe todas as 4 permissões

#### 4. Usuários de Teste (executados se `RUN_USERS_SEED=true`)

```csharp
Usernames: alice, bob, carol, dave, eve, frank, grace, heidi, ivan, judy
Email: {username}@test.com
Password: "123456"
Sem permissões por padrão
```

### Migrations

**Pasta:** `Api/Migrations/`

**Comandos úteis:**

```bash
# Criar migration
dotnet ef migrations add NomeDaMigration

# Aplicar migrations
dotnet ef database update

# Reverter migration
dotnet ef database update NomeMigrationAnterior

# Remover última migration (não aplicada)
dotnet ef migrations remove

# Ver SQL de uma migration
dotnet ef migrations script
```

## Dependências

**Arquivo:** `Api/Api.csproj:1`

```xml
<ItemGroup>
  <PackageReference Include="BCrypt.Net-Next" Version="4.0.3" />
  <PackageReference Include="DotNetEnv" Version="3.1.1" />
  <PackageReference Include="Microsoft.AspNetCore.OpenApi" Version="8.0.20" />
  <PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="9.0.9" />
  <PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="9.0.4" />
  <PackageReference Include="Resend" Version="0.1.7" />
  <PackageReference Include="Swashbuckle.AspNetCore" Version="6.6.2" />
  <PackageReference Include="System.IdentityModel.Tokens.Jwt" Version="8.14.0" />
</ItemGroup>
```

## Próximos Passos

- [Frontend Documentation](./04-FRONTEND.md)
- [API Reference](./05-API-REFERENCE.md)
- [Sistema de Permissões](./06-PERMISSOES.md)
