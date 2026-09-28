# fiap-esperanca-solidaria-usuario-api

API de **usuários, autenticação e sessão** da plataforma Esperança Solidária (FIAP 11NETT). Cadastra
doadores e gestores de ONG no Firebase Authentication e no PostgreSQL, faz login/refresh de token, mantém
a sessão no Redis e tem um publisher de e-mail para a fila SQS consumida pela `notificacao-lambda`
(hoje desligado no código).

> Para subir o ambiente completo (k8s, LocalStack, API Gateway, front), siga o README do repositório
> **`fiap-esperanca-solidaria-infra`**. Este documento cobre a API isoladamente.

---

## Sumário

- [Stack](#stack)
- [Como a autenticação funciona](#como-a-autenticação-funciona)
- [Estrutura da solução](#estrutura-da-solução)
- [Endpoints](#endpoints)
- [Configuração](#configuração)
- [Rodando localmente](#rodando-localmente)
- [Rodando no Kubernetes](#rodando-no-kubernetes)
- [Testes](#testes)
- [CI/CD](#cicd)
- [Troubleshooting](#troubleshooting)

---

## Stack

| Tecnologia | Uso |
|---|---|
| .NET 10 / ASP.NET Core | API REST |
| MediatR + FluentValidation | Commands/queries e validação |
| Firebase Admin SDK + Identity Toolkit REST | Criação de usuário, custom claims (`roles`), login e refresh de token |
| Entity Framework Core + Npgsql | Persistência dos usuários (`users-db`), migrations no startup |
| Redis | Sessões (1 h, renovadas a cada refresh de token) |
| AWS SDK SQS | Publisher de e-mail para a `notification-queue` (chamada comentada no cadastro) |
| AWS SDK S3 | Foto de perfil (bucket `usuario-images`) |
| Serilog + OpenTelemetry + Prometheus | Logs, traces (Tempo) e métricas (`/metrics`) |
| OpenAPI + Scalar | Documentação interativa |
| xUnit | Testes (`FiapEsperancaSolidaria.Usuario.Teste`) |

---

## Como a autenticação funciona

```
Cadastro (POST /User/Doador)
  └─▶ cria usuário no Firebase ─▶ grava no Postgres ─▶ custom claim roles=["Doador"]

Login (POST /User/Login {email, password})
  └─▶ Firebase signInWithPassword ─▶ cria sessão no Redis (TTL 1 h)
      └─▶ { sessionId, idToken, refreshToken, expiresIn, email }

Refresh (POST /User/RefreshToken {sessionId, refreshToken})
  └─▶ Firebase securetoken ─▶ novo idToken, sessão renovada por mais 1 h

Chamadas protegidas (qualquer API): Authorization: Bearer <idToken>
  └─▶ JWT do Firebase validado pelo Lambda authorizer (gateway) e/ou pela própria API

Logout (DELETE /User/Session/{sessionId}) ─▶ remove a sessão do Redis
```

- Os papéis são **`Doador`** e **`GestorONG`**, gravados como custom claim `roles` no Firebase — é essa
  claim que o `lambda-authorizer` e a `campanha-api` leem.
- O cadastro público cria sempre `Doador`. Um gestor só é criado por outro gestor (`POST /User/GestorONG`)
  ou promovido com `PUT /User/MakeGestorONG`.
- Depois de mudar os papéis de alguém, o usuário precisa fazer login (ou refresh) de novo para o token
  refletir a nova claim.

---

## Estrutura da solução

Solução: `src/fiap-esperanca-solidaria-usuario-api.slnx`

```
.
├── src/                                          # Host ASP.NET Core (FiapEsperancaSolidaria.Usuario.Api)
│   ├── Controllers/v1/UserController.cs          # Todos os endpoints
│   ├── Configurations/                           # Auth (JWT Firebase), CORS, OpenAPI/Scalar, health, migrations, Serilog
│   ├── Contracts/Requests/                       # CreateUserRequest, UpdateUserRequest
│   ├── Middlewares/                              # ExceptionMiddleware, RequestResponseLoggingMiddleware
│   ├── Program.cs
│   ├── appsettings.json                          # Base (Firebase e connection strings vazios — vêm de env/secret)
│   ├── appsettings.Development.json              # CORS e S3 locais
│   └── fiap-esperanca-solidaria-usuario-api.http # Requisições de exemplo
├── fiap-esperanca-solidaria-application/         # Casos de uso
│   ├── UserFeature/Commands/                     # AuthUser (login), CreateUser, UpdateUser, UploadUserImage,
│   │                                             # RefreshToken, LogoutSession, MakeGestorONG
│   ├── UserFeature/Queries/GetSession
│   └── Sessions/SessionLifetime.cs               # Duração da sessão (1 h)
├── fiap-esperanca-solidaria-usuario-auth/        # Integração Firebase (AuthService, FirebaseService: claims, login, refresh)
├── fiap-esperanca-solidaria-usuario-domain/      # Entidades, contratos (repositórios, publisher) e exceções
├── fiap-esperanca-solidaria-usuario-infrastructure/ # EF Core (DbContext, Migrations), repositórios, Redis, S3, CurrentUser
├── fiap-esperanca-solidaria-usuario-contract/    # DTOs de resposta (LoginResponse etc.)
├── fiap-esperanca-solidaria-usuario-observability/ # OpenTelemetry, Prometheus
├── fiap-esperanca-solidaria-usuario-shared/      # Abstrações e Option types
├── FiapEsperancaSolidaria.Usuario.Queue/         # SQS: EmailNotificationPublisher
├── FiapEsperancaSolidaria.Usuario.Teste/         # Testes
└── docker/dockerfile                             # Build multi-stage (restaura/compila só o projeto da API)
```

### Arquivos importantes

- **`src/Controllers/v1/UserController.cs`** — rota base `api/v1/User`.
- **`fiap-esperanca-solidaria-usuario-auth/Adapter/FirebaseService.cs`** — onde a claim `roles` é gravada
  (`SetCustomUserClaimsAsync`).
- **`FiapEsperancaSolidaria.Usuario.Queue/Publisher/EmailNotificationPublisher.cs`** — envia
  `{ To, Subject, Body, CorrelationId }` para `SqsSettings:EmailQueueUrl`; é o formato que a
  `notificacao-lambda` espera.
- **`src/Configurations/ApiConfig.cs`** — política CORS `Frontend` a partir de `Cors:AllowedOrigins`.
- **`docker/dockerfile`** — restaura e compila apenas `src/FiapEsperancaSolidaria.Usuario.Api.csproj`
  (a `.slnx` inclui o projeto de testes, que não é copiado para a imagem).

---

## Endpoints

Base direta: `http://localhost:5043` (local) ou `http://localhost:30084` (k8s). Via API Gateway o prefixo
é `/users` (ex.: `/users/api/v1/User/Login`).

| Método | Rota | Acesso | Corpo / descrição |
|---|---|---|---|
| POST | `/api/v1/User/Doador` | anônimo | `{ name, email, password, cpf, image? }` — cadastro público de doador |
| POST | `/api/v1/User/GestorONG` | GestorONG | Mesmo corpo — cria outro gestor |
| PUT | `/api/v1/User/Doador/{userId}` | Doador | `UpdateUserRequest` — atualiza o próprio cadastro |
| PUT | `/api/v1/User/GestorONG/{userId}` | GestorONG | `UpdateUserRequest` |
| POST | `/api/v1/User/images` | anônimo | multipart `file` (imagem, até 5 MB) → URL no S3 (usada no cadastro) |
| POST | `/api/v1/User/Login` | anônimo | `{ email, password }` → `{ sessionId, idToken, refreshToken, expiresIn, email }` |
| POST | `/api/v1/User/RefreshToken` | anônimo | `{ sessionId, refreshToken }` → mesmo formato do login |
| GET | `/api/v1/User/Session/{sessionId}` | autenticado (gateway) | Dados da sessão; `404` se expirada |
| DELETE | `/api/v1/User/Session/{sessionId}` | autenticado (gateway) | Logout → `204` |
| PUT | `/api/v1/User/MakeGestorONG` | GestorONG | `{ email }` — promove um usuário a gestor |

Login/refresh inválidos retornam `401`.

### Infra

| Rota | Descrição |
|---|---|
| `/scalar` | Documentação Scalar (OpenAPI em `/openapi/v1.json`) |
| `/health`, `/health/ready`, `/health/live` | Health checks |
| `/metrics` | Métricas Prometheus |

---

## Configuração

| Chave | Descrição |
|---|---|
| `ConnectionStrings:DefaultConnection` | PostgreSQL (`users-db`) |
| `ConnectionStrings:Redis` | Redis das sessões |
| `Firebase:ProjectId` | Projeto Firebase (`esperancasolidaria`) |
| `Firebase:ApiKey` | Web API key (login e refresh via Identity Toolkit) |
| `Firebase:CredentialJson` | Conteúdo do JSON da service account (Admin SDK) |
| `Cors:AllowedOrigins` | Origens do front (ex.: `http://localhost:5173`) |
| `SqsSettings:Region/AccessKey/SecretKey/ServiceUrl` | SQS (LocalStack) |
| `SqsSettings:EmailQueueUrl` | Fila `notification-queue` |
| `S3Settings:*` | S3 (LocalStack), `BucketName = usuario-images`, `PublicBaseUrl` |
| `OpenTelemetry:ServiceName` / `TempoEndpoint` | Traces |

> **Segurança:** `Firebase:CredentialJson` e `Firebase:ApiKey` ficam vazios no `appsettings.json` de
> propósito. Forneça-os por variável de ambiente (`Firebase__CredentialJson`, `Firebase__ApiKey`),
> user-secrets ou secret do Kubernetes — **nunca commite** a service account.

---

## Rodando localmente

Pré-requisitos: .NET SDK 10, PostgreSQL, Redis, LocalStack e as credenciais do Firebase. Os containers do
`docker-compose` da infra atendem (Postgres em `localhost:5444`, Redis `6379`, LocalStack `4566`).

```powershell
cd src
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5444;Database=users-db;Username=<user>;Password=<senha>"
dotnet user-secrets set "ConnectionStrings:Redis" "localhost:6379,password=<senha>,abortConnect=false"
dotnet user-secrets set "Firebase:ApiKey" "<web-api-key>"
dotnet user-secrets set "Firebase:CredentialJson" (Get-Content <caminho>\service-account.json -Raw)

aws sqs create-queue --queue-name notification-queue --endpoint-url http://localhost:4566 --region us-east-1

dotnet run --project FiapEsperancaSolidaria.Usuario.Api.csproj
```

- API em http://localhost:5043 (Scalar em http://localhost:5043/scalar).
- As migrations são aplicadas no startup (`ApplyMigrations`).
- Requisições de exemplo em `src/fiap-esperanca-solidaria-usuario-api.http`.

### Docker

```powershell
docker build -f docker/dockerfile -t usuario-api:local .
docker run -p 8080:8080 -e Firebase__ApiKey=... -e Firebase__CredentialJson=... usuario-api:local
```

---

## Rodando no Kubernetes

Deployment em `fiap-esperanca-solidaria-infra/k8s/users-api/` (imagem
`projetofiap/fiap-esperanca-solidaria-usuario-api:latest`, Service `users-api`, NodePort 30084 → 8080).
As credenciais do Firebase chegam pelo `shared-secret` (`FIREBASE_APIKEY`, `FIREBASE_CREDENTIALJSON`,
`FIREBASE_PROJECT_ID`), criado pelo `register-secrets-configs.ps1` da infra.

```powershell
kubectl rollout restart deployment/users-deployment -n apps   # puxa a :latest mais recente
```

> O manifest ainda não define `Cors__AllowedOrigins__0`; se o front chamar a API direto, configure com
> `kubectl set env deployment/users-deployment -n apps Cors__AllowedOrigins__0=http://localhost:5173`.

---

## Testes

```powershell
dotnet test src/fiap-esperanca-solidaria-usuario-api.slnx
```

---

## CI/CD

| Workflow | Gatilho | O que faz |
|---|---|---|
| `ci-push.yml` | push em `feature/**`, `bugfix/**`, `hotfix/**` | build + testes; abre PR para `develop` se não existir |
| `ci-pull-request.yml` | PR para `develop`/`main` | build + testes |
| `cd-release.yml` | tag `v*` (no histórico de `develop`) | build, testes, Trivy e push de `:<tag>` e `:latest` para `projetofiap/fiap-esperanca-solidaria-usuario-api` |

---

## Troubleshooting

| Sintoma | Solução |
|---|---|
| Login retorna `401` | Credenciais inválidas ou `Firebase:ApiKey` incorreta/placeholder. |
| Erro ao iniciar: credencial do Firebase | `Firebase:CredentialJson` vazio ou JSON inválido. |
| `403` na campanha-api/gateway após login | Usuário sem claim `roles` (usuários antigos/seed). Recadastre ou promova e faça login de novo. |
| Sessão some antes do esperado | Sessão expira 1 h após o login/último refresh; use `RefreshToken` para renovar. |
| Upload de foto devolve URL inacessível | Ajuste `S3Settings:PublicBaseUrl` para um host resolvível pelo navegador. |
| E-mail de boas-vindas não chega | Esperado hoje: a publicação está comentada no `CreateUserCommandHandler`. Com ela ativa, confira a fila `notification-queue` e a `notificacao-lambda` (o LocalStack só simula o SES). |
