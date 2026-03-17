# Frontend - WebApp Documentation

Documentação completa do frontend React da aplicação.

## Índice

1. [Visão Geral](#visão-geral)
2. [Estrutura de Pastas](#estrutura-de-pastas)
3. [Roteamento](#roteamento)
4. [Contextos](#contextos)
5. [Custom Hooks](#custom-hooks)
6. [Componentes](#componentes)
7. [Services](#services)
8. [Sistema de Permissões](#sistema-de-permissões)
9. [Tema e Estilos](#tema-e-estilos)
10. [Configuração](#configuração)

## Visão Geral

O frontend é uma Single Page Application (SPA) construída com React 19, TypeScript e Material-UI.

**Características:**

- React 19 com TypeScript
- Vite como build tool
- Material-UI para componentes
- React Router para navegação
- Axios para HTTP requests
- Context API para estado global
- Custom hooks para lógica de negócio
- Tema claro/escuro
- RBAC no frontend

**Porta padrão:** 5173

**URL:** `http://localhost:5173`

## Estrutura de Pastas

```
WebApp/src/
├── api/                      # Configuração Axios
│   └── index.ts
├── assets/                   # Imagens e ícones
├── components/               # Componentes reutilizáveis
│   ├── ActionsSelect/
│   ├── AuthUserDisplay/
│   ├── ConfirmDialog/
│   ├── LoginForm/
│   ├── NoResultsFound/
│   ├── PageTitle/
│   ├── ReportsTable/
│   ├── SidePanel/
│   ├── SnackbarNotification/
│   ├── StatsCard/
│   ├── SystemResourceEditionModal/
│   ├── SystemResourceForm/
│   ├── SystemResourcesSelect/
│   ├── SystemResourcesTable/
│   ├── UserEditionModal/
│   ├── UserForm/
│   ├── UsersSelect/
│   ├── UsersTable/
│   └── ...
├── contexts/                 # Estado global
│   ├── AuthContext.tsx
│   ├── NotificationContext.tsx
│   ├── PermissionsContext.tsx
│   ├── SystemResourcesContext.tsx
│   ├── ThemeContext.tsx
│   └── UsersContext.tsx
├── helpers/                  # Utilitários
├── hooks/                    # Custom hooks
│   ├── useAuth.ts
│   ├── useNotification.ts
│   ├── usePermissions.ts
│   ├── useReports.ts
│   ├── useSystemResources.ts
│   ├── useThemeMode.ts
│   └── useUsers.ts
├── interfaces/               # TypeScript types
│   ├── User.ts
│   ├── SystemResource.ts
│   └── SystemLog.ts
├── layouts/                  # Layouts
│   ├── DefaultLayout.tsx
│   └── CleanLayout.tsx
├── mappers/                  # Mapeamento de dados
├── pages/                    # Páginas
│   ├── Login.tsx
│   ├── PasswordReset.tsx
│   ├── Profile.tsx
│   ├── Users.tsx
│   ├── Resources.tsx
│   └── Reports.tsx
├── permissions/              # RBAC
│   ├── PermissionsMap.ts
│   ├── Rules.ts
│   └── MenuVisibility.ts
├── routes/                   # Rotas
│   ├── index.tsx
│   └── ProtectedRoute.tsx
├── services/                 # API calls
│   ├── authServices.ts
│   ├── usersServices/
│   ├── systemResourcesServices/
│   └── systemLogsServices.ts
├── App.tsx                   # Componente raiz
├── main.tsx                  # Entry point
└── theme.ts                  # Tema MUI
```

## Roteamento

### Arquivo Principal

**Arquivo:** `WebApp/src/routes/index.tsx:1`

### Estrutura de Rotas

```typescript
<BrowserRouter>
  <Routes>
    {/* Rotas Públicas */}
    <Route element={<CleanLayout />}>
      <Route path="/" element={<Navigate to="/login" />} />
      <Route path="/login" element={<Login />} />
      <Route path="/password-reset" element={<PasswordReset />} />
      <Route path="/unauthorized" element={<Unauthorized />} />
    </Route>

    {/* Rotas Protegidas */}
    <Route element={<DefaultLayout />}>
      <Route
        path="/profile"
        element={
          <ProtectedRoute requiredPermission={null}>
            <Profile />
          </ProtectedRoute>
        }
      />
      <Route
        path="/users"
        element={
          <ProtectedRoute requiredPermission="users">
            <Users />
          </ProtectedRoute>
        }
      />
      <Route
        path="/resources"
        element={
          <ProtectedRoute requiredPermission="resources">
            <Resources />
          </ProtectedRoute>
        }
      />
      <Route
        path="/reports"
        element={
          <ProtectedRoute requiredPermission="reports">
            <Reports />
          </ProtectedRoute>
        }
      />
    </Route>
  </Routes>
</BrowserRouter>
```

### ProtectedRoute

**Arquivo:** `WebApp/src/routes/ProtectedRoute.tsx:1`

**Responsabilidades:**

- Verificar autenticação
- Verificar permissões
- Redirecionar se não autorizado

```typescript
interface Props {
  children: ReactNode;
  requiredPermission: string | null;
}

export const ProtectedRoute = ({ children, requiredPermission }: Props) => {
  const { token, authUser } = useAuth();

  if (!token) {
    return <Navigate to="/login" />;
  }

  if (requiredPermission && !hasPermission(authUser, requiredPermission)) {
    return <Navigate to="/unauthorized" />;
  }

  return <>{children}</>;
};
```

## Contextos

### AuthContext

**Arquivo:** `WebApp/src/contexts/AuthContext.tsx:1`

**Estado:**

```typescript
interface AuthContextType {
  token: string | null;
  authUser: User | null;
  handleLogin: (credentials: LoginDto) => Promise<void>;
  handleExternalLogin: (token: string) => Promise<void>;
  handlePasswordResetRequest: (email: string) => Promise<void>;
  handlePasswordReset: (token: string, password: string) => Promise<void>;
  handleLogout: () => void;
}
```

**Funcionalidades:**

- Armazena token e dados do usuário no localStorage
- Persiste sessão entre reloads
- Métodos de autenticação completos

**Uso:**

```typescript
const { token, authUser, handleLogin, handleLogout } = useAuth();
```

### ThemeContext

**Arquivo:** `WebApp/src/contexts/ThemeContext.tsx:1`

**Estado:**

```typescript
interface ThemeContextType {
  mode: "light" | "dark";
  toggleTheme: () => void;
}
```

**Funcionalidades:**

- Gerencia tema claro/escuro
- Persiste preferência no localStorage

### NotificationContext

**Arquivo:** `WebApp/src/contexts/NotificationContext.tsx:1`

**Estado:**

```typescript
interface NotificationContextType {
  showSuccess: (message: string) => void;
  showError: (message: string) => void;
  showInfo: (message: string) => void;
  showWarning: (message: string) => void;
}
```

**Funcionalidades:**

- Exibe notificações toast
- Integração com Snackbar do Material-UI

### PermissionsContext

**Arquivo:** `WebApp/src/contexts/PermissionsContext.tsx:1`

**Estado:**

```typescript
interface PermissionsContextType {
  userPermissions: string[];
  hasPermission: (permission: string) => boolean;
  isRootUser: boolean;
}
```

**Funcionalidades:**

- Gerencia permissões do usuário atual
- Métodos utilitários para validação de RBAC

### SystemResourcesContext

**Arquivo:** `WebApp/src/contexts/SystemResourcesContext.tsx:1`

**Estado:**

```typescript
interface SystemResourcesContextType {
  systemResources: SystemResource[];
  loading: boolean;
  fetchSystemResources: () => Promise<void>;
}
```

**Funcionalidades:**

- Cache de recursos do sistema
- Evita chamadas repetidas à API

### UsersContext

**Arquivo:** `WebApp/src/contexts/UsersContext.tsx:1`

**Estado:**

```typescript
interface UsersContextType {
  users: User[];
  loading: boolean;
  fetchUsers: () => Promise<void>;
}
```

**Funcionalidades:**

- Cache de usuários
- Estado compartilhado entre componentes
- Integra com Material-UI ThemeProvider

**Uso:**

```typescript
const { mode, toggleTheme } = useThemeMode();
```

## Custom Hooks

### useAuth

**Arquivo:** `WebApp/src/hooks/useAuth.ts:1`

Acessa o AuthContext:

```typescript
export const useAuth = () => {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error("useAuth must be used within AuthProvider");
  }
  return context;
};
```

### useUsers

**Arquivo:** `WebApp/src/hooks/useUsers.ts:1`

**Estado e Métodos:**

```typescript
interface UseUsersReturn {
  users: User[];
  totalUsers: number;
  loading: boolean;
  error: string | null;
  fetchUsers: (page: number, limit: number) => Promise<void>;
  fetchUserById: (id: number) => Promise<User>;
  searchUsers: (key: string, page: number, limit: number) => Promise<void>;
  createUser: (data: CreateUserDto) => Promise<void>;
  updateUser: (id: number, data: UpdateUserDto) => Promise<void>;
  deleteUser: (id: number) => Promise<void>;
}
```

**Características:**

- Gerencia estado de usuários
- Paginação integrada
- Loading e error states
- Operações CRUD completas

**Uso:**

```typescript
const { users, loading, fetchUsers, createUser } = useUsers();

useEffect(() => {
  fetchUsers(1, 10);
}, []);
```

### useSystemResources

**Arquivo:** `WebApp/src/hooks/useSystemResources.ts:1`

Estrutura similar ao useUsers para recursos do sistema.

### useReports

**Arquivo:** `WebApp/src/hooks/useReports.ts:1`

**Estado e Métodos:**

```typescript
interface UseReportsReturn {
  logs: SystemLog[];
  totalLogs: number;
  loading: boolean;
  fetchLogs: (filters: LogFilters) => Promise<void>;
}

interface LogFilters {
  page: number;
  limit: number;
  userId?: number;
  action?: string;
  startDate?: string;
  endDate?: string;
}
```

### useNotification

**Arquivo:** `WebApp/src/hooks/useNotification.ts:1`

Acessa o NotificationContext para exibir notificações:

```typescript
export const useNotification = () => {
  const context = useContext(NotificationContext);
  if (!context) {
    throw new Error("useNotification must be used within NotificationProvider");
  }
  return context;
};
```

### usePermissions

**Arquivo:** `WebApp/src/hooks/usePermissions.ts:1`

Acessa o PermissionsContext para validações de RBAC:

```typescript
export const usePermissions = () => {
  const context = useContext(PermissionsContext);
  if (!context) {
    throw new Error("usePermissions must be used within PermissionsProvider");
  }
  return context;
};
```

## Componentes

### Componentes de Formulário

#### LoginForm

**Arquivo:** `WebApp/src/components/LoginForm/index.tsx:1`

**Props:** Nenhuma (usa AuthContext)

**Campos:**

- Identifier (email ou username)
- Password

**Funcionalidades:**

- Validação de campos
- Loading state
- Tratamento de erros
- Link para redefinição de senha

#### UserForm

**Arquivo:** `WebApp/src/components/UserForm/index.tsx:1`

**Props:**

```typescript
interface Props {
  initialData?: User | null;
  onSubmit: (data: CreateUserDto | UpdateUserDto) => Promise<void>;
  isEditing?: boolean;
}
```

**Campos:**

- Username
- Email
- Password (opcional na edição)
- Full Name
- Permissions (multi-select)

**Validações:**

- Email válido
- Senha mínima 6 caracteres
- Campos obrigatórios

#### SystemResourceForm

**Arquivo:** `WebApp/src/components/SystemResourceForm/index.tsx:1`

Similar ao UserForm, para recursos do sistema.

### Componentes de Tabela

#### UsersTable

**Arquivo:** `WebApp/src/components/UsersTable/index.tsx:1`

**Props:**

```typescript
interface Props {
  users: User[];
  onEdit: (user: User) => void;
  onDelete: (id: number) => void;
}
```

**Colunas:**

- ID
- Username
- Email
- Full Name
- Permissions (badges)
- Created At
- Actions (edit, delete)

**Funcionalidades:**

- Ordenação por coluna
- Ações condicionais baseadas em permissões
- Confirmação de exclusão

#### SystemResourcesTable

Similar ao UsersTable para recursos.

#### ReportsTable

**Arquivo:** `WebApp/src/components/ReportsTable/index.tsx:1`

**Colunas:**

- ID
- User
- Action
- Created At

**Funcionalidades:**

- Filtros (usuário, ação, data)
- Paginação
- Exportação (futuro)

### Componentes de Select

#### UsersSelect

**Arquivo:** `WebApp/src/components/UsersSelect/index.tsx:1`

**Props:**

```typescript
interface Props {
  value: number | null;
  onChange: (userId: number | null) => void;
  label?: string;
}
```

Carrega lista de usuários do endpoint `/api/users/options`.

#### SystemResourcesSelect

Similar ao UsersSelect para recursos.

### Componentes de Layout

#### SidePanel

**Arquivo:** `WebApp/src/components/SidePanel/index.tsx:1`

**Responsabilidades:**

- Menu lateral navegável
- Exibe apenas itens com permissão (via MenuVisibility)
- Destaque do item ativo
- Botão de logout

**Itens do Menu:**

```typescript
const menuItems = [
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
```

#### AuthUserDisplay

**Arquivo:** `WebApp/src/components/AuthUserDisplay/index.tsx:1`

Exibe dados do usuário autenticado no topo do layout.

## Services

### authServices

**Arquivo:** `WebApp/src/services/authServices.ts:1`

```typescript
export const authServices = {
  login: async (credentials: LoginDto) => {
    const response = await api.post("/auth/login", credentials);
    return response.data;
  },

  externalLogin: async (token: string) => {
    const response = await api.post("/auth/external", { token });
    return response.data;
  },

  requestPasswordReset: async (email: string) => {
    const response = await api.post("/auth/password/request-new", { email });
    return response.data;
  },

  resetPassword: async (token: string, newPassword: string) => {
    const response = await api.post("/auth/password/reset", {
      token,
      newPassword,
    });
    return response.data;
  },
};
```

### usersServices

**Pasta:** `WebApp/src/services/usersServices/`

```typescript
export const usersServices = {
  listUsers: async (page: number, limit: number) => {
    const response = await api.get("/users", { params: { page, limit } });
    return response.data;
  },

  listUserById: async (id: number) => {
    const response = await api.get(`/users/${id}`);
    return response.data;
  },

  listUsersForSelect: async () => {
    const response = await api.get("/users/options");
    return response.data;
  },

  createUser: async (data: CreateUserDto) => {
    const response = await api.post("/users", data);
    return response.data;
  },

  updateUser: async (id: number, data: UpdateUserDto) => {
    const response = await api.put(`/users/${id}`, data);
    return response.data;
  },

  deleteUser: async (id: number) => {
    await api.delete(`/users/${id}`);
  },
};
```

### systemResourcesServices

Similar ao usersServices para recursos.

### systemLogsServices

**Arquivo:** `WebApp/src/services/systemLogsServices.ts:1`

```typescript
export const systemLogsServices = {
  listSystemLogs: async (filters: LogFilters) => {
    const response = await api.get("/reports", { params: filters });
    return response.data;
  },
};
```

## Sistema de Permissões

### PermissionsMap

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

### Rules

**Arquivo:** `WebApp/src/permissions/Rules.ts:1`

```typescript
export const isRootUser = (user: User | null): boolean => {
  return user?.id === 1;
};

export const hasPermission = (
  user: User | null,
  permission: string,
): boolean => {
  if (!user) return false;
  if (isRootUser(user)) return true;
  return user.permissions.some((p) => p.name === permission);
};

export const canEditPassword = (authUser: User, targetUser: User): boolean => {
  return isRootUser(authUser) || authUser.id === targetUser.id;
};

export const canEditPermissions = (
  authUser: User,
  targetUser: User,
): boolean => {
  return isRootUser(authUser) && targetUser.id !== 1;
};

export const filterAssignablePermissions = (
  authUser: User,
  allPermissions: SystemResource[],
): SystemResource[] => {
  if (isRootUser(authUser)) {
    return allPermissions;
  }
  // Usuários não-root não podem atribuir root e resources
  return allPermissions.filter(
    (p) => p.name !== "root" && p.name !== "resources",
  );
};
```

### MenuVisibility

**Arquivo:** `WebApp/src/permissions/MenuVisibility.ts:1`

```typescript
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

## Tema e Estilos

### theme.ts

**Arquivo:** `WebApp/src/theme.ts:1`

```typescript
export const getTheme = (mode: "light" | "dark") =>
  createTheme({
    palette: {
      mode,
      primary: {
        main: mode === "light" ? "#198a0fff" : "#b6f990ff",
      },
      background: {
        default: mode === "light" ? "#f5f5f5" : "#121212",
        paper: mode === "light" ? "#ffffff" : "#1e1e1e",
      },
    },
    typography: {
      fontFamily: "Roboto, Arial, sans-serif",
    },
  });
```

**Cores:**

**Modo Claro:**

- Primary: `#198a0fff` (verde)
- Background: `#f5f5f5` / `#fff`

**Modo Escuro:**

- Primary: `#b6f990ff` (verde claro)
- Background: `#121212` / `#1e1e1e`

## Configuração

### Axios Instance

**Arquivo:** `WebApp/src/api/index.ts:1`

```typescript
const api = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL,
});

// Request Interceptor: Adiciona token JWT
api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem("token");
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => Promise.reject(error),
);

// Response Interceptor: Redireciona em 401
api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      localStorage.removeItem("token");
      localStorage.removeItem("authUser");
      window.location.href = "/login";
    }
    return Promise.reject(error);
  },
);
```

### Variáveis de Ambiente

**Arquivo:** `WebApp/.env.example:1`

```env
VITE_API_BASE_URL=http://localhost:{PORT}/api
```

### Vite Config

**Arquivo:** `WebApp/vite.config.ts:1`

```typescript
export default defineConfig({
  plugins: [react()],
  server: {
    port: 5173,
    host: true,
  },
});
```

## Layouts

### DefaultLayout

**Arquivo:** `WebApp/src/layouts/DefaultLayout.tsx:1`

**Estrutura:**

- SidePanel (menu lateral)
- Toggle para abrir/fechar menu
- Toggle de tema (light/dark)
- AuthUserDisplay
- Outlet para rotas filhas

**Usado em:** Rotas autenticadas

### CleanLayout

**Arquivo:** `WebApp/src/layouts/CleanLayout.tsx:1`

**Estrutura:**

- Layout minimalista
- Sem menu lateral
- Acompanha tema
- Outlet para rotas filhas

**Usado em:** Login, Redefinição de senha, Unauthorized

## TypeScript Interfaces

### User

```typescript
export interface User {
  id: number;
  username: string;
  email: string;
  fullName: string;
  active: boolean;
  createdAt: string;
  updatedAt: string;
  permissions: SystemResource[];
}
```

### SystemResource

```typescript
export interface SystemResource {
  id: number;
  name: string;
  exhibitionName: string;
  active: boolean;
  createdAt: string;
  updatedAt: string;
}
```

### SystemLog

```typescript
export interface SystemLog {
  id: number;
  userId: number;
  userName: string;
  action: string;
  createdAt: string;
}
```

## Dependências

**Arquivo:** `WebApp/package.json:1`

```json
{
  "dependencies": {
    "@emotion/react": "^11.14.0",
    "@emotion/styled": "^11.14.1",
    "@fortawesome/fontawesome-svg-core": "^7.1.0",
    "@fortawesome/free-solid-svg-icons": "^7.1.0",
    "@fortawesome/react-fontawesome": "^3.1.0",
    "@mui/icons-material": "^7.3.4",
    "@mui/material": "^7.3.4",
    "@mui/x-date-pickers": "^8.16.0",
    "@mui/x-date-pickers-pro": "^8.16.0",
    "axios": "^1.12.2",
    "date-fns": "^4.1.0",
    "react": "^19.1.1",
    "react-dom": "^19.1.1",
    "react-router-dom": "^7.9.4"
  },
  "devDependencies": {
    "@eslint/js": "^9.36.0",
    "@types/node": "^24.6.0",
    "@types/react": "^19.1.16",
    "@types/react-dom": "^19.1.9",
    "@vitejs/plugin-react": "^5.0.4",
    "eslint": "^9.36.0",
    "eslint-plugin-react-hooks": "^5.2.0",
    "eslint-plugin-react-refresh": "^0.4.22",
    "globals": "^16.4.0",
    "typescript": "~5.9.3",
    "typescript-eslint": "^8.45.0",
    "vite": "^7.1.7"
  }
}
```

## Próximos Passos

- [API Reference](./05-API-REFERENCE.md)
- [Sistema de Permissões](./06-PERMISSOES.md)
- [Guia de Uso](./07-GUIA-DE-USO.md)
