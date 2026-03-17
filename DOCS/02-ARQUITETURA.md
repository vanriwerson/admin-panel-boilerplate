# Arquitetura do Sistema

Este documento descreve a arquitetura completa do Admin Panel Boilerplate, incluindo estrutura de pastas, padrões de design e fluxos principais.

## Índice

1. [Visão Geral](#visão-geral)
2. [Arquitetura Geral](#arquitetura-geral)
3. [Estrutura Backend](#estrutura-backend)
4. [Estrutura Frontend](#estrutura-frontend)
5. [Fluxo de Dados](#fluxo-de-dados)
6. [Segurança](#segurança)
7. [Banco de Dados](#banco-de-dados)

## Visão Geral

O Admin Panel Boilerplate segue uma arquitetura em camadas com separação clara entre frontend e backend:

```
┌─────────────────────────────────────────┐
│          Frontend (React)               │
│  - Interface de Usuário                 │
│  - Gerenciamento de Estado              │
│  - Roteamento e Navegação               │
└──────────────┬──────────────────────────┘
               │ HTTP/REST
               │ (JSON + JWT)
┌──────────────▼──────────────────────────┐
│          Backend (.NET API)             │
│  - Controllers REST                     │
│  - Business Logic (Services)            │
│  - Middlewares de Autenticação          │
└──────────────┬──────────────────────────┘
               │ Entity Framework
               │
┌──────────────▼──────────────────────────┐
│      Banco de Dados (PostgreSQL)        │
│  - Usuários e Permissões                │
│  - Recursos do Sistema                  │
│  - Logs de Auditoria                    │
└─────────────────────────────────────────┘
```

## Arquitetura Geral

### Stack Tecnológica

**Backend:**

- Framework: ASP.NET Core 8.0
- ORM: Entity Framework Core 9.0.9
- Banco de Dados: PostgreSQL 14+
- Autenticação: JWT (System.IdentityModel.Tokens.Jwt 8.14.0)
- Hashing: BCrypt.Net-Next 4.0.3
- Email: Resend API 0.1.7
- Documentação: Swagger/OpenAPI (Swashbuckle.AspNetCore 6.6.2, Microsoft.AspNetCore.OpenApi 8.0.20)
- Variáveis de Ambiente: DotNetEnv 3.1.1

**Frontend:**

- Framework: React 19.1.1
- Build Tool: Vite 7.1.7
- Linguagem: TypeScript 5.9.3
- UI Framework: Material-UI 7.3.4
- Roteamento: React Router 7.9.4
- HTTP Client: Axios 1.12.2
- Date Utils: date-fns 4.1.0
- Ícones: FontAwesome 7.1.0

**Infraestrutura & CI/CD:**

- Containerização: Docker
- Orquestração: Docker Compose
- CI/CD: GitHub Actions
- Versionamento: Semantic Release 25.0.2

### Padrões de Design

#### Backend

1. **Repository Pattern**
   - `GenericRepository<T>` para operações CRUD
   - Abstração do acesso a dados
   - Facilita testes e manutenção

2. **Service Layer Pattern**
   - Lógica de negócio isolada em services
   - Controllers apenas delegam para services
   - Cada operação em um service específico

3. **DTO Pattern**
   - Data Transfer Objects para entrada/saída
   - Separação entre modelo de domínio e API
   - Validação de dados centralizada

4. **Middleware Pipeline**
   - Autenticação via middleware
   - Validação de permissões centralizada
   - Tratamento global de exceções

#### Frontend

1. **Component-Based Architecture**
   - Componentes reutilizáveis
   - Separação de responsabilidades
   - Props e composition

2. **Context API**
   - Gerenciamento de estado global
   - AuthContext para autenticação
   - ThemeContext para tema

3. **Custom Hooks**
   - Lógica reutilizável
   - Encapsulamento de side effects
   - Hooks para cada domínio (useUsers, useResources, etc)

4. **Protected Routes**
   - Roteamento baseado em permissões
   - Componente ProtectedRoute
   - Redirecionamento automático

## Estrutura Backend

### Organização de Pastas

```
Api/
├── Controllers/              # Endpoints REST
│   ├── AuthController.cs
│   ├── UsersController.cs
│   ├── SystemLogsController.cs
│   ├── SystemResourcesController.cs
│   ├── SystemStatsController.cs
│   └── PasswordsController.cs
│
├── Services/                 # Lógica de Negócio
│   ├── AccessPermissions/
│   ├── RefreshTokens/
│   ├── SystemResources/
│   ├── SystemStatsServices/
│   └── Users/
│
├── Models/                   # Entidades do Banco
│   ├── User.cs
│   ├── SystemResource.cs
│   ├── AccessPermission.cs
│   ├── SystemLog.cs
│   ├── RefreshToken.cs
│   └── Common/
│       └── AuditableEntity.cs
│
├── Dtos/                     # Data Transfer Objects
│   ├── AccessPermissionDtos/
│   ├── AuthDtos/
│   ├── PasswordDtos/
│   ├── SystemLogDtos/
│   ├── SystemResourceDtos/
│   ├── SystemStatsDtos/
│   └── UserDtos/
│
├── Data/                     # Configuração do Banco
│   ├── ApiDbContext.cs
│   ├── DbInitializer.cs
│   └── Configurations/
│
├── Repositories/             # Acesso a Dados
│   ├── AccessPermissionRepository.cs
│   ├── RefreshTokenRepository.cs
│   ├── SystemLogRepository.cs
│   ├── SystemResourceRepository.cs
│   └── UserRepository.cs
│
├── Security/                 # Autenticação e Autorização
│   ├── Auth/
│   ├── Jwt/
│   ├── Passwords/
│   ├── Permissions/
│   ├── Policies/
│   └── RefreshTokens/
│
├── Middlewares/              # Pipeline HTTP
│   ├── RequireAuthorization.cs
│   ├── ValidateUserPermissions.cs
│   ├── ExceptionHandler.cs
│   └── AppException.cs
│
├── Auditing/                 # Sistema de Logs
│   ├── SystemLogActionFactory.cs
│   └── SystemLogDataSerializer.cs
│
├── Extensions/               # Injeção de Dependências
│   └── DependencyInjection/
│
├── Interfaces/               # Contratos
│   └── Repositories/
│
├── Helpers/                  # Utilitários
│   ├── EnvLoader.cs
│   ├── Logger.cs
│   └── Pagination/
│
├── Mappers/                  # Mapeamento de Dados
│   └── UserMapper.cs
│
├── Validations/              # Validações Customizadas
│
└── Program.cs               # Configuração e Bootstrap
```

### Camadas do Backend

#### 1. Controllers (Camada de Apresentação)

Responsabilidades:

- Receber requisições HTTP
- Validar entrada básica
- Delegar para services
- Retornar respostas HTTP

Exemplo:

```csharp
[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    // Injeta apenas o service necessário
    // Retorna ActionResult apropriado
    // Não contém lógica de negócio
}
```

#### 2. Services (Camada de Negócio)

Responsabilidades:

- Implementar regras de negócio
- Orquestrar operações
- Validar dados complexos
- Chamar repositórios
- Registrar logs

Exemplo:

```csharp
public class CreateUser
{
    // Recebe DTOs
    // Valida regras de negócio
    // Cria entidade
    // Salva no banco via repository
    // Registra log de auditoria
    // Retorna DTO de resposta
}
```

#### 3. Repositories (Camada de Acesso a Dados)

Responsabilidades:

- Abstrair acesso ao banco
- Operações CRUD genéricas
- Queries customizadas
- Soft delete

Exemplo:

```csharp
public class GenericRepository<T> : IGenericRepository<T>
{
    // CreateAsync, GetAllAsync, GetByIdAsync
    // UpdateAsync, DeleteAsync (soft)
    // SearchAsync (busca textual)
}
```

#### 4. Middlewares (Pipeline)

**RequireAuthorization:**

- Valida presença de token JWT
- Verifica assinatura e expiração
- Extrai claims do usuário

**ValidateUserPermissions:**

- Mapeia endpoint → permissão requerida
- Verifica se usuário tem a permissão
- Bloqueio de atribuição de permissões root

**ExceptionHandler:**

- Captura exceções não tratadas
- Retorna respostas padronizadas
- Log de erros

### Fluxo de Requisição (Backend)

```
HTTP Request
    │
    ▼
RequireAuthorization Middleware
    │ (valida JWT)
    ▼
ValidateUserPermissions Middleware
    │ (valida permissão)
    ▼
Controller
    │ (valida entrada básica)
    ▼
Service
    │ (lógica de negócio)
    ▼
Repository
    │ (acesso ao banco)
    ▼
Entity Framework
    │
    ▼
PostgreSQL
    │
    ▼
Response (DTO)
```

## Estrutura Frontend

### Organização de Pastas

```
WebApp/src/
├── api/                      # Configuração HTTP
│   └── index.ts              # Axios instance
│
├── components/               # Componentes reutilizáveis
│   ├── LoginForm/
│   ├── UserForm/
│   ├── UsersTable/
│   ├── SystemResourceForm/
│   ├── SystemResourcesTable/
│   ├── ReportsTable/
│   ├── UsersSelect/
│   ├── SystemResourcesSelect/
│   ├── SidePanel/
│   └── ...
│
├── contexts/                 # Estado Global
│   ├── AuthContext.tsx       # Autenticação
│   ├── NotificationContext.tsx # Notificações
│   ├── PermissionsContext.tsx # Permissões RBAC
│   ├── SystemResourcesContext.tsx # Recursos do sistema
│   ├── ThemeContext.tsx      # Tema claro/escuro
│   └── UsersContext.tsx      # Usuários
│
├── hooks/                    # Hooks Customizados
│   ├── useAuth.ts
│   ├── useNotification.ts
│   ├── usePermissions.ts
│   ├── useReports.ts
│   ├── useSystemResources.ts
│   ├── useThemeMode.ts
│   └── useUsers.ts
│
├── interfaces/               # TypeScript Types
│   ├── User.ts
│   ├── SystemResource.ts
│   ├── SystemLog.ts
│   └── ...
│
├── layouts/                  # Layouts de Página
│   ├── DefaultLayout.tsx     # Com menu lateral
│   └── CleanLayout.tsx       # Sem menu (login)
│
├── pages/                    # Páginas/Views
│   ├── Login.tsx
│   ├── PasswordReset.tsx
│   ├── Profile.tsx
│   ├── Users.tsx
│   ├── Resources.tsx
│   └── Reports.tsx
│
├── permissions/              # Sistema RBAC
│   ├── PermissionsMap.ts     # IDs de permissões
│   ├── Rules.ts              # Regras de negócio
│   └── MenuVisibility.ts     # Filtro de menu
│
├── routes/                   # Configuração de Rotas
│   ├── index.tsx             # Router principal
│   └── ProtectedRoute.tsx    # HOC para proteção
│
├── services/                 # Comunicação com API
│   ├── authServices.ts
│   ├── usersServices/
│   ├── systemResourcesServices/
│   └── systemLogsServices.ts
│
├── helpers/                  # Utilitários
│   ├── dateFormatter.ts
│   └── ...
│
├── theme.ts                  # Tema Material-UI
├── App.tsx                   # Componente raiz
└── main.tsx                  # Entry point
```

### Camadas do Frontend

#### 1. Pages (Camada de Apresentação)

Responsabilidades:

- Renderizar a interface
- Compor componentes
- Usar hooks customizados
- Não contêm lógica complexa

#### 2. Components (Componentes Reutilizáveis)

Responsabilidades:

- UI components isolados
- Recebem props
- Emitem eventos
- Stateless quando possível

#### 3. Hooks (Lógica de Negócio)

Responsabilidades:

- Encapsular lógica reutilizável
- Gerenciar estado local
- Side effects (API calls)
- Retornar dados e funções

Exemplo:

```typescript
export const useUsers = () => {
  const [users, setUsers] = useState([]);
  const [loading, setLoading] = useState(false);

  const fetchUsers = async () => {
    // API call
  };

  const createUser = async (data) => {
    // API call + atualiza estado
  };

  return { users, loading, fetchUsers, createUser };
};
```

#### 4. Services (Camada de Comunicação)

Responsabilidades:

- Fazer chamadas HTTP
- Transformar dados
- Tratar erros básicos
- Retornar promises

Exemplo:

```typescript
export const listUsers = async (page: number, limit: number) => {
  const response = await api.get("/users", {
    params: { page, limit },
  });
  return response.data;
};
```

#### 5. Contexts (Estado Global)

Responsabilidades:

- Compartilhar estado entre componentes
- Prover métodos de atualização
- Persistir dados (localStorage)

**AuthContext:**

- Token JWT
- Dados do usuário
- Permissões
- Métodos de login/logout

**ThemeContext:**

- Tema atual (light/dark)
- Método para alternar

### Fluxo de Dados (Frontend)

```
User Interaction
    │
    ▼
Component
    │ (event handler)
    ▼
Custom Hook
    │ (business logic)
    ▼
Service
    │ (HTTP request)
    ▼
Axios Interceptor
    │ (add JWT token)
    ▼
Backend API
    │
    ◄─── Response
    │
    ▼
Hook Updates State
    │
    ▼
Component Re-renders
```

## Fluxo de Dados

### Fluxo de Autenticação

```
1. User submits login form
   │
   ▼
2. LoginForm → handleLogin (AuthContext)
   │
   ▼
3. authServices.login(credentials)
   │
   ▼
4. POST /api/auth/login
   │
   ▼
5. Backend validates credentials
   │
   ▼
6. JWT generated with claims
   │
   ▼
7. Response: { token, user, permissions }
   │
   ▼
8. AuthContext stores in localStorage
   │
   ▼
9. Axios interceptor sets Authorization header
   │
   ▼
10. Redirect to protected route
```

### Fluxo de Proteção de Rotas

**Frontend:**

```
Route access attempt
    │
    ▼
ProtectedRoute component
    │
    ├─ No token? → Redirect to /login
    │
    ├─ No permission? → Redirect to /unauthorized
    │
    └─ Has permission → Render component
```

**Backend:**

```
HTTP Request
    │
    ▼
RequireAuthorization
    │
    ├─ No token? → 401 Unauthorized
    │
    ├─ Invalid token? → 401 Unauthorized
    │
    └─ Valid token → Continue
        │
        ▼
ValidateUserPermissions
    │
    ├─ Is root? → Allow
    │
    ├─ Has permission? → Allow
    │
    └─ No permission? → 403 Forbidden
```

### Fluxo de CRUD

Exemplo: Criar Usuário

```
Frontend:
1. User fills UserForm
2. Submit → handleCreateUser (useUsers hook)
3. Hook calls createUser service
4. Service: POST /api/users
   ↓

Backend:
5. UsersController.Create(CreateUserDto)
6. CreateUser.ExecuteAsync(dto)
7. Validates business rules:
   - Email único
   - Username único
   - Permissões válidas
8. Hashes password with BCrypt
9. Creates User entity
10. Repository.CreateAsync(user)
11. EF Core saves to PostgreSQL
12. CreateSystemLog (audit)
13. Returns UserResponseDto
    ↓

Frontend:
14. Service returns user data
15. Hook updates local state
16. Component re-renders
17. Shows success notification
18. Refreshes user list
```

## Segurança

### Autenticação JWT

**Token Structure:**

```json
{
  "header": {
    "alg": "HS256",
    "typ": "JWT"
  },
  "payload": {
    "id": "1",
    "username": "root",
    "email": "root@admin.com",
    "permissions": "1,2,3,4",
    "exp": 1234567890
  },
  "signature": "..."
}
```

**Claims:**

- `id`: ID do usuário
- `username`: Nome de usuário
- `email`: Email
- `permissions`: IDs de permissões (comma-separated)
- `exp`: Expiração (7 dias)

### Proteção de Endpoints

**Middleware Pipeline:**

1. `RequireAuthorization`: Valida presença e validade do JWT
2. `ValidateUserPermissions`: Valida permissões específicas

**Regras de Permissão:**

- Usuário root (ID 1) tem acesso total
- Endpoints mapeados para permissões específicas
- Validação de atribuição de permissões root/resources

### Hashing de Senhas

- Algoritmo: BCrypt
- Work factor: 12
- Sal automático por senha

### CORS

Configurado para aceitar requisições apenas do frontend:

```csharp
options.AddPolicy("AllowWebApp", builder =>
{
    builder.WithOrigins(Environment.GetEnvironmentVariable("WEB_APP_URL"))
           .AllowAnyHeader()
           .AllowAnyMethod();
});
```

## Banco de Dados

### Modelo de Dados

```
┌─────────────┐       ┌────────────────────┐       ┌─────────────────┐
│    users    │       │ access_permissions │       │ system_resources│
├─────────────┤       ├────────────────────┤       ├─────────────────┤
│ id (PK)     │◄──────│ user_id (FK)       │      ┌│ id (PK)         │
│ username    │       │ system_resource_id │──────┘│ name            │
│ email       │       │ id (PK)            │       │ exhibition_name │
│ password    │       │ created_at         │       │ active          │
│ full_name   │       │ updated_at         │       │ created_at      │
│ active      │       └────────────────────┘       │ updated_at      │
│ created_at  │                                     └─────────────────┘
│ updated_at  │
└─────────────┘
      │
      │ 1:N
      ▼
┌──────────────┐       ┌────────────────────┐
│ system_logs  │       │  refresh_tokens    │
├──────────────┤       ├────────────────────┤
│ id (PK)      │       │ id (PK)            │
│ user_id (FK) │       │ token_hash         │
│ action       │       │ expires_at         │
│ data         │       │ is_revoked         │
│ created_at   │       │ created_at         │
│ ip_address   │       │ revoked_at         │
│ generated_by │       │ user_id (FK)       │
└──────────────┘       └────────────────────┘
```

### Relacionamentos

- **users ↔ system_resources**: Many-to-Many (via access_permissions)
- **users → system_logs**: One-to-Many
- **users → refresh_tokens**: One-to-Many

### Índices

- `users.email`: UNIQUE
- `users.username`: UNIQUE
- `system_resources.name`: UNIQUE
- Foreign keys têm índices automáticos

### Migrations

Entity Framework Core gerencia o schema:

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### Seeds

**Dados Iniciais (sempre criados):**

- Usuário root (username: root, senha: root1234)
- 4 recursos do sistema (root, users, resources, reports)

**Dados de Teste (opcional - RUN_USERS_SEED=true):**

- 10 usuários: alice, bob, carol, dave, eve, frank, grace, heidi, ivan, judy
- Sem permissões por padrão

## Próximos Passos

Agora que você entende a arquitetura, explore:

1. [Backend - Detalhes da API](./03-BACKEND.md)
2. [Frontend - Detalhes do WebApp](./04-FRONTEND.md)
3. [API Reference - Endpoints](./05-API-REFERENCE.md)
4. [Sistema de Permissões](./06-PERMISSOES.md)
