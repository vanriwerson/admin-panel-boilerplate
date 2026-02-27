# Admin Panel BoilerPlate

> Modelo de painel administrativo full-stack com **backend em .NET 8 + PostgreSQL** e
> **frontend em React + Vite + TypeScript**, incluindo **autenticação JWT**, **CRUD de usuários**,
> **CRUD de recursos do sistema**, **controle de permissões RBAC**, **proteção de rotas** e
> **auditoria de sistema** com integração completa entre frontend e backend.

---

## Tecnologias Utilizadas

- **Backend**
  - [.NET 8](https://learn.microsoft.com/en-us/dotnet/core/introduction)
  - [Entity Framework Core 9](https://learn.microsoft.com/en-us/ef/core/)
  - [PostgreSQL 14+](https://www.postgresql.org/)
  - [BCrypt.Net-Next 4.0](https://www.nuget.org/packages/BCrypt.Net-Next/)
  - [JWT (JSON Web Token)](https://jwt.io/introduction)
  - [Swagger/OpenAPI](https://swagger.io/docs/)
  - [Resend](https://resend.com/) (serviço de e-mail)

- **Frontend**
  - [React 19](https://react.dev/)
  - [Vite 7](https://vitejs.dev/)
  - [TypeScript 5.9](https://www.typescriptlang.org/)
  - [Material-UI (MUI) 7](https://mui.com/)
  - [Axios](https://axios-http.com/)
  - [React Router 7](https://reactrouter.com/)
  - [date-fns](https://date-fns.org/) (utilitários de data)

- **DevOps & CI/CD**
  - [Docker & Docker Compose](https://docs.docker.com/compose/)
  - [GitHub Actions](https://github.com/features/actions) (CI/CD)
  - [Semantic Release](https://semantic-release.gitbook.io/) (versionamento automático)
  - Containers para banco de dados, backend e frontend

---

## Estrutura do Projeto

```
admin-panel-boilerplate/
│
├── Api/ # Backend .NET
│   ├── Controllers/
│   ├── Data/
│   ├── Dtos/
│   ├── Helpers/
│   ├── Middlewares/
│   ├── Models/
│   ├── Services/
│   ├── Program.cs
│   └── .env
│
├── WebApp/ # Frontend React + Vite
│   ├── public/
│   ├── src/
│   │   ├── api/
│   │   ├── components/
│   │   ├── contexts/
│   │   ├── helpers/
│   │   ├── hooks/
│   │   ├── interfaces/
│   │   ├── pages/
│   │   ├── permissions/
│   │   ├── routes/
│   │   ├── App.tsx
│   │   └── main.tsx
│   ├── tsconfig.json
│   ├── package.json
│   └── .env
│
├── docker-compose.development.yml   # Desenvolvimento local
├── docker-compose.staging.yml       # Ambiente de staging
└── docker-compose.production.yml    # Ambiente de produção
```

---

## Pré-requisitos

- [Docker](https://docs.docker.com/get-docker/)
- [Docker Compose](https://docs.docker.com/compose/install/)
- [Node.js](https://nodejs.org/en/) (para rodar frontend localmente, opcional se usar via container)
- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) (para rodar backend localmente, opcional se usar via container)

---

## Rodando o Projeto via Docker Compose

### 1. Clonar o repositório

```bash
git clone git@github.com:github-user/repo-name.git
cd app-name
```

### 2. Criar arquivos `.env`

```bash
# Backend
cp Api/.env.example Api/.env

# Frontend
cp WebApp/.env.example WebApp/.env
```

Edite o arquivo `Api/.env` com suas configurações:

```env
# Database
DB_HOST=db
DB_PORT=5432
DB_USER=postgres
DB_PASSWORD=postgres
DB_NAME=admin_panel_db

POSTGRES_USER=postgres
POSTGRES_PASSWORD=postgres
POSTGRES_DB=admin_panel_db

# Seeds
RUN_USERS_SEED=true

# Application
API_PORT={PORT}

# JWT (gere uma chave segura)
JWT_SECRET_KEY=sua-chave-secreta-com-pelo-menos-32-caracteres

# CORS
WEB_APP_URL=http://localhost:5173

# Email (opcional)
RESEND_API_KEY=sua-api-key
RESEND_FROM_EMAIL=seu-email@dominio.com
```

> Gere uma chave JWT segura:

```bash
openssl rand -base64 64
```

#### Configuração de Porta da API

O usuário deve definir a porta da API através da variável `API_PORT` no arquivo `.env`. Na documentação, onde você encontrar `{PORT}`, substitua pela porta que você configurou.

> **Para projetos DTI PMA**: Siga a convenção de portas da sequência **521x** (ex: {PORT}, 5211, 5212...). Verifique qual é a próxima porta disponível antes de configurar seu projeto para evitar conflitos.

### 3. Subir os containers (Desenvolvimento)

```bash
docker compose -f docker-compose.development.yml up -d
```

- PostgreSQL: exposto em `localhost:5432`
- Backend: exposto em `http://localhost:{PORT}`
- Frontend: exposto em `http://localhost:5173`
- Swagger: `http://localhost:{PORT}/swagger`

### 4. Credenciais padrão

- **Usuário:** `root`
- **Senha:** `root1234`

> As migrations são aplicadas automaticamente ao iniciar o container da API em modo desenvolvimento.

---

## Rodando Localmente sem Docker (opcional)

### Banco de dados

Configure sua conexão PostgreSQL localmente ou suba somente o banco de dados via docker com:

```bash
docker compose -f docker-compose.development.yml up db -d
```

### Backend

```bash
cd Api
cp .env.example .env  # Configure suas variáveis de ambiente
dotnet restore
dotnet ef database update
dotnet run
```

A API estará disponível em `http://localhost:{PORT}`

### Frontend

```bash
cd WebApp
cp .env.example .env  # Configure VITE_API_BASE_URL=http://localhost:{PORT}/api
npm install
npm run dev
```

O frontend estará disponível em `http://localhost:5173`

---

## CI/CD

O projeto utiliza GitHub Actions para CI/CD com os seguintes workflows:

- **build-test-pr.yml**: Executado em push para `development` e PRs - build e testes
- **build-and-test.yml**: Executado em push para `main` e `staging` - build, testes, release e deploy

### Branches e Versionamento

O projeto usa [Semantic Release](https://semantic-release.gitbook.io/) para versionamento automático:

| Branch        | Ambiente        | Tag                |
| ------------- | --------------- | ------------------ |
| `main`        | Produção        | `v1.0.0`           |
| `staging`     | Staging         | `v1.0.0-staging.1` |
| `development` | Desenvolvimento | `v1.0.0-dev.1`     |

### Convenção de Commits

Use [Conventional Commits](https://www.conventionalcommits.org/) para commits semânticos:

- `feat:` Nova funcionalidade (minor version)
- `fix:` Correção de bug (patch version)
- `docs:` Documentação
- `refactor:` Refatoração
- `test:` Testes
- `chore:` Tarefas de manutenção

---

## Documentação detalhada

> Você pode encontrar informações mais completas na pasta [DOCS](./DOCS/):

- [Quick Start](./DOCS/QUICK-START.md) - Guia rápido de 5 minutos
- [Instalação](./DOCS/01-INSTALACAO.md) - Instalação completa
- [Arquitetura](./DOCS/02-ARQUITETURA.md) - Arquitetura do sistema
- [Backend](./DOCS/03-BACKEND.md) - Documentação da API
- [Frontend](./DOCS/04-FRONTEND.md) - Documentação do WebApp
- [API Reference](./DOCS/05-API-REFERENCE.md) - Referência de endpoints
- [Permissões](./DOCS/06-PERMISSOES.md) - Sistema RBAC
- [Guia de Uso](./DOCS/07-GUIA-DE-USO.md) - Tutorial para usuários
- [Desenvolvimento](./DOCS/08-DESENVOLVIMENTO.md) - Guia para desenvolvedores
- [CI/CD](./DOCS/09-CI-CD.md) - Pipeline de integração e entrega contínua

---

## Observações

- Todas as variáveis de ambiente são obrigatórias.
- Logs de inicialização da api indicam se a conexão com o banco foi bem-sucedida.

---

## Guia de Desenvolvimento e Evolução do Sistema

Este projeto segue padrões bem definidos para facilitar a manutenção e adição de novos recursos. Abaixo, um guia passo-a-passo para adicionar novos endpoints à API e integrá-los na interface web.

### Adicionando Novos Recursos à API (.NET)

1. **Definir a Entidade (Model)**:
   - Crie uma classe em `Api/Models/` representando a entidade do banco.
   - Use anotações `[Table("nome_tabela")]` e `[Key]` para mapeamento EF Core.

2. **Criar DTOs**:
   - Em `Api/Dtos/`, crie DTOs para Create, Update e Read (ex.: `EntityCreateDto`, `EntityUpdateDto`, `EntityReadDto`).
   - Use validações com `[Required]`, `[MaxLength]`, etc.

3. **Configurar Entity Framework**:
   - Em `Api/Data/Configurations/`, crie `EntityConfiguration.cs` para definir constraints, índices e relacionamentos.
   - Registre no `ApiDbContext.cs`.

4. **Criar Migration**:

   ```bash
   cd Api
   dotnet ef migrations add NomeDaMigration
   dotnet ef database update
   ```

5. **Implementar Serviço**:
   - Em `Api/Services/EntityServices/`, crie classes como `CreateEntity.cs`, `GetAllEntities.cs`, etc.
   - Use injeção do `IGenericRepository<Entity>` para operações CRUD.

6. **Criar Controller**:
   - Em `Api/Controllers/`, crie `EntityController.cs` com endpoints RESTful.
   - Use `[HttpGet]`, `[HttpPost]`, etc., e retorne IActionResult padronizado.
   - Aplique middlewares de autorização se necessário.

7. **Atualizar Seeders** (opcional):
   - Em `Api/Data/DbInitializer.cs`, adicione dados iniciais se necessário.

### Integrando Novos Recursos na Interface Web (React)

1. **Definir Interfaces TypeScript**:
   - Em `WebApp/src/interfaces/`, crie tipos para a entidade e DTOs (ex.: `Entity.ts`, `EntityCreatePayload.ts`).

2. **Criar Serviço de API**:
   - Em `WebApp/src/services/`, crie funções para consumir os endpoints (ex.: `createEntity`, `getEntities`).
   - Use a instância Axios configurada em `api/index.ts`.

3. **Implementar Contexto (Context API)**:
   - Em `WebApp/src/contexts/`, crie `EntityContext.tsx` seguindo o padrão de `UsersContext.tsx`.
   - Inclua estados para lista, paginação, loading e error.
   - Forneça funções CRUD via provider.

4. **Criar Hook Personalizado**:
   - Em `WebApp/src/hooks/`, crie `useEntity.ts` que usa `useContext(EntityContext)`.

5. **Desenvolver Componentes**:
   - Em `WebApp/src/components/`, crie componentes reutilizáveis (ex.: `EntityTable.tsx`, `EntityForm.tsx`, `EntityDialog.tsx`).
   - Use hooks para estado e notificações (Snackbar).

6. **Criar Página**:
   - Em `WebApp/src/pages/`, crie `Entity/index.tsx` com layout e lógica de CRUD.
   - Use `ConfirmDialog` para exclusões e `showNotification` para feedback.

7. **Configurar Rotas**:
   - Em `WebApp/src/routes/index.tsx`, adicione a nova rota com provider e proteção de permissão.
   - Exemplo: `<EntityProvider><Entity /></EntityProvider>`

8. **Adicionar Permissões**:
   - Em `WebApp/src/permissions/`, defina novas regras RBAC se necessário.

### Padrões Seguidos

- **Backend**: Generic Repository, Dependency Injection, Middleware de Exceção, Logs Automáticos.
- **Frontend**: Context API para estado global, Hooks para abstração, Componentes Reutilizáveis, Notificações via Snackbar.
- **Segurança**: JWT, RBAC, Validações Server/Client-side.
- **UI/UX**: Material-UI, Responsividade, Acessibilidade.

Para mais detalhes, consulte os READMEs específicos da [API](./Api/README.md) e [WebApp](./WebApp/README.md).

---
