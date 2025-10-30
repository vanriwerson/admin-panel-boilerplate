# Admin Pannel BoilerPlate Frontend - React + Vite + MaterialUI

> Frontend em **React** com **Vite** e **TypeScript**, integrado com a API .NET deste projeto.  
> Fornece um painel administrativo moderno, seguro e escalável, permitindo gerenciamento de **usuários com controle de permissões de acesso** e também o gerenciamento de **recursos do sistema**, além de permitir **auditoria** das ações executadas.
> Inclui um **fluxo completo de autenticação via JWT**, **recuperação de senha** e **rotas protegidas** contra acesso indevido.

---

## Tecnologias Utilizadas

- [**React 18**](https://reactjs.org/): Biblioteca para criação de interfaces declarativas e reativas.
- [**Vite**](https://vitejs.dev/): Bundler moderno e rápido para desenvolvimento frontend.
- [**TypeScript**](https://www.typescriptlang.org/): Superset de JavaScript que adiciona tipagem estática.
- [**MaterialUI (MUI)**](https://mui.com/): Biblioteca de componentes para React com design consistente e responsivo.
- [**Axios**](https://axios-http.com/): Cliente HTTP para consumo da API.
- [**React Router**](https://reactrouter.com/): Gerenciamento de rotas do frontend.

---

## Estrutura do Projeto

```
generic-login-dotnet-react/
│
├── WebApp/
│   ├── public/                # Arquivos estáticos
│   ├── src/
│   │   ├── api/               # Instância Axios configurada com baseURL, headers e interceptors JWT
│   │   ├── components/        # Componentes reutilizáveis (UserTable, UserForm, LoginForm)
│   │   ├── contexts/          # Configuração do ContextApi
│   │   ├── helpers/           # Funções auxiliares
│   │   ├── hooks/             # Hooks personalizados
│   │   ├── interfaces/        # Contratos de Tipagem TypeScript
│   │   ├── pages/             # Páginas principais da aplicação
│   │   ├── permissions/       # Regras do role based access control (RBAC)
│   │   ├── routes/            # Configuração das rotas
│   │   ├── App.tsx            # Configuração do layout principal
│   │   └── main.tsx           # Entrada do React e renderização do App
│   ├── tsconfig.json          # Configuração TypeScript
│   └── package.json           # Dependências e scripts do projeto
│
└── Api/                       # Backend PostgreSQL + .NET
```

---

## Funcionalidades

##### - Login

- Permite autenticação utilizando `username` ou `email` (`identifier`) e `password`.
- Disponibiliza autenticação por redirecionamento enviando `token` via url (desde que utilizando o mesmo `JWT_SECRET_KEY`).
- Armazena token no `localStorage` e configura cabeçalho `Authorization` para todas as requisições.

##### - Perfil

- Exibe as informações do usuário logado, permitindo edição (de acordo com RBAC).

##### - Gerenciamento de Usuários

- Listagem paginada e pesquisável de usuários.
- Formulário de `Criação de usuários`.
- `Edição e exclusão` de usuários diretamente da tabela.
- `Controle de permissões` por recurso do sistema.
- Exibição e edição condicionais com base nas regras RBAC.

##### - Recursos de Sistema (System Resources)

- Listagem paginada e pesquisável de recursos de sistema.
- Formulário de `Criação de recursos de sistema`.
- `Edição e exclusão` de recursos do sistema diretamente da tabela.
- Integração com a gestão de usuários (cada usuário tem uma lista `permissions`, baseada nos recursos do sistema que ele deve acessar).

##### - Relatórios de Auditoria (System Logs)

- Listagem paginada e filtrável dos logs de sistema.
- Geração de relatórios com filtros cumulativos por:
  - Período (início e fim)
  - Usuário específico
  - Ação executada (`create`, `update`, `delete`, `login`, `senha`)

##### - Hooks Personalizados

- `useAuth` gerencia token, login, logout e mantém cabeçalho de autorização configurado.
- `useUsers`, `useSystemResources` e `useReports` fazem a abstração entre a camada services e a UI, persistindo dados para exibição, ações CRUD e paginação.

---

## Rodando a aplicação localmente

### 1. Instalar dependências

```bash
cd WebApp
npm install
```

### 2. Configurar base URL da API

- Crie um arquivo `WebApp/.env` e nele defina VITE_API_BASE_URL com a url de sua api.
  > Essa variável será utilizada pelo arquivo `src/api/index.ts` para configurar a instância do axios.

### 3. Rodar a aplicação

```bash
npm run dev
```

> A aplicação estará disponível em `http://localhost:5173`.

---

## Estrutura de Rotas

| Rota            | Descrição                                                |
| --------------- | -------------------------------------------------------- |
| `/login`        | Tela de autenticação                                     |
| `/profile`      | Informações do usuário logado                            |
| `/unauthorized` | Redirecionamento em caso de acesso à rota não autorizada |
| `/users`        | Painel de gestão de usuários                             |
| `/resources`    | Página de gerenciamento de recursos do sistema           |
| `/reports`      | Relatórios de auditoria filtráveis e paginados           |

---

## Integração com API

- Todos os endpoints da aplicação são consumidos via instância configurada do **Axios**.
- Token JWT é enviado automaticamente no header `Authorization: Bearer <token>` após login.
- Todas as chamadas à api são gerenciadas pela camada `services`.

---

## Sobre o Desenvolvedor

[Bruno Riwerson Silva](https://www.linkedin.com/in/bruno-riwerson/) é um **desenvolvedor full-stack** apaixonado por tecnologia e boas práticas de engenharia de software. Proficiente no uso de **React+MaterialUI** no front-end e **NodeJS+Express** no back-end, além de conhecer outras tecnologias como `Golang`, `Java`, `Docker`, entre outras. Possui experiência no uso de bancos de dados relacionais e não-relacionais, o que o torna um profissional dinâmico e apto a criar soluções escaláveis, seguras e bem estruturadas.
