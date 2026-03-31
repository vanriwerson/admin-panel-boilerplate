# Plano de Revisão de Código — Admin Panel Boilerplate

> Stack: React 19 + TypeScript (Frontend) · .NET 8 + PostgreSQL (Backend)
> Data: 2026-03-30

---

## Sumário

1. [Visão Geral do Projeto](#1-visão-geral-do-projeto)
2. [Arquitetura & Estrutura](#2-arquitetura--estrutura)
3. [Backend — .NET 8](#3-backend--net-8)
4. [Frontend — React 19 + TypeScript](#4-frontend--react-19--typescript)
5. [Segurança](#5-segurança)
6. [Banco de Dados & ORM](#6-banco-de-dados--orm)
7. [Testes & Qualidade](#7-testes--qualidade)
8. [Infraestrutura & CI/CD](#8-infraestrutura--cicd)
9. [Documentação](#9-documentação)
10. [Matriz de Riscos](#10-matriz-de-riscos)
11. [Roadmap de Melhorias](#11-roadmap-de-melhorias)

---

## 1. Visão Geral do Projeto

| Item | Detalhe |
|---|---|
| **Propósito** | Boilerplate de painel administrativo com autenticação JWT, RBAC e auditoria de sistema |
| **Backend** | .NET 8 · Entity Framework Core 9 · PostgreSQL 14+ · BCrypt · Swagger |
| **Frontend** | React 19 · Vite · TypeScript · Material-UI 7 · Axios · React Router 7 |
| **DevOps** | Docker Compose (dev/staging/prod) · GitHub Actions · Semantic Release |
| **Padrões** | Repository · Orchestrator · Guard Clauses · Context + Hooks · DTO Pattern |

### Pontos Fortes Identificados

- Separação clara de camadas (Controllers → Services → Repositories)
- Sistema de RBAC funcional com 4 permissões (ROOT, USERS, RESOURCES, REPORTS)
- Auditoria completa de operações (CREATE, UPDATE, DELETE, LOGIN)
- Suporte a dark/light mode, paginação, modais de confirmação
- Pipeline CI/CD com versionamento semântico automatizado
- Documentação extensa em português (`/DOCS/`)

---

## 2. Arquitetura & Estrutura

### Diagrama de Fluxo Atual

```
[React App]
    │
    ├── AuthContext / hooks / services
    │         │
    │    [Axios + Interceptors]
    │         │
    └─────────┼──────────────────────────────────────▶ [.NET 8 API]
                                                            │
                                                   ┌────────┴────────┐
                                                   │   Middleware    │
                                                   │  (JWT + RBAC)   │
                                                   └────────┬────────┘
                                                            │
                                                   ┌────────┴────────┐
                                                   │   Controllers   │
                                                   └────────┬────────┘
                                                            │
                                                   ┌────────┴────────┐
                                                   │    Services     │
                                                   │  (Use Cases)    │
                                                   └────────┬────────┘
                                                            │
                                                   ┌────────┴────────┐
                                                   │  Repositories   │
                                                   └────────┬────────┘
                                                            │
                                                   ┌────────┴────────┐
                                                   │   PostgreSQL    │
                                                   └─────────────────┘
```

### Análise

| Critério | Nota | Observação |
|---|---|---|
| Separação de responsabilidades | ✅ Boa | Controllers não contêm lógica de negócio |
| Coesão dos módulos | ✅ Boa | Cada serviço tem escopo definido |
| Acoplamento entre camadas | ⚠️ Médio | Services acoplados diretamente a Repositories sem Unit of Work |
| Escalabilidade horizontal | ⚠️ Médio | JWT stateless facilita, mas sem caching/rate-limit |
| Consistência de nomenclatura | ✅ Boa | Convenções seguidas no backend e frontend |

### Melhorias Estruturais

**[ARCH-01] Implementar Unit of Work**

Atualmente cada service injeta múltiplos repositories e salva separadamente. Um `IUnitOfWork` centralizaria as transações:

```csharp
// Atual: SaveChanges distribuído
await _userRepository.CreateAsync(user);
await _accessPermissionRepository.CreateAsync(permission);

// Proposto: Transação atômica
await _unitOfWork.BeginTransactionAsync();
await _userRepository.CreateAsync(user);
await _permissionRepository.CreateAsync(permission);
await _unitOfWork.CommitAsync();
```

**[ARCH-02] Considerar Mediator (MediatR) para desacoplamento de use-cases**

Os Orchestrators (`CreateUserWithAccessGranted`) já sugerem use-cases isolados. MediatR formalizaria esse padrão e facilitaria a injeção de comportamentos cross-cutting (logging, validação) via pipeline behaviors.

---

## 3. Backend — .NET 8

### 3.1 Controllers

**Arquivo referência:** [Api/Controllers/UsersController.cs](Api/Controllers/UsersController.cs)

| Critério | Status | Detalhe |
|---|---|---|
| RESTful semântico | ✅ | Verbos e URIs corretos |
| Retorno de status HTTP | ⚠️ | Alguns endpoints retornam `200` onde `201 Created` seria mais adequado |
| Acoplamento com Services | ✅ | Controllers delegam para services |
| Tratamento de erros | ✅ | ExceptionHandler middleware captura AppException |

**[API-01] Retornar `201 Created` com Location Header em POST**

```csharp
// Atual
return Ok(result);

// Proposto
return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
```

**[API-02] Padronizar contratos de erro**

Adicionar `ProblemDetails` (RFC 7807) para respostas de erro:

```csharp
// Proposto via middleware
return Problem(
    title: "Recurso não encontrado",
    detail: ex.Message,
    statusCode: 404,
    instance: HttpContext.Request.Path
);
```

---

### 3.2 Services & Use Cases

**Arquivo referência:** [Api/Services/Users/CreateUser.cs](Api/Services/Users/CreateUser.cs)

| Critério | Status | Detalhe |
|---|---|---|
| Responsabilidade única | ✅ | Cada service faz uma operação |
| Validação de entrada | ⚠️ | Depende de Guard.cs manual, sem FluentValidation |
| Mensagens de erro | ⚠️ | Mensagens genéricas em alguns casos |
| Retorno tipado | ✅ | DTOs bem definidos |

**[SVC-01] Adotar FluentValidation**

A classe `Guard.cs` cobre casos básicos, mas validações complexas (ex: formato de email, regras de negócio compostas) se beneficiariam de FluentValidation:

```csharp
public class CreateUserValidator : AbstractValidator<CreateUserDto>
{
    public CreateUserValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).MinimumLength(8).Matches("[A-Z]");
        RuleFor(x => x.Username).NotEmpty().MaximumLength(50);
    }
}
```

**[SVC-02] Respostas de serviço com Result Pattern**

Evitar lançar exceções para fluxos esperados de negócio:

```csharp
// Atual: lança exceção
throw new AppException("Usuário não encontrado", 404);

// Proposto: Result<T>
return Result<UserDto>.Failure("Usuário não encontrado", ErrorCode.NotFound);
```

---

### 3.3 Autenticação & Tokens

**Arquivo referência:** [Api/Security/Jwt/JwtServices.cs](Api/Security/Jwt/JwtServices.cs)

| Critério | Status | Detalhe |
|---|---|---|
| Algoritmo de assinatura | ✅ | HS256 |
| Expiração do token | ⚠️ | 480 minutos (8h) — alto para produção |
| Refresh token revogação | ✅ | Suporte a revogação implementado |
| Secret Key mínima | ✅ | Documentado como min. 32 chars |
| Rotation de refresh token | ⚠️ | Não implementado |

**[SEC-JWT-01] Reduzir expiração do access token**

8 horas é um intervalo longo para um access token. Recomenda-se:
- Access token: 15–30 minutos
- Refresh token: 7 dias (já suportado)

**[SEC-JWT-02] Implementar Refresh Token Rotation**

A cada uso do refresh token, revogá-lo e emitir um novo par (access + refresh). Detecta reuso de token roubado.

---

### 3.4 Sistema de Auditoria

**Arquivo referência:** [Api/Auditing/Services/](Api/Auditing/Services/)

| Critério | Status | Detalhe |
|---|---|---|
| Cobertura de operações | ✅ | CREATE, UPDATE, DELETE, LOGIN |
| Serialização de payload | ✅ | JSON serializado em SystemLog.Data |
| Consulta com filtros | ✅ | userId, action, startDate, endDate |
| Crescimento ilimitado | ⚠️ | Tabela `system_logs` sem política de retenção |
| Index na tabela | ⚠️ | Verificar se há índice em `created_at` e `user_id` |

**[AUD-01] Política de Retenção de Logs**

```csharp
// Job agendado (ex: Hangfire ou BackgroundService)
public async Task PurgeOldLogsAsync()
{
    var cutoff = DateTime.UtcNow.AddDays(-_retentionDays);
    await _dbContext.SystemLogs
        .Where(l => l.CreatedAt < cutoff)
        .ExecuteDeleteAsync();
}
```

**[AUD-02] Adicionar índices nas colunas mais filtradas**

```csharp
// Em SystemLogConfiguration.cs
entity.HasIndex(l => l.CreatedAt);
entity.HasIndex(l => l.UserId);
entity.HasIndex(l => l.Action);
```

---

### 3.5 Tratamento de Erros

**Arquivo referência:** [Api/Middlewares/ExceptionHandler.cs](Api/Middlewares/ExceptionHandler.cs)

| Critério | Status | Detalhe |
|---|---|---|
| Captura global | ✅ | Middleware captura AppException e genérica |
| Mensagem de erro em produção | ✅ | Genérica para erros 500 |
| Logging de erros | ⚠️ | Logger.cs customizado, sem Serilog estruturado |
| Correlation ID | ❌ | Não implementado |

**[ERR-01] Logging Estruturado com Serilog**

```csharp
// Program.cs
builder.Host.UseSerilog((ctx, cfg) => cfg
    .WriteTo.Console(new JsonFormatter())
    .WriteTo.File("logs/api-.log", rollingInterval: RollingInterval.Day)
    .Enrich.WithCorrelationId()
    .Enrich.FromLogContext());
```

**[ERR-02] Adicionar X-Correlation-Id em toda request/response**

Facilita rastrear uma requisição do frontend ao banco de dados em sistemas distribuídos.

---

## 4. Frontend — React 19 + TypeScript

### 4.1 Gerenciamento de Estado

**Arquivo referência:** [WebApp/src/contexts/AuthContext.tsx](WebApp/src/contexts/AuthContext.tsx)

| Critério | Status | Detalhe |
|---|---|---|
| Context API para estado global | ✅ | AuthContext, ThemeContext, NotificationContext |
| Re-renders desnecessários | ⚠️ | Context sem memoização pode causar renders em cascata |
| Estado derivado | ⚠️ | `authUser` lido do localStorage na inicialização sem validação |
| Separação de concerns | ✅ | Cada contexto tem responsabilidade única |

**[FE-STATE-01] Memoizar valores de contexto**

```tsx
// Atual: novo objeto a cada render
<AuthContext.Provider value={{ token, authUser, handleLogin }}>

// Proposto: estável entre renders
const value = useMemo(
  () => ({ token, authUser, handleLogin, handleLogout }),
  [token, authUser]
);
<AuthContext.Provider value={value}>
```

**[FE-STATE-02] Considerar Zustand para estados complexos**

Para aplicações que crescem além do boilerplate, Zustand oferece stores independentes sem o problema de re-renders em cascata do Context, com API simples e sem boilerplate.

---

### 4.2 Interceptors Axios & Refresh Token

**Arquivo referência:** [WebApp/src/api/index.ts](WebApp/src/api/index.ts)

| Critério | Status | Detalhe |
|---|---|---|
| Injeção automática de token | ✅ | Interceptor de request |
| Fila para requests concorrentes | ✅ | Implementado com subscribers |
| Tratamento de falha no refresh | ✅ | Dispara logout |
| Timeout configurado | ❌ | Não definido — requisições podem travar |

**[FE-API-01] Definir timeout global no Axios**

```typescript
const api = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL,
  timeout: 15000, // 15 segundos
});
```

**[FE-API-02] Centralizar tipos de erro da API**

```typescript
// src/api/errors.ts
export class ApiError extends Error {
  constructor(
    public statusCode: number,
    public message: string,
    public correlationId?: string
  ) { super(message); }
}
```

---

### 4.3 Formulários & Validação

| Critério | Status | Detalhe |
|---|---|---|
| Validação client-side | ⚠️ | Validação básica via HTML5 / estado manual |
| Feedback de erro por campo | ⚠️ | Inconsistente entre formulários |
| Estado de loading em submit | ⚠️ | Nem todos os botões têm estado de loading |
| Acessibilidade (a11y) | ⚠️ | Labels e aria-* não verificados |

**[FE-FORM-01] Adotar React Hook Form + Zod**

```typescript
// schema de validação compartilhado frontend/backend
import { z } from 'zod';

export const createUserSchema = z.object({
  username: z.string().min(3).max(50),
  email: z.string().email(),
  password: z.string().min(8).regex(/[A-Z]/),
  fullName: z.string().min(2),
});

type CreateUserForm = z.infer<typeof createUserSchema>;
```

---

### 4.4 Tipagem TypeScript

**Arquivo referência:** [WebApp/src/interfaces/](WebApp/src/interfaces/)

| Critério | Status | Detalhe |
|---|---|---|
| Interfaces bem definidas | ✅ | AuthUser, PagedResponse, SystemLog |
| Uso de `any` | ⚠️ | Verificar se há any implícitos |
| Enums vs string literals | ⚠️ | Permissões como string ('root', 'users') sem enum |
| tsconfig strict mode | ⚠️ | Verificar se `strict: true` está habilitado |

**[FE-TS-01] Substituir strings por Enum ou const object**

```typescript
// Atual
export const PERMISSIONS = {
  ROOT: 'root',
  USERS: 'users',
  RESOURCES: 'resources',
  REPORTS: 'reports',
} as const;

type Permission = typeof PERMISSIONS[keyof typeof PERMISSIONS];
// Garante que apenas valores válidos são usados
```

**[FE-TS-02] Habilitar strict mode no tsconfig**

```json
{
  "compilerOptions": {
    "strict": true,
    "noUncheckedIndexedAccess": true,
    "exactOptionalPropertyTypes": true
  }
}
```

---

### 4.5 Componentes & UX

| Critério | Status | Detalhe |
|---|---|---|
| Componentização | ✅ | Componentes reutilizáveis bem definidos |
| Estado de carregamento | ⚠️ | Loading states não consistentes |
| Tratamento de erro nas páginas | ⚠️ | Sem Error Boundaries |
| Responsividade | ✅ | MUI com breakpoints |
| Acessibilidade | ⚠️ | Não verificada sistematicamente |

**[FE-UX-01] Adicionar Error Boundaries**

```tsx
// src/components/ErrorBoundary/index.tsx
class ErrorBoundary extends React.Component {
  state = { hasError: false };
  static getDerivedStateFromError() { return { hasError: true }; }
  render() {
    if (this.state.hasError) return <ErrorFallback />;
    return this.props.children;
  }
}
```

**[FE-UX-02] Skeleton loaders nas tabelas**

Substituir telas em branco durante carregamento por `<Skeleton>` do MUI, melhorando percepção de performance.

---

## 5. Segurança

### 5.1 Armazenamento de Tokens

| Critério | Status | Risco |
|---|---|---|
| Token em localStorage | ⚠️ | Médio — vulnerável a XSS |
| authUser em localStorage | ⚠️ | Médio — dados do usuário expostos |
| HttpOnly cookies | ❌ | Não implementado |

**[SEC-01] Migrar access token para memória (in-memory)**

O access token (curta duração) deve viver apenas na memória da aplicação. O refresh token pode permanecer em `HttpOnly cookie` (configurado pelo servidor):

```typescript
// Proposta:
// - access token: variável em memória (state/closure)
// - refresh token: HttpOnly cookie (set-cookie do servidor)
```

Isso elimina o vetor de ataque XSS para roubo de tokens.

**[SEC-02] Content Security Policy (CSP)**

Adicionar headers CSP na configuração do servidor para mitigar XSS:

```csharp
// Program.cs
app.Use(async (context, next) => {
    context.Response.Headers.Add(
        "Content-Security-Policy",
        "default-src 'self'; script-src 'self'; style-src 'self' 'unsafe-inline';"
    );
    await next();
});
```

---

### 5.2 Senhas & Credenciais

| Critério | Status | Detalhe |
|---|---|---|
| Hashing com BCrypt | ✅ | BCrypt.Net-Next, salt rounds: 10 |
| Credenciais padrão inseguras | ⚠️ | root/root1234 — deve ser trocado em produção |
| Política de senha | ⚠️ | Validação mínima, sem regras de complexidade |
| Rate limit em login | ❌ | Não implementado — vulnerável a brute force |

**[SEC-03] Rate Limiting no endpoint de login**

```csharp
// Program.cs (.NET 8 rate limiting nativo)
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("login", o => {
        o.PermitLimit = 5;
        o.Window = TimeSpan.FromMinutes(15);
        o.QueueLimit = 0;
    });
});

// AuthController.cs
[RateLimiter("login")]
[HttpPost("login")]
public async Task<IActionResult> Login(...)
```

**[SEC-04] Forçar troca de senha padrão**

Adicionar flag `RequirePasswordChange` no modelo `User` para forçar troca no primeiro login do usuário root (ou de qualquer usuário criado com senha temporária).

---

### 5.3 Autorização & RBAC

| Critério | Status | Detalhe |
|---|---|---|
| Middleware de permissões | ✅ | ValidateUserPermissions.cs |
| Proteção de rotas no frontend | ✅ | ProtectedRoute.tsx |
| Verificação dupla (frontend + backend) | ✅ | RBAC em ambas as camadas |
| Princípio do menor privilégio | ✅ | Usuário precisa de permissão explícita |

---

## 6. Banco de Dados & ORM

### 6.1 Entity Framework Core

**Arquivo referência:** [Api/Data/ApiDbContext.cs](Api/Data/ApiDbContext.cs)

| Critério | Status | Detalhe |
|---|---|---|
| Configuração via Fluent API | ✅ | Configurations/ bem organizados |
| Auto-audit (CreatedAt/UpdatedAt) | ✅ | AuditableEntity + SaveChanges override |
| Migrations versionadas | ✅ | Uma migration inicial |
| N+1 query problem | ⚠️ | Verificar includes explícitos nas queries |
| Paginação eficiente | ✅ | PagedResult com Skip/Take |

**[DB-01] Verificar N+1 em consultas com relacionamentos**

```csharp
// Potencial N+1
var users = await _context.Users.ToListAsync();
foreach (var user in users)
    user.Permissions = await _context.Permissions.Where(...).ToListAsync();

// Correto: Include explícito
var users = await _context.Users
    .Include(u => u.AccessPermissions)
        .ThenInclude(ap => ap.SystemResource)
    .ToListAsync();
```

**[DB-02] AsNoTracking para queries somente leitura**

```csharp
// Em todas as queries GET
var users = await _context.Users
    .AsNoTracking()
    .Where(u => u.Active)
    .ToListAsync();
```

**[DB-03] Compiled Queries para hot paths**

```csharp
private static readonly Func<ApiDbContext, int, Task<User?>> GetUserByIdQuery =
    EF.CompileAsyncQuery((ApiDbContext ctx, int id) =>
        ctx.Users.FirstOrDefault(u => u.Id == id));
```

---

### 6.2 Migrations & Seeding

| Critério | Status | Detalhe |
|---|---|---|
| Migration inicial | ✅ | InitialCreate bem estruturado |
| Auto-migration em startup | ✅ | DbInitializer.cs |
| Seed de dados essenciais | ✅ | System resources + root user |
| Seed condicional (prod/dev) | ⚠️ | RUN_USERS_SEED por env var |
| Backup antes de migration | ❌ | Não automatizado |

---

## 7. Testes & Qualidade

### 7.1 Estado Atual

| Tipo | Status | Cobertura |
|---|---|---|
| Testes unitários (backend) | ❌ | Não encontrados |
| Testes de integração (backend) | ❌ | Não encontrados |
| Testes unitários (frontend) | ❌ | Não encontrados |
| Testes E2E | ❌ | Não encontrados |
| Linting | ✅ | ESLint configurado no frontend |
| Análise estática C# | ⚠️ | Não verificado (Roslyn Analyzers) |

> O CI/CD executa `dotnet test`, mas nenhum projeto de teste foi encontrado. Este é o maior gap de qualidade do projeto.

---

### 7.2 Plano de Implementação de Testes

**Backend — xUnit + Testcontainers**

```csharp
// Estrutura proposta
Api.Tests/
  ├── Unit/
  │   ├── Services/
  │   │   ├── CreateUserTests.cs
  │   │   ├── AuthServicesTests.cs
  │   │   └── JwtServicesTests.cs
  │   └── Validators/
  │       └── UserValidatorTests.cs
  ├── Integration/
  │   ├── Controllers/
  │   │   ├── AuthControllerTests.cs
  │   │   └── UsersControllerTests.cs
  │   └── Fixtures/
  │       └── ApiTestFixture.cs  // Testcontainers PostgreSQL
  └── Api.Tests.csproj
```

```csharp
// Exemplo: teste de integração com banco real via Testcontainers
public class AuthControllerTests : IClassFixture<ApiTestFixture>
{
    [Fact]
    public async Task Login_WithValidCredentials_ReturnsToken()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/login", new {
            Username = "root",
            Password = "root1234"
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<LoginResponse>();
        body!.Token.Should().NotBeNullOrEmpty();
    }
}
```

**Frontend — Vitest + Testing Library**

```typescript
// Estrutura proposta
WebApp/src/
  └── __tests__/
      ├── components/
      │   ├── LoginForm.test.tsx
      │   └── UsersTable.test.tsx
      ├── hooks/
      │   └── useAuth.test.ts
      └── services/
          └── authServices.test.ts
```

```typescript
// Exemplo: teste do hook useAuth
import { renderHook, act } from '@testing-library/react';
import { useAuth } from '../hooks/useAuth';

test('handleLogin stores token on success', async () => {
  const { result } = renderHook(() => useAuth(), { wrapper: AuthProvider });

  await act(async () => {
    await result.current.handleLogin({ username: 'root', password: 'root1234' });
  });

  expect(result.current.authUser).not.toBeNull();
  expect(result.current.token).not.toBeNull();
});
```

---

## 8. Infraestrutura & CI/CD

### 8.1 Docker

| Critério | Status | Detalhe |
|---|---|---|
| Multi-stage build (API) | ✅ | build → runtime (imagem menor) |
| Hot reload em dev | ✅ | Dockerfile.dev com `dotnet watch` |
| Healthcheck no banco | ✅ | pg_isready configurado |
| Variáveis sensíveis | ⚠️ | `.env` copiado para o container em dev |
| Imagem base atualizada | ⚠️ | Verificar CVEs nas imagens base |

**[INFRA-01] Usar Docker secrets em produção**

Em vez de `.env` no container, utilizar Docker secrets ou variáveis de ambiente injetadas pelo orquestrador (Kubernetes Secrets, AWS Parameter Store, etc.).

**[INFRA-02] Adicionar HEALTHCHECK nos Dockerfiles**

```dockerfile
# Api/Dockerfile
HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
  CMD curl -f http://localhost:${API_PORT}/health || exit 1
```

---

### 8.2 GitHub Actions

| Critério | Status | Detalhe |
|---|---|---|
| Pipeline build + test | ✅ | build-and-test.yml |
| PR checks | ✅ | build-test-pr.yml |
| Semantic release | ✅ | .releaserc configurado |
| Push para DockerHub | ✅ | Após testes |
| Deploy automático | ✅ | Via webhook staging/production |
| Secrets no Actions | ⚠️ | Verificar se JWT_SECRET está em Secrets |
| Dependabot | ❌ | Não configurado |

**[CI-01] Adicionar Dependabot**

```yaml
# .github/dependabot.yml
version: 2
updates:
  - package-ecosystem: nuget
    directory: /Api
    schedule:
      interval: weekly
  - package-ecosystem: npm
    directory: /WebApp
    schedule:
      interval: weekly
  - package-ecosystem: docker
    directory: /
    schedule:
      interval: weekly
```

**[CI-02] Adicionar etapa de análise de segurança**

```yaml
# Adicionar no workflow principal
- name: Run Trivy vulnerability scan
  uses: aquasecurity/trivy-action@master
  with:
    scan-type: fs
    format: table
    exit-code: 1
    severity: CRITICAL,HIGH
```

---

## 9. Documentação

| Item | Status | Qualidade |
|---|---|---|
| README.md | ✅ | Excelente — completo |
| DOCS/ (7 arquivos) | ✅ | Excelente — cobertura ampla |
| Swagger/OpenAPI | ✅ | Habilitado em desenvolvimento |
| Comentários no código | ⚠️ | Esparsos — ausentes em lógicas complexas |
| REFACTOR.md | ✅ | Boa — direciona evolução da arquitetura |
| CHANGELOG.md | ✅ | Automatizado via Semantic Release |

**[DOC-01] Documentar endpoints com XML comments**

```csharp
/// <summary>
/// Autentica o usuário e retorna JWT + refresh token.
/// </summary>
/// <param name="dto">Credenciais de login</param>
/// <returns>Token de acesso e dados do usuário autenticado</returns>
/// <response code="200">Login realizado com sucesso</response>
/// <response code="401">Credenciais inválidas</response>
[HttpPost("login")]
[ProducesResponseType(typeof(LoginResponseDto), 200)]
[ProducesResponseType(401)]
public async Task<IActionResult> Login([FromBody] LoginDto dto) { ... }
```

**[DOC-02] Adicionar ADR (Architecture Decision Records)**

Para decisões como escolha de JWT vs Session, BCrypt vs Argon2, localStorage vs cookies. Arquivo simples em `/DOCS/ADR/` preserva o contexto das escolhas.

---

## 10. Matriz de Riscos

| ID | Área | Risco | Probabilidade | Impacto | Prioridade |
|---|---|---|---|---|---|
| R-01 | Segurança | Tokens em localStorage vulneráveis a XSS | Média | Alto | 🔴 Alta |
| R-02 | Segurança | Sem rate limit no login — brute force | Alta | Alto | 🔴 Alta |
| R-03 | Qualidade | Zero testes automatizados | Alta | Alto | 🔴 Alta |
| R-04 | Segurança | Access token com 8h de expiração | Média | Médio | 🟡 Média |
| R-05 | Performance | N+1 queries em relacionamentos | Média | Médio | 🟡 Média |
| R-06 | Manutenção | Logs sem política de retenção | Baixa | Médio | 🟡 Média |
| R-07 | Segurança | Credenciais padrão root/root1234 | Alta | Baixo | 🟡 Média |
| R-08 | DevOps | Sem Dependabot — deps desatualizadas | Alta | Baixo | 🟢 Baixa |
| R-09 | UX | Sem Error Boundaries no React | Média | Baixo | 🟢 Baixa |
| R-10 | Infraestrutura | Sem HEALTHCHECK nos Dockerfiles | Baixa | Baixo | 🟢 Baixa |

---

## 11. Roadmap de Melhorias

### Sprint 1 — Segurança Crítica (semana 1–2)

- [ ] **[SEC-03]** Rate limiting no endpoint de login
- [ ] **[SEC-JWT-01]** Reduzir expiração do access token para 15–30 min
- [ ] **[SEC-JWT-02]** Refresh Token Rotation
- [ ] **[SEC-01]** Mover access token para memória (iniciar estudo de migração)

### Sprint 2 — Qualidade de Código (semana 3–4)

- [ ] **[SVC-01]** Adotar FluentValidation no backend
- [ ] **[FE-FORM-01]** React Hook Form + Zod no frontend
- [ ] **[FE-TS-02]** strict mode no tsconfig
- [ ] **[API-01]** Retornar 201 Created com Location Header

### Sprint 3 — Observabilidade (semana 5–6)

- [ ] **[ERR-01]** Serilog com structured logging
- [ ] **[ERR-02]** Correlation ID nas requests
- [ ] **[INFRA-02]** HEALTHCHECK nos Dockerfiles
- [ ] **[AUD-02]** Índices nas colunas de logs

### Sprint 4 — Testes (semana 7–8)

- [ ] Criar projeto `Api.Tests` com xUnit
- [ ] Configurar Testcontainers para PostgreSQL
- [ ] Escrever testes unitários dos services críticos
- [ ] Configurar Vitest no frontend
- [ ] Escrever testes dos hooks principais

### Sprint 5 — Performance & DevOps (semana 9–10)

- [ ] **[DB-02]** AsNoTracking em queries de leitura
- [ ] **[DB-01]** Auditoria de N+1 queries
- [ ] **[CI-01]** Dependabot para deps e Docker
- [ ] **[CI-02]** Trivy scan na pipeline
- [ ] **[AUD-01]** Política de retenção de logs

---

## Referências de Código

| Arquivo | Relevância |
|---|---|
| [Api/Program.cs](Api/Program.cs) | Bootstrap, DI, Middleware chain |
| [Api/Security/Jwt/JwtServices.cs](Api/Security/Jwt/JwtServices.cs) | Token creation/validation |
| [Api/Middlewares/ValidateUserPermissions.cs](Api/Middlewares/ValidateUserPermissions.cs) | RBAC enforcement |
| [Api/Middlewares/ExceptionHandler.cs](Api/Middlewares/ExceptionHandler.cs) | Global error handling |
| [Api/Data/ApiDbContext.cs](Api/Data/ApiDbContext.cs) | EF Core context + auto-audit |
| [WebApp/src/api/index.ts](WebApp/src/api/index.ts) | Axios + refresh interceptors |
| [WebApp/src/contexts/AuthContext.tsx](WebApp/src/contexts/AuthContext.tsx) | Auth state management |
| [WebApp/src/routes/index.tsx](WebApp/src/routes/index.tsx) | Route definitions + RBAC |
| [WebApp/src/permissions/Rules.ts](WebApp/src/permissions/Rules.ts) | Frontend permission logic |

---

*Documento gerado em 2026-03-30. Revisar e atualizar conforme o projeto evolui.*
