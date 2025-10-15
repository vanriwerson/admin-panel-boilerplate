# Admin Panel BoilerPlate

> Boilerplate de painel administrativo full-stack com **backend em .NET 8 + PostgreSQL** e **frontend em React + Vite + TypeScript**, incluindo **autenticação JWT**, **CRUD de usuários** e integração completa entre frontend e backend.

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
│   │   ├── hooks/
│   │   ├── pages/
│   │   ├── types/
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

- Backend: exposto em `http://localhost:5209`
- Frontend: exposto em `http://localhost:5173`
- PostgreSQL: exposto em `localhost:5432`

### 4. Aplicar migrations no banco (caso use container para backend)

```bash
cd Api
dotnet ef database update
```

> Isso criará as tabelas iniciais no PostgreSQL.

---

## Rodando Localmente sem Docker (opcional)

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

## Endpoints da API

- `/api/auth/login` → Login JWT
- `/api/auth/external` → Login via token externo
- `/api/users` → CRUD de usuários

> Para documentação completa, abra Swagger em: `http://localhost:<API_PORT>/swagger/`

---

## Frontend

- `/login` → Login JWT
- `/dashboard` → Painel de usuários com listagem, criação, edição e exclusão

> O token JWT é enviado automaticamente em todas as requisições Axios após login.

---

## Observações

- Todas as variáveis de ambiente são obrigatórias.
- Logs do backend indicam se a conexão com o banco foi bem-sucedida.
- É possível acessar tanto frontend quanto backend via browser ou ferramentas como Postman.

---

## Sobre o Desenvolvedor

[Bruno Riwerson Silva](https://www.linkedin.com/in/bruno-riwerson/) é um profissional apaixonado por tecnologia. Desenvolvedor full-stack proficiente no uso de React com MaterialUI no front-end e NodeJS com Express no back-end. Possui experiência no uso de bancos de dados relacionais e não-relacionais, além de conhecer outras tecnologias como Golang, Java, Docker, entre outras, tornando-o dinâmico e apto a solucionar quaisquer problemas de modo eficiente.
