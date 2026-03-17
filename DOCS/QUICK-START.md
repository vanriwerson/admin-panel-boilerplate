# Quick Start - Guia Rápido

Comece a usar o Admin Panel Boilerplate em 5 minutos!

## Instalação Rápida (Docker)

#### 1. Clone o repositório

```bash
git clone git@github.com:vanriwerson/admin-panel-boilerplate.git
cd admin-panel-boilerplate
```

# 2. Configure variáveis de ambiente

```bash
  cp Api/.env.example Api/.env
  cp WebApp/.env.example WebApp/.env
```

Edite os arquivos, adequando portas conforme necessário.

**Api/.env:**

```bash
  # Database connection
  DB_HOST=localhost
  DB_PORT=5432
  DB_USER=postgres
  DB_PASSWORD=postgres
  DB_NAME=admin_panel_db

  # Rodar serviço db utilizando Docker
  POSTGRES_USER=postgres
  POSTGRES_PASSWORD=postgres
  POSTGRES_DB=admin_panel_db

  # Roda seed de usuários para desenvolvimento. Setar como false em produção
  RUN_USERS_SEED=true

  # Application
  API_PORT=5209

  # JWT
  JWT_SECRET_KEY=XvwKtOBmcu74xUwf8iaTLhb+JNCq1F73jUkbkuNHG+U=

  # CORS
  WEB_APP_URL=http://localhost:5173

  # Redefinição de senha via e-mail
  RESEND_API_KEY=re_ChaveDeApiDoServicoResend
  RESEND_FROM_EMAIL=emailCadastradoNoResend
```

**WebApp/.env:**

```bash
  VITE_API_BASE_URL=http://localhost:API_PORT/api
```

# 4. Inicie os containers (desenvolvimento)

docker compose -f docker-compose.development.yml up -d

# 5. Aguarde a inicialização (30-60 segundos)

docker compose -f docker-compose.development.yml logs -f

```

## Acesso

- **Frontend:** http://localhost:5173
- **Backend API:** http://localhost:API_PORT
- **Swagger:** http://localhost:{PORT}/swagger

> **Nota:** Substitua `API_PORT` pelo valor definido para a variável de ambiente `API_PORT` no arquivo `Api/.env`. Para projetos DTI PMA, siga a convenção de portas da sequência **521x** (5210, 5211, 5212...).

## Credenciais Padrão

```

Usuário: root
Senha: root1234

```

## Primeiros Passos

### 1. Crie Seu Primeiro Usuário

```

1. Acesse "Usuários"
2. Preencha o form:
   - Username: teste
   - Email: teste@exemplo.com
   - Senha: senha123
   - Nome: Usuário Teste
   - Permissões: Gerenciamento de Usuários
3. Clique em "Cadastrar"

```

### 2. Teste as Permissões

```

1. Faça logout
2. Entre com o novo usuário (teste / senha123)
3. Observe que só tem acesso ao módulo "Usuários"
4. Não consegue acessar "Recursos" ou "Relatórios"

```

### 3. Explore os Relatórios

```

1. Faça login como root
2. Acesse "Relatórios"
3. Veja todas as ações registradas
4. Filtre por usuário ou data

````

## Comandos Úteis

### Docker

```bash
# Parar containers
docker compose -f docker-compose.development.yml stop

# Reiniciar containers
docker compose -f docker-compose.development.yml restart

# Ver logs
docker compose -f docker-compose.development.yml logs -f api
docker compose -f docker-compose.development.yml logs -f webapp

# Recriar do zero (CUIDADO: apaga dados!)
docker compose -f docker-compose.development.yml down -v
docker compose -f docker-compose.development.yml up -d
````

### Backend (Sem Docker)

```bash
cd Api

# Restaurar dependências
dotnet restore

# Aplicar migrations
dotnet ef database update

# Executar
dotnet run

# Hot reload (watch mode)
dotnet watch run
```

### Frontend (Sem Docker)

```bash
cd WebApp

# Instalar dependências
npm install

# Executar dev server
npm run dev

# Build para produção
npm run build

# Preview do build
npm run preview
```

## Testes de API

### cURL

```bash
# Login
curl -X POST http://localhost:{PORT}/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"identifier":"root","password":"root1234"}'

# Salve o token retornado
TOKEN="eyJhbGciOiJIUzI1NiIs..."

# Listar usuários
curl -X GET "http://localhost:{PORT}/api/users?page=1&limit=10" \
  -H "Authorization: Bearer $TOKEN"

# Criar usuário
curl -X POST http://localhost:{PORT}/api/users \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "username":"novo",
    "email":"novo@exemplo.com",
    "password":"senha123",
    "fullName":"Novo Usuário",
    "permissionsIds":[2]
  }'
```

### Swagger UI

1. Acesse: http://localhost:{PORT}/swagger
2. Clique em "Authorize"
3. Digite: `Bearer {seu-token}`
4. Teste os endpoints diretamente

## Estrutura de Pastas (Resumo)

```
admin-panel-boilerplate/
├── Api/                    # Backend .NET
│   ├── Controllers/        # Endpoints REST
│   ├── Services/           # Lógica de negócio
│   ├── Models/             # Entidades do banco
│   ├── Dtos/               # Data Transfer Objects
│   └── Middlewares/        # Autenticação e permissões
│
├── WebApp/                 # Frontend React
│   ├── src/
│   │   ├── pages/          # Páginas
│   │   ├── components/     # Componentes
│   │   ├── hooks/          # Custom hooks
│   │   ├── services/       # Chamadas de API
│   │   └── contexts/       # Estado global
│   └── public/
│
├── DOCS/                   # Documentação completa
└── docker-compose.yml      # Orquestração
```

## Recursos Disponíveis

### Backend

- ✅ Autenticação JWT
- ✅ CRUD de Usuários
- ✅ CRUD de Recursos do Sistema
- ✅ Logs de Auditoria
- ✅ Redefinição de Senha (email)
- ✅ Soft Delete
- ✅ Paginação
- ✅ RBAC (Role-Based Access Control)

### Frontend

- ✅ Login/Logout
- ✅ Gerenciamento de Usuários
- ✅ Gerenciamento de Recursos
- ✅ Relatórios com Filtros
- ✅ Perfil do Usuário
- ✅ Tema Claro/Escuro
- ✅ Proteção de Rotas
- ✅ Menu Dinâmico por Permissões

## Próximos Passos

### Para Usuários

1. ✅ [Instalação Completa](./01-INSTALACAO.md)
2. ✅ [Guia de Uso](./07-GUIA-DE-USO.md)
3. ✅ [Sistema de Permissões](./06-PERMISSOES.md)

### Para Desenvolvedores

1. ✅ [Arquitetura](./02-ARQUITETURA.md)
2. ✅ [Backend](./03-BACKEND.md)
3. ✅ [Frontend](./04-FRONTEND.md)
4. ✅ [Desenvolvimento](./08-DESENVOLVIMENTO.md)

### Para Integrações

1. ✅ [API Reference](./05-API-REFERENCE.md)

## Problemas Comuns

### Porta já em uso

```bash
# Descubra qual processo usa a porta
lsof -i :{PORT}

# Mate o processo
kill -9 <PID>

# Ou altere a porta em Api/.env
API_PORT=5211
```

### Containers não iniciam

```bash
# Veja os logs
docker-compose logs

# Reconstrua as imagens
docker-compose build --no-cache
docker-compose up -d
```

### Frontend não conecta com API

```bash
# Verifique a variável de ambiente
cat WebApp/.env
# Deve conter: VITE_API_BASE_URL=http://localhost:{PORT}/api

# Reinicie o dev server
cd WebApp
npm run dev
```

### Banco de dados vazio

```bash
# Verifique a variável RUN_USERS_SEED
cat Api/.env
# Deve conter: RUN_USERS_SEED=true

# Reinicie o container da API
docker-compose restart api
```

## Variáveis de Ambiente Essenciais

### Backend (Api/.env)

```env
# Banco de Dados
DB_HOST=db                  # ou localhost sem Docker
DB_PORT=5432
DB_USER=postgres
DB_PASSWORD=postgres
DB_NAME=admin_panel_db

# Configuração para Docker (PostgreSQL container)
POSTGRES_USER=postgres
POSTGRES_PASSWORD=postgres
POSTGRES_DB=admin_panel_db

# Seeds
RUN_USERS_SEED=true          # Cria dados iniciais (usuário root) e, em
                           # ambiente de desenvolvimento, usuários de teste
                           # (nome mais descritivo que RUN_USERS_SEED).

# API
API_PORT={PORT}

# Segurança
JWT_SECRET_KEY=sua-chave-secreta-minimo-32-caracteres

# CORS
WEB_APP_URL=http://localhost:5173

# Email (Opcional)
RESEND_API_KEY=
RESEND_FROM_EMAIL=
```

### Frontend (WebApp/.env)

```env
VITE_API_BASE_URL=http://localhost:{PORT}/api
```

## Recursos Adicionais

- [Documentação Completa](./README.md)
- [API Swagger](http://localhost:{PORT}/swagger)
- [Reportar Bug](https://github.com/TheRermz/admin-panel-boilerplate/issues)

## Checklist de Produção

Antes de fazer deploy em produção:

- [ ] Altere `JWT_SECRET_KEY` para valor único e seguro
- [ ] Altere senha do usuário root
- [ ] Altere credenciais do banco de dados
- [ ] Configure HTTPS/TLS
- [ ] Configure backup do banco de dados
- [ ] Configure variáveis de ambiente via secrets
- [ ] Defina `RUN_USERS_SEED=false` (ou remova a variável)
- [ ] Configure rate limiting
- [ ] Configure logs estruturados
- [ ] Configure monitoramento
- [ ] Teste disaster recovery

## Suporte

Consulte a [documentação completa](./README.md) para informações detalhadas sobre cada tópico.
