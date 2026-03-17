# Instalação e Configuração

Este guia detalha como instalar e configurar o Admin Panel Boilerplate tanto com Docker quanto sem Docker.

## Índice

1. [Pré-requisitos](#pré-requisitos)
2. [Instalação com Docker (Recomendado)](#instalação-com-docker-recomendado)
3. [Instalação sem Docker](#instalação-sem-docker)
4. [Configuração de Variáveis de Ambiente](#configuração-de-variáveis-de-ambiente)
5. [Verificação da Instalação](#verificação-da-instalação)
6. [Problemas Comuns](#problemas-comuns)

## Pré-requisitos

Antes de começar a trabalhar neste projeto, certifique-se de ter instaladas as ferramentas abaixo (ou equivalentes) no seu ambiente:

- **.NET SDK** (recomendado: 8.x) - necessário para compilar e rodar o backend.

  ```bash
    # Exemplo (Ubuntu):
    sudo apt update && sudo apt install -y dotnet-sdk-8.0
    # Windows/Mac: baixe em https://dotnet.microsoft.com/download
  ```

- **Node.js** (recomendado: 18.x ou superior) - necessário para rodar o frontend.

  ```bash
    # Exemplo (nvm):
    nvm install 18
    nvm use 18
    # Alternativamente: https://nodejs.org/
  ```

- **Docker** (opcional, mas recomendado) - usado para rodar o banco de dados e outros serviços em containers.

  ```bash
    # Exemplo (Ubuntu):
    sudo apt update && sudo apt install -y docker.io
    # Verifique com: docker --version
  ```

- **Docker Compose** (pode vir incluso com Docker Desktop / Docker Engine como `docker compose`) - usado para orquestrar os serviços usados pelo template.

  ```bash
    # Verifique instalação:
    docker compose version
    # Se necessário (Ubuntu):
    sudo apt install -y docker-compose
  ```

- **dotnet-ef** (Entity Framework CLI) - para executar migrations e gerenciar o banco de dados:
  ```bash
    dotnet tool install --global dotnet-ef
    dotnet ef --version
  ```

### Para instalação com Docker:

- [Docker](https://www.docker.com/get-started) (versão 20.10 ou superior)
- [Docker Compose](https://docs.docker.com/compose/install/) (versão 2.x - integrado ao Docker)
- Git

### Para instalação sem Docker:

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js](https://nodejs.org/) (versão 18 ou superior)
- [PostgreSQL](https://www.postgresql.org/download/) (versão 14 ou superior)
- [npm](https://www.npmjs.com/) ou [yarn](https://yarnpkg.com/)
- Git

## Instalação com Docker (Recomendado)

### Passo 1: Clone o Repositório

```bash
git clone <url-do-repositorio>
cd admin-panel-boilerplate
```

### Passo 2: Configure as Variáveis de Ambiente

#### Backend (Api/.env)

```bash
cp Api/.env.example Api/.env
```

Edite o arquivo `Api/.env`:

```env
# Configuração do Banco de Dados
DB_HOST=db
DB_PORT=5432
DB_USER=postgres
DB_PASSWORD=postgres
DB_NAME=admin_panel_db

# Configuração para Docker (PostgreSQL container)
POSTGRES_USER=postgres
POSTGRES_PASSWORD=postgres
POSTGRES_DB=admin_panel_db

# Seed do Banco (cria dados iniciais)
RUN_USERS_SEED=true

# Porta da API (defina sua própria porta)
# Para projetos DTI PMA, siga a sequência 521x (5210, 5211, 5212...)
API_PORT={PORT}

# Chave secreta para JWT (ALTERE PARA PRODUÇÃO!)
JWT_SECRET_KEY=sua-chave-secreta-muito-segura-aqui-com-pelo-menos-32-caracteres

# URL do Frontend (para CORS)
WEB_APP_URL=http://localhost:5173

# Configuração de Email (Resend)
RESEND_API_KEY=sua-api-key-do-resend
RESEND_FROM_EMAIL=seu-email@dominio.com
```

#### Frontend (WebApp/.env)

```bash
cp WebApp/.env.example WebApp/.env
```

Edite o arquivo `WebApp/.env`:

```env
VITE_API_BASE_URL=http://localhost:{PORT}/api
```

### Passo 3: Inicie os Containers

```bash
# Inicia todos os serviços em background (desenvolvimento)
docker compose -f docker-compose.development.yml up -d

# Para ver os logs
docker compose -f docker-compose.development.yml logs -f

# Para ver logs de um serviço específico
docker compose -f docker-compose.development.yml logs -f api
docker compose -f docker-compose.development.yml logs -f webapp
```

### Passo 4: Aguarde a Inicialização

O primeiro build pode levar alguns minutos. Os containers serão iniciados na seguinte ordem:

1. **db** (PostgreSQL)
2. **api** (Backend .NET)
3. **webapp** (Frontend React)

### Passo 5: Acesse a Aplicação

- **Frontend**: http://localhost:5173
- **Backend API**: http://localhost:{PORT}
- **Swagger (API Docs)**: http://localhost:{PORT}/swagger

### Comandos Úteis do Docker

```bash
# Parar os containers
docker compose -f docker-compose.development.yml stop

# Parar e remover os containers
docker compose -f docker-compose.development.yml down

# Parar, remover containers e volumes (limpa o banco de dados)
docker compose -f docker-compose.development.yml down -v

# Reconstruir as imagens
docker compose -f docker-compose.development.yml build

# Reiniciar um serviço específico
docker compose -f docker-compose.development.yml restart api

# Ver status dos containers
docker compose -f docker-compose.development.yml ps

# Acessar o terminal de um container
docker compose -f docker-compose.development.yml exec api bash
docker compose -f docker-compose.development.yml exec webapp sh
```

## Instalação sem Docker

### Passo 1: Clone o Repositório

```bash
git clone <url-do-repositorio>
cd admin-panel-boilerplate
```

### Passo 2: Configure o Banco de Dados PostgreSQL

Crie um banco de dados PostgreSQL:

```bash
# Entre no PostgreSQL
psql -U postgres

# Crie o banco de dados
CREATE DATABASE admin_panel_db;

# Crie um usuário (opcional)
CREATE USER admin WITH PASSWORD 'admin123';
GRANT ALL PRIVILEGES ON DATABASE admin_panel_db TO admin;

# Saia do psql
\q
```

### Passo 3: Configure o Backend

#### Variáveis de Ambiente

```bash
cd Api
cp .env.example .env
```

Edite o arquivo `Api/.env`:

```env
# Configuração do Banco de Dados
DB_HOST=localhost
DB_PORT=5432
DB_USER=postgres
DB_PASSWORD=postgres
DB_NAME=admin_panel_db

# Seed do Banco
RUN_USERS_SEED=true

# Porta da API (defina sua própria porta)
# Para projetos DTI PMA, siga a sequência 521x (5210, 5211, 5212...)
API_PORT={PORT}

# Chave secreta para JWT
JWT_SECRET_KEY=sua-chave-secreta-muito-segura-aqui-com-pelo-menos-32-caracteres

# URL do Frontend
WEB_APP_URL=http://localhost:5173

# Configuração de Email
RESEND_API_KEY=sua-api-key-do-resend
RESEND_FROM_EMAIL=seu-email@dominio.com
```

#### Restaurar Dependências e Executar

```bash
# Restaurar pacotes NuGet
dotnet restore

# Aplicar migrations
dotnet ef database update

# Executar a API
dotnet run
```

A API estará disponível em `http://localhost:{PORT}`

### Passo 4: Configure o Frontend

Em outro terminal:

```bash
cd WebApp
cp .env.example .env
```

Edite o arquivo `WebApp/.env`:

```env
VITE_API_BASE_URL=http://localhost:{PORT}/api
```

#### Instalar Dependências e Executar

```bash
# Instalar dependências
npm install
# ou
yarn install

# Executar em modo desenvolvimento
npm run dev
# ou
yarn dev
```

O frontend estará disponível em `http://localhost:5173`

### Passo 5: Verificar Entity Framework (Opcional)

Se você precisar criar novas migrations ou atualizar o banco:

```bash
cd Api

# Criar uma nova migration
dotnet ef migrations add NomeDaMigration

# Aplicar migrations pendentes
dotnet ef database update

# Reverter última migration
dotnet ef database update NomeDaMigrationAnterior

# Remover última migration (se não foi aplicada)
dotnet ef migrations remove
```

## Configuração de Variáveis de Ambiente

### Backend (Api/.env)

| Variável            | Descrição                    | Obrigatório | Padrão    |
| ------------------- | ---------------------------- | ----------- | --------- |
| `DB_HOST`           | Host do PostgreSQL           | Sim         | localhost |
| `DB_PORT`           | Porta do PostgreSQL          | Sim         | 5432      |
| `DB_USER`           | Usuário do banco             | Sim         | -         |
| `DB_PASSWORD`       | Senha do banco               | Sim         | -         |
| `DB_NAME`           | Nome do banco de dados       | Sim         | -         |
| `POSTGRES_USER`     | Usuário PostgreSQL (Docker)  | Não\*       | -         |
| `POSTGRES_PASSWORD` | Senha PostgreSQL (Docker)    | Não\*       | -         |
| `POSTGRES_DB`       | Database PostgreSQL (Docker) | Não\*       | -         |
| `RUN_USERS_SEED`    | Executar seed inicial        | Não         | false     |
| `API_PORT`          | Porta da API\*\*\*           | Sim         | -         |
| `JWT_SECRET_KEY`    | Chave secreta para JWT       | Sim         | -         |
| `WEB_APP_URL`       | URL do frontend (CORS)       | Sim         | -         |
| `RESEND_API_KEY`    | API Key do Resend            | Não\*\*     | -         |
| `RESEND_FROM_EMAIL` | Email remetente              | Não\*\*     | -         |

\*Obrigatório apenas ao usar Docker Compose (para criação do container PostgreSQL).
\*\*Obrigatório apenas se for usar o recurso de redefinição de senha por email.
\*\*\*Defina sua própria porta. Para projetos DTI PMA, siga a convenção da sequência **521x** (5210, 5211, 5212...).

### Frontend (WebApp/.env)

| Variável            | Descrição       | Obrigatório | Padrão                      |
| ------------------- | --------------- | ----------- | --------------------------- |
| `VITE_API_BASE_URL` | URL base da API | Sim         | http://localhost:{PORT}/api |

## Verificação da Instalação

### 1. Verifique se os serviços estão rodando

**Com Docker:**

```bash
docker-compose ps
```

Todos os serviços devem estar com status "Up".

**Sem Docker:**

- Verifique se a API responde: `curl http://localhost:{PORT}/api/auth/login`
- Acesse o frontend: http://localhost:5173

### 2. Verifique o banco de dados

```bash
# Com Docker
docker-compose exec db psql -U admin -d admin_panel_db -c "SELECT * FROM users;"

# Sem Docker
psql -U admin -d admin_panel_db -c "SELECT * FROM users;"
```

Você deve ver o usuário `root` e possivelmente usuários de teste.

### 3. Teste o login

Acesse http://localhost:5173 e faça login com:

- **Usuário**: `root`
- **Senha**: `root1234`

Se conseguir acessar o painel, a instalação foi bem-sucedida!

### 4. Verifique o Swagger

Acesse http://localhost:{PORT}/swagger para ver a documentação interativa da API.

## Problemas Comuns

### Erro de conexão com o banco de dados

**Problema**: `Npgsql.NpgsqlException: Connection refused`

**Solução**:

- Verifique se o PostgreSQL está rodando
- Verifique as credenciais no arquivo `.env`
- Se usando Docker, aguarde o container `db` inicializar completamente

### Porta já em uso

**Problema**: `Error starting userland proxy: listen tcp4 0.0.0.0:{PORT}: bind: address already in use`

**Solução**:

```bash
# Descubra qual processo está usando a porta
lsof -i :{PORT}

# Mate o processo
kill -9 <PID>

# Ou altere a porta no Api/.env (API_PORT)
```

### Migrations não aplicadas

**Problema**: Erro ao iniciar a API sobre tabelas não existentes

**Solução**:

```bash
cd Api
dotnet ef database update
```

### CORS Error no Frontend

**Problema**: `Access to XMLHttpRequest blocked by CORS policy`

**Solução**:

- Verifique se `WEB_APP_URL` no `Api/.env` está correto
- Certifique-se que a URL inclui a porta (ex: `http://localhost:5173`)

### Variáveis de ambiente não carregadas

**Problema**: API usa valores padrão ao invés do `.env`

**Solução**:

- Verifique se o arquivo `.env` está na raiz da pasta `Api/`
- Certifique-se que não há espaços extras nas variáveis
- Reinicie a aplicação

### Email não enviado

**Problema**: Erro ao solicitar redefinição de senha

**Solução**:

- Verifique se `RESEND_API_KEY` está configurado corretamente
- Confirme que o email em `RESEND_FROM_EMAIL` está verificado no Resend
- Veja os logs da API para detalhes do erro

### Frontend não conecta com a API

**Problema**: Requests falham ou timeout

**Solução**:

- Verifique se `VITE_API_BASE_URL` no `WebApp/.env` está correto
- Certifique-se que a API está rodando: `curl http://localhost:{PORT}`
- Limpe o cache do navegador e reinicie o dev server

### Docker build falha

**Problema**: Erro durante `docker-compose build`

**Solução**:

```bash
# Limpe o cache do Docker
docker system prune -a

# Reconstrua sem cache
docker-compose build --no-cache
```

## Próximos Passos

Após a instalação bem-sucedida:

1. Leia a [Arquitetura do Sistema](./02-ARQUITETURA.md)
2. Explore a [API Reference](./05-API-REFERENCE.md)
3. Consulte o [Guia de Uso](./07-GUIA-DE-USO.md)
4. Entenda o [Sistema de Permissões](./06-PERMISSOES.md)

## Configuração de Produção

Para ambientes de produção, considere:

1. **Segurança**:
   - Altere `JWT_SECRET_KEY` para uma chave forte e única
   - Use senhas fortes para o banco de dados
   - Configure HTTPS/TLS
   - Altere a senha do usuário root

2. **Performance**:
   - Configure connection pooling no PostgreSQL
   - Implemente caching (Redis)
   - Use um CDN para assets estáticos

3. **Monitoramento**:
   - Configure logs estruturados
   - Implemente health checks
   - Configure alertas de erro

4. **Backup**:
   - Configure backups automáticos do PostgreSQL
   - Implemente disaster recovery

Consulte o [Guia de Desenvolvimento](./08-DESENVOLVIMENTO.md) para mais informações.
