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

## Sobre o Desenvolvedor

[Bruno Riwerson Silva](https://www.linkedin.com/in/bruno-riwerson/) é um **desenvolvedor full-stack** apaixonado por tecnologia e boas práticas de engenharia de software. Proficiente no uso de **React+MaterialUI** no front-end e **NodeJS+Express** no back-end, além de conhecer outras tecnologias como `Golang`, `Java`, `Docker`, entre outras. Possui experiência no uso de bancos de dados relacionais e não-relacionais, o que o torna um profissional dinâmico e apto a criar soluções escaláveis, seguras e bem estruturadas.
