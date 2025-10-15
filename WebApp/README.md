# Generic Login Frontend - React + Vite + MaterialUI

> Frontend em **React** com **Vite** e **TypeScript**, integrado com a API .NET deste projeto.  
> Inclui **fluxo de login JWT**, **painel administrativo para usuários**, formulários de cadastro/edição e lista paginada de usuários com opção de busca.

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
│   │   ├── api/               # Instância Axios configurada com baseURL e headers
│   │   ├── components/        # Componentes reutilizáveis (UserTable, UserForm, LoginForm)
│   │   ├── hooks/             # Hooks personalizados (useAuth)
│   │   ├── pages/             # Páginas (Dashboard, Login)
│   │   ├── types/             # Tipagens TypeScript (UserReadDto, LoginPayload, etc)
│   │   ├── App.tsx            # Configuração das rotas e layout principal
│   │   └── main.tsx           # Entrada do React e renderização do App
│   ├── tsconfig.json          # Configuração TypeScript
│   └── package.json           # Dependências e scripts do projeto
│
└── Api/                       # Backend PostgreSQL + .NET
```

---

## Funcionalidades

- **Login JWT**

  - Campo para `identifier` e `password`.
  - Autenticação via API `/auth/login`.
  - Armazena token no `localStorage` e configura cabeçalho `Authorization` para todas as requisições.

- **Painel Administrativo**

  - Lista paginada de usuários (`UserTable`), integrada com a API.
  - Criação e edição de usuários via `UserForm`.
  - Exclusão de usuários diretamente da tabela.
  - Pesquisa de usuários com filtro (`/users/search?key=`).

- **Formulários Reutilizáveis**

  - `UserForm` é utilizado tanto para criação quanto para edição, com modal para edição.

- **Controle de Estados**
  - `useAuth` gerencia token, login, logout e mantém cabeçalho de autorização configurado.
  - Loading states e mensagens de erro exibidas dinamicamente.

---

## Rodando a aplicação localmente

### 1. Instalar dependências

```bash
cd WebApp
npm install
```

### 2. Configurar base URL da API

No arquivo `src/api/index.ts`:

```ts
import axios from 'axios';

const api = axios.create({
  baseURL: 'http://localhost:5209/api', // Altere para a URL da sua API
});

export default api;
```

### 3. Rodar o frontend

```bash
npm run dev
```

- A aplicação estará disponível em `http://localhost:5173`.

---

## Estrutura de Rotas

- `/login` → Página de login com formulário JWT.
- `/dashboard` → Painel administrativo com:
  - Lista de usuários paginada e pesquisável.
  - Botões de editar e excluir usuários.
  - Formulário para criação de novos usuários.

---

## Integração com API

- Todos os endpoints de usuários e autenticação são consumidos via **Axios**.
- Token JWT é enviado automaticamente no header `Authorization: Bearer <token>` após login.
- Requisições de CRUD:

| Método | Endpoint             | Função            |
| ------ | -------------------- | ----------------- |
| POST   | `/auth/login`        | Autenticação JWT  |
| GET    | `/users`             | Listar usuários   |
| GET    | `/users/search?key=` | Buscar usuários   |
| POST   | `/users`             | Criar usuário     |
| PUT    | `/users/{id}`        | Atualizar usuário |
| DELETE | `/users/{id}`        | Remover usuário   |

---

## Sobre o Dev

[Bruno Riwerson Silva](https://www.linkedin.com/in/bruno-riwerson/) é um profissional apaixonado por tecnologia. Desenvolvedor full-stack proficiente no uso de React com MaterialUI no front-end e NodeJS com Express no back-end. Possui experiência no uso de bancos de dados relacionais e não-relacionais, além de conhecer outras tecnologias como Golang, Java, Docker, entre outras, tornando-o dinâmico e apto a solucionar quaisquer problemas de modo eficiente.
