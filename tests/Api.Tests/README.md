# Admin Panel Boilerplate - Test Suite

> Projeto de testes automatizados da API .NET.
>
> A suíte foi construída utilizando testes unitários, testes de integração e análise de cobertura de código para garantir a estabilidade do template ao longo do tempo.
>
> O objetivo é manter uma cobertura mínima de **80%**, validando regras de negócio, validações, serviços, repositórios e endpoints da API.

---

# Tecnologias Utilizadas

## xUnit

Framework principal de testes utilizado no ecossistema .NET.

Responsável por:

- Organização dos testes
- Execução dos testes
- Relatórios de sucesso/falha

---

## Moq

Biblioteca para criação de objetos simulados (Mocks).

Permite testar serviços e validações sem depender de:

- Banco de dados
- APIs externas
- Serviços de terceiros

---

## FluentAssertions

Biblioteca de asserções com sintaxe mais legível.

Exemplo:

```csharp
result.Should().BeTrue();
```

ao invés de:

```csharp
Assert.True(result);
```

---

## Coverlet

Ferramenta responsável pela coleta de cobertura de testes.

Permite identificar:

- Linhas cobertas
- Linhas não cobertas
- Métodos não testados

---

## ReportGenerator

Transforma os relatórios XML do Coverlet em relatórios HTML navegáveis.

---

## Microsoft.AspNetCore.Mvc.Testing

Biblioteca utilizada para testes de integração da API.

Permite subir uma instância real da aplicação durante os testes.

---

## Testcontainers PostgreSQL

Permite executar um container PostgreSQL temporário para testes de integração.

Cada execução utiliza um banco isolado.

---

# Estrutura do Projeto

```text
tests/
└── Api.Tests/
    │
    ├── Unit/
    │   ├── Guards/
    │   ├── Validators/
    │   ├── Services/
    │   ├── Policies/
    │   └── Mappers/
    │
    ├── Integration/
    │   ├── Repositories/
    │   └── Controllers/
    │
    ├── TestResults/
    │
    ├── Api.Tests.csproj
    └── README.md
```

---

# Executando os Testes

Na raiz do monorepo:

```bash
dotnet test
```

Resultado esperado:

```text
Total tests: XX
Passed: XX
Failed: 0
```

---

# Executando Apenas o Projeto de Testes

```bash
dotnet test tests/Api.Tests
```

---

# Executando Uma Classe Específica

```bash
dotnet test --filter UserValidatorTests
```

---

# Executando Um Método Específico

```bash
dotnet test --filter ValidateCreateAsync_ShouldThrow_WhenUsernameAlreadyExists
```

---

# Gerando Cobertura de Testes

Na raiz do projeto:

```bash
dotnet test \
/p:CollectCoverage=true \
/p:CoverletOutputFormat=cobertura \
/p:CoverletOutput=./tests/Api.Tests/TestResults/coverage
```

Ao final da execução será criado:

```text
tests/Api.Tests/TestResults/
└── coverage.cobertura.xml
```

---

# Gerando Relatório HTML

Após gerar o XML de cobertura:

```bash
reportgenerator \
-reports:tests/Api.Tests/TestResults/coverage.cobertura.xml \
-targetdir:CoverageReport \
-reporttypes:Html
```

Será criada a pasta:

```text
CoverageReport/
├── index.html
├── classes.html
├── history.html
└── ...
```

---

# Automatizando com Script

Para automatizar a geração do relatório de cobertura em um único passo, execute o script a partir da raiz do monorepo:

```bash
./scripts/coverage.sh
```

Esse script faz o seguinte:

- Executa `dotnet test` com coleta de cobertura (`Coverlet`)
- Remove a pasta antiga `CoverageReport`
- Gera o relatório HTML com `reportgenerator`
- Abre automaticamente `CoverageReport/index.html`

> A explicação dos comandos manuais acima continua válida. Use o script quando quiser um atalho, ou os comandos separados quando precisar de controle mais fino.

---

# Visualizando o Relatório

Ubuntu:

```bash
xdg-open CoverageReport/index.html
```

Ou abrir manualmente:

```text
CoverageReport/index.html
```

em qualquer navegador.

---

# Interpretando o Relatório

O relatório apresenta:

## Line Coverage

Percentual de linhas executadas pelos testes.

Exemplo:

```text
Line Coverage: 84.3%
```

---

## Branch Coverage

Percentual de caminhos condicionais executados.

Exemplo:

```csharp
if (user == null)
```

Para atingir cobertura completa é necessário testar:

- condição verdadeira
- condição falsa

---

## Method Coverage

Percentual de métodos executados durante os testes.

---

# Meta de Cobertura

Cobertura mínima desejada:

```text
80%
```

Objetivo ideal:

```text
85%+
```

Não é obrigatório atingir 100%.

Coberturas muito altas frequentemente indicam:

- testes redundantes
- aumento do custo de manutenção

---

# Estratégia de Crescimento da Cobertura

A ordem recomendada para implementação dos testes é:

1. Guards
2. Validators
3. Policies
4. Mappers
5. Services
6. Orchestrators
7. Repositories
8. Controllers

Essa abordagem maximiza o ganho de cobertura com o menor esforço.

---

# Convenções Adotadas

Nome dos arquivos:

```text
ClasseTestadaTests.cs
```

Exemplo:

```text
UserValidatorTests.cs
GuardTests.cs
CreateUserTests.cs
```

---

Nome dos métodos:

```csharp
Metodo_Cenario_ResultadoEsperado
```

Exemplo:

```csharp
ValidateCreateAsync_ShouldThrow_WhenEmailAlreadyExists
```

---

# Arquivos Ignorados pelo Git

Os seguintes diretórios não devem ser versionados:

```text
bin/
obj/
TestResults/
CoverageReport/
```

Exemplo de .gitignore:

```gitignore
bin/
obj/
TestResults/
CoverageReport/
```

---

# Fluxo Recomendado para Desenvolvimento

Após implementar uma nova funcionalidade:

1. Criar os testes.
2. Executar:

```bash
dotnet test
```

3. Verificar cobertura:

```bash
dotnet test \
/p:CollectCoverage=true \
/p:CoverletOutputFormat=cobertura
```

4. Gerar relatório HTML:

```bash
reportgenerator \
-reports:tests/Api.Tests/TestResults/coverage.cobertura.xml \
-targetdir:CoverageReport \
-reporttypes:Html
```

5. Garantir que a cobertura permaneça acima de 80%.
