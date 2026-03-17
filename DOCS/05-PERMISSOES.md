# Sistema de Permissões (RBAC)

Documentação completa do sistema de Role-Based Access Control (RBAC).

## Índice

1. [Visão Geral](#visão-geral)
2. [Recursos do Sistema](#recursos-do-sistema)
3. [Modelo de Dados](#modelo-de-dados)
4. [Regras de Permissão](#regras-de-permissão)
5. [Implementação Backend](#implementação-backend)
6. [Implementação Frontend](#implementação-frontend)
7. [Exemplos Práticos](#exemplos-práticos)

## Visão Geral

O sistema utiliza RBAC (Role-Based Access Control) para controlar o acesso a recursos e funcionalidades.

**Conceitos principais:**

- **Usuários**: Entidades que usam o sistema
- **Recursos**: Funcionalidades/módulos do sistema
- **Permissões**: Associação entre usuário e recurso
- **Root**: Usuário com acesso total (ID 1)

**Características:**

- Controle granular por recurso
- Usuário pode ter múltiplas permissões
- Validação no backend e frontend
- Usuário root tem acesso irrestrito
- Proteção contra escalação de privilégios

## Recursos do Sistema

### Recursos Padrão (Seeds)

| ID  | Name      | Exhibition Name | Descrição                            |
| --- | --------- | --------------- | ------------------------------------ |
| 1   | root      | Administrador   | Acesso total ao sistema              |
| 2   | users     | Usuários        | Gerenciamento de usuários            |
| 3   | resources | Recursos        | Gerenciamento de recursos do sistema |
| 4   | reports   | Relatórios      | Visualização de logs e auditoria     |

### Hierarquia de Permissões

```
root (ID: 1)
├─ Acesso total a tudo
├─ Pode atribuir qualquer permissão
├─ Pode editar qualquer usuário
└─ Pode deletar qualquer usuário

users (ID: 2)
├─ Gerenciar usuários
├─ Criar/editar/deletar usuários
├─ Pode atribuir permissões (exceto root e resources)
└─ Não pode editar usuário root

resources (ID: 3)
├─ Gerenciar recursos do sistema
├─ Criar/editar/deletar recursos
└─ APENAS root pode atribuir esta permissão

reports (ID: 4)
├─ Visualizar logs de auditoria
├─ Filtrar relatórios
└─ Somente leitura
```

## Modelo de Dados

### Tabelas

#### users

```sql
CREATE TABLE users (
  id SERIAL PRIMARY KEY,
  username VARCHAR(255) UNIQUE NOT NULL,
  email VARCHAR(255) UNIQUE NOT NULL,
  password VARCHAR(255) NOT NULL,
  full_name VARCHAR(255) NOT NULL,
  active BOOLEAN DEFAULT true,
  created_at TIMESTAMP DEFAULT NOW(),
  updated_at TIMESTAMP DEFAULT NOW()
);
```

#### system_resources

```sql
CREATE TABLE system_resources (
  id SERIAL PRIMARY KEY,
  name VARCHAR(255) UNIQUE NOT NULL,
  exhibition_name VARCHAR(255) NOT NULL,
  active BOOLEAN DEFAULT true,
  created_at TIMESTAMP DEFAULT NOW(),
  updated_at TIMESTAMP DEFAULT NOW()
);
```

#### access_permissions

```sql
CREATE TABLE access_permissions (
  id SERIAL PRIMARY KEY,
  user_id INTEGER REFERENCES users(id),
  system_resource_id INTEGER REFERENCES system_resources(id),
  created_at TIMESTAMP DEFAULT NOW(),
  updated_at TIMESTAMP DEFAULT NOW(),
  UNIQUE(user_id, system_resource_id)
);
```

### Relacionamentos

```
users (1) ←→ (N) access_permissions (N) ←→ (1) system_resources
```

- Um usuário pode ter múltiplas permissões
- Um recurso pode ser atribuído a múltiplos usuários
- A tabela `access_permissions` é a tabela associativa

## Regras de Permissão

### Regras Gerais

1. **Usuário Root (ID 1)**
   - Tem acesso a TODOS os recursos
   - Não precisa ter permissões explícitas
   - Não pode ser deletado
   - Não pode ter suas permissões editadas por outros

2. **Permissão Root (ID 1)**
   - APENAS usuário root pode atribuir
   - Outros usuários não podem criar usuários root
   - Não aparece em formulários para não-root

3. **Permissão Resources (ID 3)**
   - APENAS usuário root pode atribuir
   - Gerenciar recursos é sensível
   - Não aparece em formulários para não-root

4. **Permissão Users (ID 2)**
   - Permite gerenciar usuários
   - Não pode atribuir permissões root ou resources
   - Não pode editar usuário root

5. **Permissão Reports (ID 4)**
   - Apenas visualização
   - Não permite alterações

### Regras de Atribuição

| Usuário Autenticado | Pode Atribuir                           |
| ------------------- | --------------------------------------- |
| Root                | root, users, resources, reports (todas) |
| Com permissão users | users, reports (apenas essas)           |
| Sem permissão users | Nenhuma                                 |

### Regras de Edição

| Ação                  | Root | Com permissão users | Sem permissão |
| --------------------- | ---- | ------------------- | ------------- |
| Editar próprio perfil | ✓    | ✓                   | ✓             |
| Editar própria senha  | ✓    | ✓                   | ✓             |
| Editar outro usuário  | ✓    | ✓ (exceto root)     | ✗             |
| Editar senha de outro | ✓    | ✗                   | ✗             |
| Editar permissões     | ✓    | ✓ (limitado)        | ✗             |
| Deletar usuário       | ✓    | ✓ (exceto root)     | ✗             |

## Implementação Backend

### Middleware: ValidateUserPermissions

**Arquivo:** `Api/Middlewares/ValidateUserPermissions.cs:1`

**Lógica:**

```csharp
public async Task InvokeAsync(HttpContext context)
{
    // 1. Extrai permissões do JWT
    var token = context.Request.Headers["Authorization"].ToString().Split(" ").Last();
    var decodedToken = JsonWebToken.Decode(token);
    var userId = int.Parse(decodedToken.Claims.First(c => c.Type == "id").Value);
    var permissions = decodedToken.Claims.First(c => c.Type == "permissions").Value;
    var permissionIds = permissions.Split(',').Select(int.Parse).ToList();

    // 2. Identifica se é root
    var isRootUser = userId == 1;

    // 3. Mapeia endpoint → permissão requerida
    var path = context.Request.Path.Value;
    var requiredPermission = EndpointPermissions.Map.FirstOrDefault(
        ep => path.StartsWith(ep.Key)
    ).Value;

    // 4. Valida acesso
    if (requiredPermission != 0 && !isRootUser && !permissionIds.Contains(requiredPermission))
    {
        context.Response.StatusCode = 403;
        await context.Response.WriteAsync("Forbidden");
        return;
    }

    // 5. Validações especiais para atribuição de permissões
    if (context.Request.Method == "POST" && path == "/api/users")
    {
        var body = await ReadBodyAsync<CreateUserDto>(context.Request);

        // Não-root não pode atribuir root (1) ou resources (3)
        if (!isRootUser && body.PermissionsIds.Any(p => p == 1 || p == 3))
        {
            context.Response.StatusCode = 403;
            await context.Response.WriteAsync("Cannot assign root or resources permission");
            return;
        }
    }

    await _next(context);
}
```

### Mapeamento de Endpoints

**Arquivo:** `Api/Helpers/EndpointPermissions.cs:1`

```csharp
public static readonly Dictionary<string, int> Map = new()
{
    { "/api/users", 2 },        // Requer permissão "users"
    { "/api/resources", 3 },    // Requer permissão "resources"
    { "/api/reports", 4 }       // Requer permissão "reports"
};
```

### Validações em Services

**Exemplo:** `UpdateUser.ExecuteAsync()`

```csharp
// 1. Não pode editar usuário root se não for root
if (userToUpdate.Id == 1 && authUserId != 1)
{
    throw new UnauthorizedException("Cannot edit root user");
}

// 2. Valida atribuição de permissões
if (dto.PermissionsIds != null)
{
    var isRootUser = authUserId == 1;
    var hasRestrictedPermissions = dto.PermissionsIds.Any(p => p == 1 || p == 3);

    if (!isRootUser && hasRestrictedPermissions)
    {
        throw new ForbiddenException("Cannot assign root or resources permission");
    }
}
```

## Implementação Frontend

### Mapa de Permissões

**Arquivo:** `WebApp/src/permissions/PermissionsMap.ts:1`

```typescript
export const PermissionsMap = {
  ROOT: "root",
  USERS: "users",
  RESOURCES: "resources",
  REPORTS: "reports",
  PROFILE: "profile",
};
```

### Regras de Permissão

**Arquivo:** `WebApp/src/permissions/Rules.ts:1`

```typescript
// Verifica se é usuário root
export const isRootUser = (user: User | null): boolean => {
  return user?.id === 1;
};

// Verifica se tem permissão específica
export const hasPermission = (
  user: User | null,
  permission: string,
): boolean => {
  if (!user) return false;
  if (isRootUser(user)) return true; // Root tem todas
  return user.permissions.some((p) => p.name === permission);
};

// Pode editar senha de outro usuário?
export const canEditPassword = (authUser: User, targetUser: User): boolean => {
  return isRootUser(authUser) || authUser.id === targetUser.id;
};

// Pode editar permissões?
export const canEditPermissions = (
  authUser: User,
  targetUser: User,
): boolean => {
  return isRootUser(authUser) && targetUser.id !== 1;
};

// Filtra permissões que podem ser atribuídas
export const filterAssignablePermissions = (
  authUser: User,
  allPermissions: SystemResource[],
): SystemResource[] => {
  if (isRootUser(authUser)) {
    return allPermissions;
  }
  // Remove root e resources para não-root
  return allPermissions.filter(
    (p) => p.name !== "root" && p.name !== "resources",
  );
};
```

### ProtectedRoute

**Arquivo:** `WebApp/src/routes/ProtectedRoute.tsx:1`

```typescript
interface Props {
  children: ReactNode;
  requiredPermission: string | null;
}

export const ProtectedRoute = ({ children, requiredPermission }: Props) => {
  const { token, authUser } = useAuth();

  // Não autenticado
  if (!token) {
    return <Navigate to="/login" />;
  }

  // Sem permissão requerida (rota pública para autenticados)
  if (!requiredPermission) {
    return <>{children}</>;
  }

  // Verifica permissão
  if (!hasPermission(authUser, requiredPermission)) {
    return <Navigate to="/unauthorized" />;
  }

  return <>{children}</>;
};
```

### Visibilidade do Menu

**Arquivo:** `WebApp/src/permissions/MenuVisibility.ts:1`

```typescript
interface MenuItem {
  path: string;
  label: string;
  icon: any;
  permission: string | null;
}

export const filterMenuByPermissions = (
  menuItems: MenuItem[],
  user: User | null,
): MenuItem[] => {
  return menuItems.filter((item) => {
    if (!item.permission) return true; // Sem restrição
    return hasPermission(user, item.permission);
  });
};
```

**Uso no SidePanel:**

```typescript
const menuItems: MenuItem[] = [
  { path: "/profile", label: "Perfil", icon: PersonIcon, permission: null },
  { path: "/users", label: "Usuários", icon: PeopleIcon, permission: "users" },
  {
    path: "/resources",
    label: "Recursos",
    icon: FolderIcon,
    permission: "resources",
  },
  {
    path: "/reports",
    label: "Relatórios",
    icon: AssessmentIcon,
    permission: "reports",
  },
];

const visibleItems = filterMenuByPermissions(menuItems, authUser);
```

### Controle de UI Condicional

**Exemplo:** Formulário de Usuário

```typescript
const UserForm = ({ user }: Props) => {
  const { authUser } = useAuth();
  const [availablePermissions, setAvailablePermissions] = useState([]);

  useEffect(() => {
    // Busca todos os recursos
    systemResourcesServices.listSystemResourcesForSelect()
      .then(resources => {
        // Filtra baseado no usuário autenticado
        const filtered = filterAssignablePermissions(authUser, resources);
        setAvailablePermissions(filtered);
      });
  }, [authUser]);

  const canEditPasswordField = canEditPassword(authUser, user);
  const canEditPermissionsField = canEditPermissions(authUser, user);

  return (
    <Form>
      <TextField name="username" />
      <TextField name="email" />

      {canEditPasswordField && (
        <TextField name="password" type="password" />
      )}

      {canEditPermissionsField && (
        <SystemResourcesSelect
          options={availablePermissions}
          value={selectedPermissions}
          onChange={setSelectedPermissions}
        />
      )}
    </Form>
  );
};
```

## Exemplos Práticos

### Cenário 1: Usuário Root

**Usuário:** root (ID: 1)
**Permissões:** root, users, resources, reports

**Pode fazer:**

- ✓ Acessar todas as páginas
- ✓ Criar/editar/deletar qualquer usuário
- ✓ Atribuir qualquer permissão
- ✓ Gerenciar recursos do sistema
- ✓ Ver relatórios

**Não pode fazer:**

- ✗ Nada é restrito

---

### Cenário 2: Gerente de Usuários

**Usuário:** alice (ID: 5)
**Permissões:** users, reports

**Pode fazer:**

- ✓ Acessar /users
- ✓ Acessar /reports
- ✓ Criar usuários com permissões users e reports
- ✓ Editar usuários (exceto root)
- ✓ Deletar usuários (exceto root)
- ✓ Visualizar logs

**Não pode fazer:**

- ✗ Acessar /resources
- ✗ Atribuir permissão root
- ✗ Atribuir permissão resources
- ✗ Editar usuário root
- ✗ Editar senha de outros usuários

---

### Cenário 3: Auditor

**Usuário:** bob (ID: 6)
**Permissões:** reports

**Pode fazer:**

- ✓ Acessar /reports
- ✓ Filtrar logs por usuário/ação/data
- ✓ Editar próprio perfil e senha

**Não pode fazer:**

- ✗ Acessar /users
- ✗ Acessar /resources
- ✗ Criar/editar/deletar usuários
- ✗ Gerenciar recursos

---

### Cenário 4: Usuário Sem Permissões

**Usuário:** charlie (ID: 7)
**Permissões:** (nenhuma)

**Pode fazer:**

- ✓ Fazer login
- ✓ Acessar /profile
- ✓ Editar próprio perfil e senha
- ✓ Fazer logout

**Não pode fazer:**

- ✗ Acessar qualquer outra página
- ✗ Ver menu lateral (vazio)
- ✗ Qualquer operação CRUD

---

## Fluxo de Validação

### Request: POST /api/users

**Usuário:** alice (ID: 5, permissões: users, reports)

**Payload:**

```json
{
  "username": "john",
  "email": "john@example.com",
  "password": "password123",
  "fullName": "John Doe",
  "permissionsIds": [2, 4] // users, reports
}
```

**Backend - Middleware:**

```
1. Extrai token JWT
2. Decodifica: userId=5, permissions="2,4"
3. Verifica endpoint /api/users → requer permissão 2 (users)
4. Valida: permissions.includes(2) → TRUE
5. Valida payload: permissionsIds não contém 1 ou 3 → OK
6. Permite acesso
```

**Backend - Service:**

```
1. Valida email único
2. Valida username único
3. Hasheia senha
4. Cria usuário
5. Cria access_permissions para IDs 2 e 4
6. Registra log
7. Retorna usuário criado
```

**Frontend:**

```
1. UserForm filtra permissões disponíveis
2. Exibe apenas: users, reports (remove root, resources)
3. Envia request
4. Recebe sucesso
5. Atualiza lista de usuários
6. Exibe notificação
```

---

## Segurança

### Proteções Implementadas

1. **Double Validation**
   - Backend: Middleware + Service validation
   - Frontend: UI controls + form validation

2. **Least Privilege**
   - Usuários só recebem permissões necessárias
   - Root é único por design

3. **Privilege Escalation Prevention**
   - Não-root não pode criar root
   - Não-root não pode atribuir permissions management

4. **Audit Trail**
   - Todas as ações são registradas
   - Logs imutáveis (append-only)

5. **Token-Based Auth**
   - JWT com expiração
   - Claims incluem permissões
   - Validação em cada request

### Recomendações Adicionais

Para produção, considere:

1. **Rate Limiting**
   - Limite de tentativas de login
   - Throttling de API calls

2. **Two-Factor Authentication (2FA)**
   - Para usuários root
   - Para operações sensíveis

3. **IP Whitelisting**
   - Para acesso root
   - Para operações críticas

4. **Session Management**
   - Timeout de inatividade
   - Revogação de tokens

5. **Audit Enhancements**
   - IP address logging
   - User agent logging
   - Retention policies

## Próximos Passos

- [Guia de Uso](./07-GUIA-DE-USO.md)
- [Desenvolvimento](./08-DESENVOLVIMENTO.md)
