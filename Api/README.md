# Generic Login .NET + React

> Boilerplate criado com fluxo completo de autenticação JWT, pensado em PostgreSQL + .NET + React.
> Com seu Repositório Genérico, possibilita grande reaproveitamento de código, bastando injetar o dto específico para obter o CRUD básico para quaisquer entidades criadas.

---

## Tecnologias Utilizadas

- [**.NET 8**](https://learn.microsoft.com/en-us/dotnet/core/introduction): Framework moderno, multiplataforma e de código aberto para criação de APIs, aplicações web e serviços.
- [**Entity Framework Core**](https://learn.microsoft.com/en-us/ef/core/): ORM oficial do .NET que simplifica o acesso a bancos de dados relacionais por meio de mapeamento objeto-relacional.
- [**PostgreSQL**](https://www.postgresql.org/): Banco de dados relacional open source, robusto e altamente extensível, com suporte completo ao padrão SQL.
- [**JSON Web Token (JWT)**](https://jwt.io/introduction/): Padrão aberto para autenticação e troca segura de informações entre cliente e servidor.
- [**React**](https://react.dev/): Biblioteca JavaScript para criação de interfaces de usuário dinâmicas e componentizadas.
- [**Vite**](https://vitejs.dev/): Ferramenta de build rápida e moderna que melhora o desempenho do desenvolvimento frontend.
- [**TypeScript**](https://www.typescriptlang.org/): Superset do JavaScript que adiciona tipagem estática e recursos avançados para maior produtividade e segurança no código.
- [**Docker Compose**](https://docs.docker.com/compose/): Ferramenta para definir e gerenciar múltiplos containers Docker de forma simples e declarativa.

---

## Estrutura do projeto

```
generic-login-dotnet-react/
│
├── Api/                  # Backend .NET
│   ├── Controllers/      # Controllers da API
│   ├── Data/             # DbContext e configurações do banco
│   ├── Dtos/             # Data Transfer Objects
│   ├── Helpers/          # Helpers utilitários (paginação, snake_case, etc)
│   ├── Middlewares/      # Validações adicionais
│   ├── Models/           # Models do banco de dados
│   ├── Services/         # Lógica de negócios
│   ├── Program.cs        # Configuração da aplicação
│   └── .env              # Variáveis de ambiente
│
├── docker-compose.yml    # Orquestração Docker
└── frontend/             # Frontend React + Vite + TypeScript
```

---

## Configuração do Docker

Vide arquivo `./docker-compose.yml`

> O Postgres será exposto na **porta 5432** do host.

---

## Rodando a aplicação localmente

### 1. Subir o container do banco

```bash
docker compose up -d
```

Verifique se o container está rodando:

```bash
docker ps
```

---

### 2. Aplicar migrations do EF Core

```bash
cd Api
dotnet ef database update
```

> Isso criará a tabela `users` e a tabela `__EFMigrationsHistory`.

---

### 3. Rodar a API .NET

```bash
dotnet run
```

- A API estará disponível em `https://localhost:<API_PORT>`.

---

### Observações

- As variáveis de ambiente são obrigatórias; se alguma não estiver configurada, a aplicação lançará uma exceção ao iniciar.
- Logs de inicialização indicam se a **conexão com o banco** foi bem-sucedida.

---

## Sobre o Dev

[Bruno Riwerson Silva](https://www.linkedin.com/in/bruno-riwerson/) é um profissional apaixonado por tecnologia. Desenvolvedor full-stack proficiente no uso de React com MaterialUI no front-end e NodeJS com Express no back-end. Possui experiência no uso de bancos de dados relacionais e não-relacionais, além de conhecer outras tecnologias como Golang, Java, Docker, entre outras, tornando-o dinâmico e apto a solucionar quaisquer problemas de modo eficiente.
