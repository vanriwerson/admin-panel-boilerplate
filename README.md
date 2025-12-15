# Admin Panel BoilerPlate

> Modelo de painel administrativo full-stack com **backend em .NET 8 + PostgreSQL** e
> **frontend em React + Vite + TypeScript**, incluindo **autenticação JWT**, **CRUD de usuários**,
> **CRUD de recursos do sistema**, **controle de permissões RBAC**, **proteção de rotas** e
> **auditoria de sistema** com integração completa entre frontend e backend.

---

## Tecnologias Utilizadas

- **Backend**

  - [.NET 8](https://learn.microsoft.com/en-us/dotnet/core/introduction)
  - [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/)
  - [PostgreSQL](https://www.postgresql.org/)
  - [BCrypt](https://www.nuget.org/packages/BCrypt.Net-Next/)
  - [JWT (JSON Web Token)](https://jwt.io/introduction)
  - [Swagger](https://swagger.io/docs/)

- **Frontend**

  - [React 18](https://reactjs.org/)
  - [Vite](https://vitejs.dev/)
  - [TypeScript](https://www.typescriptlang.org/)
  - [MaterialUI (MUI)](https://mui.com/)
  - [Axios](https://axios-http.com/)
  - [React Router](https://reactrouter.com/)

- **DevOps**
  - [Docker & Docker Compose](https://docs.docker.com/compose/)
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
└── docker-compose.yml
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
git clone git@github.com:vanriwerson/admin-panel-boilerplate.git
cd generic-login-dotnet-react
```

### 2. Criar arquivo `.env` do backend

```bash
cd Api
cp .env.example .env
```

> Gere uma chave JWT segura:

```bash
echo "JWT_SECRET_KEY=$(openssl rand -base64 64)"
```

### 3. Subir todos os containers

```bash
docker compose up -d
```

- PostgreSQL: exposto em `localhost:5432`
- Backend: exposto em `http://localhost:5209`
- Frontend: exposto em `http://localhost:5173`

### 4. Aplicar migrations no banco (caso use container para backend)

```bash
cd Api
dotnet ef database update
```

> Isso criará as tabelas iniciais no PostgreSQL, definidas pela migration InitialCreate.

---

## Rodando Localmente sem Docker (opcional)

### Banco de dados

Configure sua conexão postgre localmente ou suba somente o banco de dados via docker com:

```bash
docker compose up db
```

### Backend

```bash
cd Api
dotnet run
```

### Frontend

```bash
cd WebApp
npm install
npm run dev
```

---

## Documentação detalhada

> Você pode encontrar informações mais completas sobre a aplicação acessando a documentação específica:

- [Backend](./Api/README.md)
- [Frontend](./WebApp/README.md)

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
