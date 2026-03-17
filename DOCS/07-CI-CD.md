# CI/CD - Integração e Entrega Contínua

Documentação completa dos pipelines de CI/CD configurados com GitHub Actions.

## Índice

1. [Visão Geral](#visão-geral)
2. [Arquivos de Workflow](#arquivos-de-workflow)
3. [Workflow de Desenvolvimento (build-test-pr.yml)](#workflow-de-desenvolvimento)
4. [Workflow Principal (build-and-test.yml)](#workflow-principal)
5. [Semantic Release](#semantic-release)
6. [Docker Hub](#docker-hub)
7. [Deploy Automático](#deploy-automático)
8. [Secrets Necessários](#secrets-necessários)
9. [Fluxo de Trabalho Recomendado](#fluxo-de-trabalho-recomendado)

---

## Visão Geral

O projeto utiliza **GitHub Actions** para automatizar o processo de build, teste, versionamento e deploy. Existem dois workflows principais:

```
┌─────────────────────────────────────────────────────────────────────────┐
│                        FLUXO DE CI/CD                                   │
├─────────────────────────────────────────────────────────────────────────┤
│                                                                         │
│  development branch          staging branch           main branch       │
│        │                          │                        │            │
│        ▼                          ▼                        ▼            │
│  ┌──────────┐              ┌──────────┐              ┌──────────┐       │
│  │  Build   │              │  Build   │              │  Build   │       │
│  │  Test    │              │  Test    │              │  Test    │       │
│  └──────────┘              │ Release  │              │ Release  │       │
│                            │  Docker  │              │  Docker  │       │
│                            │  Deploy  │              │  Deploy  │       │
│                            └──────────┘              └──────────┘       │
│                                  │                        │             │
│                                  ▼                        ▼             │
│                             Staging                  Production         │
│                             Server                    Server            │
│                                                                         │
└─────────────────────────────────────────────────────────────────────────┘
```

### Resumo dos Workflows

| Workflow             | Trigger                      | Jobs                                 | Propósito           |
| -------------------- | ---------------------------- | ------------------------------------ | ------------------- |
| `build-test-pr.yml`  | Push em `development/*`, PRs | build, test                          | Validação de código |
| `build-and-test.yml` | Push em `main`, `staging`    | build, test, release, docker, deploy | Pipeline completo   |

---

## Arquivos de Workflow

```
.github/
└── workflows/
    ├── build-test-pr.yml      # CI para desenvolvimento e PRs
    └── build-and-test.yml     # CI/CD completo para staging/produção
```

### Variáveis de Ambiente Globais

Ambos os workflows definem variáveis de ambiente no início:

```yaml
env:
  DOCKER_IMAGE: docker-username/app-name # Imagem Docker no Docker Hub
  APP_NAME: app-name # Nome da aplicação (deploy)
  DIR: ./Api # Diretório do backend
```

---

## Workflow de Desenvolvimento

**Arquivo:** `.github/workflows/build-test-pr.yml`

### Quando é Executado

```yaml
on:
  push:
    branches: [development, development/*] # Push em development ou sub-branches
  pull_request:
    branches: [main, staging, development] # Abertura de PRs para essas branches
```

Este workflow é executado:

- Quando há push na branch `development`
- Quando há push em branches que começam com `development/` (ex: `development/feature-x`)
- Quando um Pull Request é aberto para `main`, `staging` ou `development`

### Jobs

#### Job 1: Build

```yaml
build:
  runs-on: ubuntu-latest
  steps:
    - name: Checkout
      uses: actions/checkout@v4

    - name: Setup .NET
      uses: actions/setup-dotnet@v4
      with:
        dotnet-version: "8.0.x"

    - name: Cache NuGet packages
      uses: actions/cache@v3
      with:
        path: ~/.nuget/packages
        key: ${{ runner.os }}-nuget-${{ hashFiles('**/*.csproj') }}
        restore-keys: |
          ${{ runner.os }}-nuget-

    - name: Restore dependencies
      run: dotnet restore
      working-directory: ${{ env.DIR }}

    - name: Build
      run: dotnet build --no-restore
      working-directory: ${{ env.DIR }}
```

**Explicação passo a passo:**

| Passo | Ação                      | Descrição                                          |
| ----- | ------------------------- | -------------------------------------------------- |
| 1     | `actions/checkout@v4`     | Clona o repositório no runner                      |
| 2     | `actions/setup-dotnet@v4` | Instala o .NET SDK 8.0.x                           |
| 3     | `actions/cache@v3`        | Cacheia pacotes NuGet para acelerar builds futuros |
| 4     | `dotnet restore`          | Restaura as dependências do projeto                |
| 5     | `dotnet build`            | Compila o projeto (modo Debug)                     |

#### Job 2: Test

```yaml
test:
  runs-on: ubuntu-latest
  needs: build # Depende do job build
  steps:
    - name: Checkout
      uses: actions/checkout@v4

    - name: Setup .NET
      uses: actions/setup-dotnet@v4
      with:
        dotnet-version: 8.0.x

    - name: Run Tests
      run: dotnet test
      working-directory: ${{ env.DIR }}
```

**Explicação passo a passo:**

| Passo | Ação                      | Descrição                                    |
| ----- | ------------------------- | -------------------------------------------- |
| 1     | `actions/checkout@v4`     | Clona o repositório novamente (job separado) |
| 2     | `actions/setup-dotnet@v4` | Instala o .NET SDK                           |
| 3     | `dotnet test`             | Executa os testes unitários                  |

**Dependência:** Este job só executa após o `build` ser bem-sucedido (`needs: build`).

---

## Workflow Principal

**Arquivo:** `.github/workflows/build-and-test.yml`

### Quando é Executado

```yaml
on:
  push:
    branches: [main, staging] # Push em main ou staging
  workflow_dispatch: # Execução manual via GitHub UI
```

Este workflow é executado:

- Quando há push na branch `main` (produção)
- Quando há push na branch `staging`
- Manualmente através do botão "Run workflow" no GitHub

### Jobs

#### Job 1: Build

Idêntico ao workflow de desenvolvimento, porém com **modo Release**:

```yaml
- name: Build
  run: dotnet build --no-restore --configuration Release
  working-directory: ${{ env.DIR }}
```

A flag `--configuration Release` otimiza o código para produção.

#### Job 2: Test

```yaml
test:
  runs-on: ubuntu-latest
  needs: build
  steps:
    - name: Checkout
      uses: actions/checkout@v4

    - name: Setup .NET
      uses: actions/setup-dotnet@v4
      with:
        dotnet-version: 8.0.x

    - name: Run Tests
      run: dotnet test --configuration Release --logger "trx;LogFileName=test-results.trx"
      working-directory: ${{ env.DIR }}

    - name: Upload Test Results
      if: always()
      uses: actions/upload-artifact@v6
      with:
        name: test-results
        path: ${{ env.DIR }}/**/*.trx
```

**Diferenças do workflow de desenvolvimento:**

| Aspecto      | Desenvolvimento | Principal                        |
| ------------ | --------------- | -------------------------------- |
| Configuração | Debug           | Release                          |
| Logger       | Nenhum          | TRX (Visual Studio Test Results) |
| Artefatos    | Não             | Sim (upload dos resultados)      |

**Explicação do Upload de Artefatos:**

- `if: always()` - Executa mesmo se os testes falharem
- Salva os resultados em formato `.trx` como artefato
- Pode ser baixado posteriormente para análise

#### Job 3: Release (Semantic Release)

```yaml
release:
  runs-on: ubuntu-latest
  permissions:
    contents: write # Permissão para criar releases
  needs: test
  if: success()
  steps:
    - name: Checkout Code
      uses: actions/checkout@v4
      with:
        fetch-depth: 0 # Clona todo o histórico (necessário para análise de commits)

    - name: Setup Node.js
      uses: actions/setup-node@v4
      with:
        node-version: "24.13.0"

    - name: Install Dependencies
      run: npm install

    - name: Run Semantic Release
      run: npx semantic-release
      env:
        GITHUB_TOKEN: ${{ secrets.TOKEN_GITHUB }}
```

**Explicação passo a passo:**

| Passo | Ação                    | Descrição                                       |
| ----- | ----------------------- | ----------------------------------------------- |
| 1     | `actions/checkout@v4`   | Clona com histórico completo (`fetch-depth: 0`) |
| 2     | `actions/setup-node@v4` | Instala Node.js 24.13.0                         |
| 3     | `npm install`           | Instala dependências (semantic-release)         |
| 4     | `npx semantic-release`  | Executa o versionamento automático              |

**O que o Semantic Release faz:**

1. Analisa os commits desde a última release
2. Determina o tipo de versão (major, minor, patch)
3. Gera o CHANGELOG automaticamente
4. Cria uma tag Git
5. Publica uma release no GitHub

#### Job 4: Push to Docker

**IMPORTANTE** -- é necessário criar o repositório no _DockerHub_.

1. Acesse: [Docker Hub](https://hub.docker.com).
2. Acesse a sua conta.
3. Clique em `My Hub`.
4. Vá em Repositories.
5. Clique em Create a Repository.
6. Dê o nome da aplicação -- Ex. app-velorio -- Adicione uma descrição breve.
7. Deixe em Público.
8. Clique em Create.

```yaml
push-to-docker:
  runs-on: ubuntu-latest
  needs: test
  if: success()
  steps:
    - name: Checkout
      uses: actions/checkout@v4

    - name: Set up Docker Buildx
      uses: docker/setup-buildx-action@v3

    - name: Log in to Docker Hub
      uses: docker/login-action@v3
      with:
        username: ${{ secrets.DOCKERHUB_USERNAME }}
        password: ${{ secrets.DOCKERHUB_TOKEN }}

    - name: Determine image tags
      id: tags
      run: |
        TAGS="${{ env.DOCKER_IMAGE }}:${{ github.sha }}"

        if [[ "${{ github.ref_name }}" == "main" ]]; then
          TAGS="$TAGS,${{ env.DOCKER_IMAGE }}:latest,${{ env.DOCKER_IMAGE }}:main"
        elif [[ "${{ github.ref_name }}" == "staging" ]]; then
          TAGS="$TAGS,${{ env.DOCKER_IMAGE }}:staging"
        elif [[ "${{ github.ref_name }}" == "development" ]]; then
          TAGS="$TAGS,${{ env.DOCKER_IMAGE }}:dev"
        fi

        echo "tags=$TAGS" >> $GITHUB_OUTPUT

    - name: Build and Push
      uses: docker/build-push-action@v5
      with:
        context: ${{ env.DIR }}
        push: true
        tags: ${{ steps.tags.outputs.tags }}
        cache-from: type=gha
        cache-to: type=gha,mode=max
        build-args: |
          BUILDKIT_INLINE_CACHE=1
```

**Explicação passo a passo:**

| Passo | Ação                            | Descrição                                     |
| ----- | ------------------------------- | --------------------------------------------- |
| 1     | `actions/checkout@v4`           | Clona o repositório                           |
| 2     | `docker/setup-buildx-action@v3` | Configura Docker Buildx para builds avançados |
| 3     | `docker/login-action@v3`        | Autentica no Docker Hub                       |
| 4     | Script de tags                  | Determina as tags da imagem baseado na branch |
| 5     | `docker/build-push-action@v5`   | Constrói e publica a imagem                   |

**Tags geradas por branch:**

| Branch        | Tags                    |
| ------------- | ----------------------- |
| `main`        | `sha`, `latest`, `main` |
| `staging`     | `sha`, `staging`        |
| `development` | `sha`, `dev`            |

**Otimizações de cache:**

- `cache-from: type=gha` - Usa cache do GitHub Actions
- `cache-to: type=gha,mode=max` - Salva cache máximo
- `BUILDKIT_INLINE_CACHE=1` - Habilita cache inline no Dockerfile

#### Job 5: Deploy Staging

```yaml
deploy-staging:
  runs-on: ubuntu-latest
  needs: push-to-docker
  timeout-minutes: 10
  if: |
    github.ref_name == 'staging' && github.event_name == 'push'
  environment: staging
  steps:
    - name: Deploy to Staging (Webhook)
      uses: fjogeleit/http-request-action@v1
      with:
        url: ${{ secrets.STAGING_DEPLOY_URL }}:9000/hooks/deploy
        method: POST
        timeout: 30000
        headers: |
          Content-Type: application/json
          X-Hub-Signature: ${{ secrets.STAGING_DEPLOY_SECRET }}
        body: |
          {
            "app": "${{ env.APP_NAME }}",
            "env": "staging",
            "version": "${{ github.sha }}"
          }
```

**Condições de execução:**

- Apenas quando a branch é `staging`
- Apenas em eventos de `push` (não em `workflow_dispatch`)
- Após o job `push-to-docker` ser bem-sucedido

**Como funciona:**

1. Envia uma requisição HTTP POST para o webhook do servidor de staging
2. O webhook recebe os dados da aplicação e versão
3. O servidor de staging faz pull da nova imagem e reinicia o container

**Payload enviado:**

```json
{
  "app": "app-name",
  "env": "staging",
  "version": "abc123def456..."
}
```

#### Job 6: Deploy Production

```yaml
deploy-production:
  runs-on: ubuntu-latest
  needs: push-to-docker
  timeout-minutes: 10
  if: |
    github.ref_name == 'main' && github.event_name == 'push'
  environment: production
  steps:
    - name: Deploy to Production (Webhook)
      uses: fjogeleit/http-request-action@v1
      with:
        url: ${{ secrets.PRODUCTION_DEPLOY_URL }}:9000/hooks/deploy
        method: POST
        timeout: 30000
        headers: |
          Content-Type: application/json
          X-Hub-Signature: ${{ secrets.PRODUCTION_DEPLOY_SECRET }}
        body: |
          {
            "app": "${{ env.APP_NAME }}",
            "env": "production",
            "version": "${{ github.sha }}"
          }
```

Idêntico ao deploy de staging, porém:

- Executa apenas na branch `main`
- Usa secrets de produção
- Envia para o ambiente `production`

---

## Semantic Release

**Arquivo:** `.releaserc`

```json
{
  "branches": [
    {
      "name": "development",
      "prerelease": "dev"
    },
    {
      "name": "staging",
      "prerelease": "staging"
    },
    {
      "name": "main"
    }
  ],
  "plugins": [
    "@semantic-release/commit-analyzer",
    "@semantic-release/release-notes-generator",
    "@semantic-release/changelog",
    "@semantic-release/github",
    "@semantic-release/git"
  ]
}
```

### Branches Configuradas

| Branch        | Tipo                 | Exemplo de Versão |
| ------------- | -------------------- | ----------------- |
| `development` | Prerelease (dev)     | `1.0.0-dev.1`     |
| `staging`     | Prerelease (staging) | `1.0.0-staging.1` |
| `main`        | Release estável      | `1.0.0`           |

### Plugins Utilizados

| Plugin                    | Função                                         |
| ------------------------- | ---------------------------------------------- |
| `commit-analyzer`         | Analisa commits para determinar tipo de versão |
| `release-notes-generator` | Gera notas de release automaticamente          |
| `changelog`               | Atualiza o arquivo CHANGELOG.md                |
| `github`                  | Cria release no GitHub                         |
| `git`                     | Commita mudanças (CHANGELOG, package.json)     |

### Convenção de Commits

O Semantic Release usa [Conventional Commits](https://www.conventionalcommits.org/):

| Prefixo                        | Tipo de Versão | Exemplo                                  |
| ------------------------------ | -------------- | ---------------------------------------- |
| `feat:`                        | Minor (0.X.0)  | `feat: adiciona autenticação OAuth`      |
| `fix:`                         | Patch (0.0.X)  | `fix: corrige validação de email`        |
| `feat!:` ou `BREAKING CHANGE:` | Major (X.0.0)  | `feat!: remove endpoint deprecated`      |
| `docs:`                        | Nenhum         | `docs: atualiza README`                  |
| `chore:`                       | Nenhum         | `chore: atualiza dependências`           |
| `refactor:`                    | Nenhum         | `refactor: simplifica AuthService`       |
| `test:`                        | Nenhum         | `test: adiciona testes para UserService` |

---

## Docker Hub

### Imagem

- **Repositório:** `docker-username/docker-repo-name`
- **URL:** https://hub.docker.com/r/docker-username/docker-repo-name

### Tags Disponíveis

| Tag       | Descrição             | Atualização           |
| --------- | --------------------- | --------------------- |
| `latest`  | Última versão estável | Push em `main`        |
| `main`    | Branch main           | Push em `main`        |
| `staging` | Branch staging        | Push em `staging`     |
| `dev`     | Branch development    | Push em `development` |
| `<sha>`   | Commit específico     | Todo push             |

### Uso da Imagem

```bash
# Puxar última versão estável
docker pull docker-username/app-name:latest

# Puxar versão de staging
docker pull docker-username/app-name:staging

# Puxar commit específico
docker pull docker-username/app-name:abc123def456
```

---

## Deploy Automático

### Arquitetura de Deploy

```
┌──────────────┐     Push      ┌──────────────┐
│   GitHub     │ ────────────► │   GitHub     │
│   Repository │               │   Actions    │
└──────────────┘               └──────┬───────┘
                                      │
                    ┌─────────────────┴─────────────────┐
                    │                                   │
                    ▼                                   ▼
            ┌──────────────┐                   ┌──────────────┐
            │  Docker Hub  │                   │   Webhook    │
            │  (imagem)    │                   │   POST       │
            └──────────────┘                   └──────┬───────┘
                                                      │
                    ┌─────────────────────────────────┘
                    │
                    ▼
            ┌──────────────┐
            │   Servidor   │
            │  (webhook    │
            │   listener)  │
            └──────┬───────┘
                   │
                   ▼
            ┌──────────────┐
            │  docker pull │
            │  docker up   │
            └──────────────┘
```

### Configuração do Servidor

O servidor precisa ter um webhook listener (como [webhook](https://github.com/adnanh/webhook)) configurado na porta 9000:

```bash
# Exemplo de configuração webhook
[
        {
                "id": "deploy",
                "execute-command": "/home/seu-user/hooks/deploy.sh",
                "command-working-directory": "/home/seu-user/repos",
                "pass-arguments-to-command": [
                        {
                                "source": "payload",
                                "name": "app"
                        },
                        {
                                "source": "payload",
                                "name": "env"
                        }
                ],
                "secret": "seu-secret-aqui"
        },
]
```

### Configuração do script de deploy do Webhook

Com o listener funcionando, é necessário um script para rodar o deploy, segue o exemplo de deploy.sh

```bash
# Exemplo de configuração webhook no ambiente de produção
# Para abiente de staging, altere p $ENV == "main" para $ENV == "staging"
# E troque o nome do docker-compose.production.yml pull para docker-compose.staging.yml
#!/usr/bin/env bash
set -e

APP_NAME="$1"
ENV="$2"

BASE_DIR="/home/dtisistemas/repos"

if [[ -z "$APP_NAME" || -z "$ENV" ]]; then
  echo "Parâmetros inválidos"
  exit 1
fi

APP_DIR="$BASE_DIR/$APP_NAME"

if [[ ! -d "$APP_DIR" ]]; then
  echo "App não existe: $APP_DIR"
  exit 1
fi

cd "$APP_DIR"

if [[ "$ENV" == "main" ]]; then
  docker compose -f docker-compose.production.yml pull
  docker compose -f docker-compose.production.yml -p $APP_NAME  up -d
else
  echo "Ambiente inválido"
  exit 1
fi

docker image prune -f --filter "label=app=$APP_NAME"
```

# Pull da nova imagem

docker pull docker-username/app-name:$VERSION

# Atualiza o compose

docker compose -f docker-compose.$ENV.yml up -d --no-deps api

echo "Deploy de $APP versão $VERSION no ambiente $ENV concluído"

---

## Secrets Necessários

Configure os seguintes secrets no GitHub (Settings > Secrets and variables > Actions):

### Obrigatórios

| Secret               | Descrição                              | Exemplo            |
| -------------------- | -------------------------------------- | ------------------ |
| `TOKEN_GITHUB`       | Token GitHub com permissões de escrita | `ghp_xxxxxxxxxxxx` |
| `DOCKERHUB_USERNAME` | Usuário do Docker Hub                  | `docker-username`  |
| `DOCKERHUB_TOKEN`    | Token de acesso do Docker Hub          | `dckr_pat_xxxxx`   |

### Para Deploy de Staging

| Secret                  | Descrição                           | Exemplo                       |
| ----------------------- | ----------------------------------- | ----------------------------- |
| `STAGING_DEPLOY_URL`    | URL do servidor de staging          | `https://staging.example.com` |
| `STAGING_DEPLOY_SECRET` | Secret para autenticação do webhook | `staging-secret-key`          |

### Para Deploy de Produção

| Secret                     | Descrição                           | Exemplo                   |
| -------------------------- | ----------------------------------- | ------------------------- |
| `PRODUCTION_DEPLOY_URL`    | URL do servidor de produção         | `https://app.example.com` |
| `PRODUCTION_DEPLOY_SECRET` | Secret para autenticação do webhook | `production-secret-key`   |

### Como Criar os Secrets

1. Acesse o repositório no GitHub
2. Vá em **Settings** > **Secrets and variables** > **Actions**
3. Clique em **New repository secret**
4. Adicione nome e valor
5. Clique em **Add secret**

---

## Fluxo de Trabalho Recomendado

### 1. Desenvolvimento de Features

```bash
# Crie uma branch a partir de development
git checkout development
git pull origin development
git checkout -b development/desenvolvedor/minha-feature

# exemplo
git checkout -b development/murilo/minha-feature

# Desenvolva e faça commits
git add .
git commit -m "feat: adiciona nova funcionalidade X"

# Push para executar CI
git push origin development/minha-feature
```

O workflow `build-test-pr.yml` será executado automaticamente.

### 2. Pull Request para Development

```bash
# Abra um PR no GitHub
# development/minha-feature -> development
```

O workflow `build-test-pr.yml` será executado para validar o PR.

### 3. Merge para Development

Após aprovação do PR, faça o merge. O workflow de desenvolvimento será executado.

### 4. Promoção para Staging

```bash
# Crie PR de development para staging
# development -> staging
```

Após merge, o workflow completo é executado:

1. Build e Test
2. Semantic Release (cria versão `-staging.X`)
3. Push para Docker Hub
4. Deploy automático em staging

### 5. Promoção para Produção

```bash
# Crie PR de staging para main
# staging -> main
```

Após merge, o workflow completo é executado:

1. Build e Test
2. Semantic Release (cria versão estável)
3. Push para Docker Hub (tag `latest`)
4. Deploy automático em produção

---

## Diagrama de Execução dos Jobs

```
build-test-pr.yml (development):
┌───────┐     ┌──────┐
│ build │ ──► │ test │
└───────┘     └──────┘

build-and-test.yml (main/staging):
┌───────┐     ┌──────┐     ┌─────────┐     ┌────────────────┐
│ build │ ──► │ test │ ──► │ release │     │ push-to-docker │
└───────┘     └──────┘     └─────────┘     └───────┬────────┘
                                │                   │
                                │   (paralelo)      │
                                ▼                   ▼
                                              ┌─────────────────┐
                                              │ deploy-staging  │ (se staging)
                                              │       ou        │
                                              │ deploy-prod     │ (se main)
                                              └─────────────────┘
```

---

## Troubleshooting

### Build Falhou

1. Verifique os logs do job `build`
2. Confirme que `dotnet restore` executou sem erros
3. Verifique se há erros de compilação

### Testes Falharam

1. Baixe o artefato `test-results` (arquivo `.trx`)
2. Abra no Visual Studio ou VS Code para ver detalhes
3. Corrija os testes que falharam

### Release Não Foi Criada

1. Verifique se os commits seguem a convenção (Conventional Commits)
2. Confirme que o `TOKEN_GITHUB` tem permissões de escrita
3. Verifique os logs do job `release`

### Docker Push Falhou

1. Verifique as credenciais do Docker Hub
2. Confirme que o token não expirou
3. Verifique se a imagem existe no Docker Hub

### Deploy Falhou

1. Verifique se o webhook está acessível
2. Confirme que o secret está correto
3. Verifique os logs do servidor de destino

---

## Próximos Passos

- [Instalação](./01-INSTALACAO.md)
- [Arquitetura](./02-ARQUITETURA.md)
- [Desenvolvimento](./08-DESENVOLVIMENTO.md)
