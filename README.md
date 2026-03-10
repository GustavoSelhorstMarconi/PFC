# PFC — Personal Finance Control (Backend)

API REST do sistema de controle financeiro pessoal, construída com .NET 10 e PostgreSQL.

## Tecnologias

- **.NET 10** + ASP.NET Core
- **Entity Framework Core** + PostgreSQL (Npgsql)
- **JWT Bearer** (autenticação com refresh token)
- **FluentValidation** (validação de requests)
- **BCrypt** (hash de senhas)
- **Swagger / OpenAPI** (documentação da API)
- **AspNetCoreRateLimit** (rate limiting)

## Pré-requisitos

- .NET 10 SDK
- PostgreSQL 14+

## Instalação e execução

```bash
# Clone o repositório
git clone <url-do-repositorio>
cd PFC

# Configure a connection string e demais settings
# Edite PFC.API/appsettings.json (ou use user-secrets)

# Aplique as migrations do banco de dados
dotnet ef database update --project PFC.Infra --startup-project PFC.API

# Execute a API
dotnet run --project PFC.API
```

A API estará disponível em:
- HTTP: `http://localhost:5122`
- HTTPS: `https://localhost:7014`

A documentação Swagger é acessível na raiz: `https://localhost:7014`.

## Configuração

Edite `PFC.API/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=PFC;Username=postgres;Password=sua_senha"
  },
  "JwtSettings": {
    "SecretKey": "sua_chave_secreta_longa",
    "Issuer": "PFC.Api",
    "Audience": "PFC.Client",
    "AccessTokenExpirationMinutes": "30",
    "RefreshTokenExpirationDays": "7"
  },
  "Cors": {
    "AllowedOrigins": ["http://localhost:5173"]
  }
}
```

## Endpoints principais

| Recurso | Rota base | Auth |
|---------|-----------|------|
| Autenticação | `POST /api/auth/login`, `/register`, `/refresh`, `/revoke` | Não |
| Contas | `GET/POST/PUT /api/accounts` | Sim |
| Transações | `GET/POST/PUT/DELETE /api/transactions` | Sim |
| Importação | `POST /api/transactions/import/preview`, `/confirm` | Sim |
| Categorias | `GET/POST/PUT /api/categories` | Sim |
| Saldo | `GET /api/balance/total`, `/accounts` | Sim |
| Recorrências | `GET/POST/PUT /api/recurrences` | Sim |
| Metas | CRUD `/api/goals` | Sim |
| Dívidas | `GET/POST/PUT /api/debts` | Sim |
| Dashboard | `GET /api/dashboard/summary`, e outros | Sim |

## Arquitetura

O projeto segue **Clean Architecture** dividida em quatro camadas:

```
PFC.Domain      # Entidades e regras de negócio core
PFC.Application # Serviços, casos de uso e validações
PFC.Infra       # EF Core, repositórios, segurança JWT
PFC.API         # Controllers, middlewares, configuração
PFC.Dto         # DTOs de request e response
```

## Funcionalidades

- Autenticação JWT com refresh token e revogação
- Importação de transações via CSV e OFX (preview + confirmação)
- Dashboard com analytics financeiros
- Geração automática de transações a partir de recorrências
- Rate limiting nos endpoints de autenticação
- Seed automático de categorias padrão
