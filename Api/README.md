# Generic Login Backend - PostgreSQL + .NET

> Api com fluxo completo de autenticação **JWT**, implementado em **PostgreSQL + .NET**.  
> Inclui **hash seguro de senhas (BCrypt)**, **emissão e validação de tokens JWT**, **controle customizável de permissões de acesso**,
> **logs de sistema integrados** e um **repositório genérico** que permite criar CRUDs rapidamente apenas injetando DTOs específicos.

---

## Tecnologias Utilizadas

- [**PostgreSQL**](https://www.postgresql.org/): Banco de dados relacional open source, robusto e altamente extensível, com suporte completo ao padrão SQL.
- [**.NET 8**](https://learn.microsoft.com/en-us/dotnet/core/introduction): Framework moderno, multiplataforma e de código aberto para criação de APIs, aplicações web e serviços.
- [**Entity Framework Core**](https://learn.microsoft.com/en-us/ef/core/): ORM oficial do .NET que simplifica o acesso a bancos de dados relacionais por meio de mapeamento objeto-relacional.
- [**BCrypt**](https://www.nuget.org/packages/BCrypt.Net-Next/): Biblioteca utilizada para hash e verificação de senhas com o algoritmo bcrypt, garantindo maior segurança no armazenamento de credenciais.
- [**JSON Web Token (JWT)**](https://jwt.io/introduction/): Padrão aberto para autenticação e troca segura de informações entre cliente e servidor.
- [**Swagger**](https://swagger.io/docs/): Conjunto de ferramentas para documentação e testes interativos de APIs REST.
- [**Docker Compose**](https://docs.docker.com/compose/): Ferramenta para definir e gerenciar múltiplos containers Docker de forma simples e declarativa.

---

## Estrutura do projeto

```
generic-login-dotnet-react/
│
├── Api/                  # Backend .NET
│   ├── Controllers/      # Controllers da API
│   ├── Data/             # DbContext, configurações do banco e seeders
│   ├── Dtos/             # Data Transfer Objects
│   ├── Helpers/          # Helpers utilitários (paginação, snake_case, etc)
│   ├── Middlewares/      # Validações adicionais
│   ├── Migrations/       # Estrutura inicial do banco de dados
│   ├── Models/           # Entidades do banco de dados
│   ├── Services/         # Lógica de negócios
│   ├── Program.cs        # Configuração da aplicação
│   └── .env              # Variáveis de ambiente
│
├── docker-compose.yml    # Orquestração Docker
└── WebApp/               # Frontend React + Vite + TypeScript
```

---

## Configuração do Docker

Vide arquivo `./docker-compose.yml`

> O Postgres será exposto na **porta 5432** do host.

---

## Rodando a aplicação localmente

Antes de rodar a aplicação, crie o arquivo `Api/.env` conforme o arquivo `Api/.env.example`.

> 🔒 **Dica:** Gere uma chave segura para `JWT_SECRET_KEY` executando o comando:
>
> ```bash
> echo "JWT_SECRET_KEY=$(openssl rand -base64 64)"
> ```

---

### 1. Subir o container do banco

Vide arquivo `./docker-compose.yml`

O banco PostgreSQL será exposto na **porta 5432** do host.

```bash
docker compose up -d db
```

Verifique se o container está rodando:

```bash
docker ps
```

> Você deverá ver o nome do container `admin-panel-db` no terminal

---

### 2. Aplicar migrations do EF Core

```bash
cd Api
dotnet ef database update
```

> Isso criará o banco de dados e as tabelas iniciais:

- `access_permissions`
- `system_logs`
- `system_resources`
- `users`
- `__EFMigrationsHistory`

de acordo com a Migration InitialCreate

---

### 3. Rodar a API

```bash
dotnet run
```

- A API estará disponível em `https://localhost:<API_PORT>`.

---

### Observações

- As variáveis de ambiente são obrigatórias; se alguma não estiver configurada, a aplicação lançará uma exceção ao iniciar.
- Logs de inicialização indicam se a **conexão com o banco** foi bem-sucedida.

---

## 🔐 Fluxo de Autenticação JWT

A Api inclui um **sistema completo de autenticação JWT**, composto pelos helpers e services abaixo:

### Helpers

| Helper            | Função                                                                   |
| ----------------- | ------------------------------------------------------------------------ |
| `PasswordHashing` | Criação e verificação de hashes de senha com **BCrypt**                  |
| `JsonWebToken`    | Geração, validação e decodificação de tokens JWT usando `JWT_SECRET_KEY` |

---

### Services

| Service                | Descrição                                                                                                                                |
| ---------------------- | ---------------------------------------------------------------------------------------------------------------------------------------- |
| `LoginService`         | Autentica usuários via e-mail ou userName (identifier) / senha, valida com BCrypt e gera JWT                                             |
| `ExternalTokenService` | (Uso corporativo: Redirecionamento via intranet) Recebe um token externo, valida com o mesmo `JWT_SECRET_KEY` e troca por um JWT interno |

---

### **Autenticação (`/api/auth`)**

| Método | Rota                 | Descrição                                                                               |
| ------ | -------------------- | --------------------------------------------------------------------------------------- |
| `POST` | `/api/auth/login`    | Login com credenciais locais (`identifier`, `password`). Retorna um JWT válido.         |
| `POST` | `/api/auth/external` | Autenticação via token externo corporativo. Decodifica, valida e troca por JWT interno. |

#### Exemplo — Login local

**Request**

```json
{
  "identifier": "judy", // Usuário criado no seed
  "password": "123456"
}
```

**Response**

```json
{
  "token": "eyJhbGciOiJIUzI1NiIs..."
}
```

#### Exemplo — Autenticação via token externo

**Request**

```json
{
  "externalToken": "token_fornecido_pelo_sso_corporativo"
}
```

**Response**

```json
{
  "token": "eyJhbGciOiJIUzI1NiIs..."
}
```

---

## 🌐 Alguns Endpoints Disponíveis

### **Usuários (`/api/users`)**

| Método   | Rota                                | Descrição                                                              |
| -------- | ----------------------------------- | ---------------------------------------------------------------------- |
| `GET`    | `/api/users`                        | Lista todos os usuários                                                |
| `GET`    | `/api/users/search?key=algumaCoisa` | Lista todos os usuários encontrados na busca (name, fullName ou email) |
| `GET`    | `/api/users/{id}`                   | Obtém detalhes de um usuário                                           |
| `GET`    | `/api/users/options`                | Retorna lista resumida (`UserLogReadDto[]`) para selects de relatórios |
| `POST`   | `/api/users`                        | Cria um novo usuário                                                   |
| `PUT`    | `/api/users/{id}`                   | Atualiza um usuário existente                                          |
| `DELETE` | `/api/users/{id}`                   | Remove um usuário                                                      |

---

### **Logs do Sistema (`/api/reports`)**

| Método | Rota           | Descrição                                           |
| ------ | -------------- | --------------------------------------------------- |
| `GET`  | `/api/reports` | Retorna logs filtrados por usuário, ação ou período |

> Suporta os queryParams `userId`, `action`, `startDate`, `endDate`, `page` e `pageSize`.
> Exemplo: http://localhost:<API_PORT>/api/reports?userId=11&startDate=2025-10-22&endDate=2025-10-23

---

### Documentação da API

A API já vem integrada com **Swagger**. Para visualizar a documentação dos endpoints e testar requisições:

- Abra no navegador: `http://localhost:<API_PORT>/swagger/`
- Todos os endpoints disponíveis serão listados com detalhes de parâmetros, respostas e exemplos.

---

## Controle de Permissões

O controle de permissões é baseado na entidade `system_resources`, que representa **módulos ou funcionalidades** da api.
Cada usuário possui uma lista de permissões vinculadas a recursos específicos, determinando quais ações ele pode executar.

---

## 📜 Logs e Auditoria

Cada ação do tipo CREATE, UPDATE, DELETE ou LOGIN cia um registro em `system_logs`, contendo:

- Id do usuário autenticado (responsável pela ação)
- Descrição da ação executada
- Data e hora em que a ação foi executada

## Sobre o Dev

[Bruno Riwerson Silva](https://www.linkedin.com/in/bruno-riwerson/) é um profissional apaixonado por tecnologia. Desenvolvedor full-stack proficiente no uso de React com MaterialUI no front-end e NodeJS com Express no back-end. Possui experiência no uso de bancos de dados relacionais e não-relacionais, além de conhecer outras tecnologias como Golang, Java, Docker, entre outras, tornando-o dinâmico e apto a solucionar quaisquer problemas de modo eficiente.
