# TekProvider — Mantenimiento de Clientes

Prueba técnica: alta, edición, listado y cambio de estado de clientes, con autenticación JWT. Backend en .NET 8 (Clean Architecture + FastEndpoints), frontend en React + TypeScript, PostgreSQL como base de datos.

## Arquitectura

```
BackEnd/
├── TekProvider.Domain/          # Entidades, enums, excepciones de negocio, puertos de dominio
├── TekProvider.Application/     # Casos de uso, DTOs, validadores (FluentValidation), puertos (interfaces)
├── TekProvider.Infrastructure/  # EF Core + Postgres, repositorios, JWT, hashing, migraciones
├── TekProvider/                 # Api — FastEndpoints, middleware, Program.cs
└── TekProvider.Tests/           # xUnit — dominio, aplicación (NSubstitute) e integración (Testcontainers)

frontend/
└── tekprovider-ui/              # React + Vite + TypeScript
```

Regla de dependencias: `Api → Application → Domain`; `Infrastructure` implementa las interfaces definidas en `Application`/`Domain`.

## Stack

**Backend:** .NET 8, FastEndpoints, EF Core + Npgsql, FluentValidation, JWT Bearer, PBKDF2 (hashing propio, xUnit + NSubstitute + Testcontainers.

**Frontend:** React 19, TypeScript, Vite, TanStack Query, Zustand, React Router, React Hook Form + Zod, Mantine (UI), Fetch API nativo (sin axios).

**Infraestructura:** Docker Compose (PostgreSQL 16 + Adminer), GitHub Actions (CI).

## Requisitos previos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js 20+](https://nodejs.org/)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)

## Levantar el proyecto

### 1. Base de datos

```bash
docker compose up -d
```

Esto levanta Postgres (puerto `5432`, usuario/db `tekprovider`/`tekprovider`) y Adminer (`http://localhost:8080`) para inspeccionar la base visualmente. El primer arranque crea la base automáticamente — no hace falta crearla a mano.

### 2. Migraciones

```bash
cd BackEnd/TekProvider.Infrastructure
dotnet ef database update
```

(Si no tienes la herramienta `dotnet-ef`: `dotnet tool install --global dotnet-ef`.)

### 3. Backend

```bash
cd BackEnd/TekProvider
dotnet run
```

- Swagger: `http://localhost:5042/swagger`
- En el primer arranque (entorno `Development`), se seedea un usuario de prueba: **`admin` / `Admin123!`**.

### 4. Frontend

```bash
cd frontend/tekprovider-ui
npm install
npm run dev
```

- App: `http://localhost:5173`
- La URL de la API se configura en `frontend/tekprovider-ui/.env` (`VITE_API_BASE_URL`, por defecto `http://localhost:5042`).

Inicia sesión con `admin`/`Admin123!`, o regístrate desde la pantalla de login.

## Endpoints

| Método | Ruta | Descripción |
|---|---|---|
| POST | `/api/auth/login` | Login, devuelve JWT |
| POST | `/api/auth/register` | Registro de usuario, devuelve JWT |
| GET | `/api/customers?search=&page=&pageSize=` | Listado paginado + búsqueda |
| GET | `/api/customers/{id}` | Detalle |
| POST | `/api/customers` | Alta |
| PUT | `/api/customers/{id}` | Edición (requiere `rowVersion` → 409 si hay conflicto de concurrencia) |
| PATCH | `/api/customers/{id}/status` | Cambio de estado dedicado (`{ "newStatus": "..." }`) |

Todos los endpoints de `/api/customers/*` requieren `Authorization: Bearer <token>`.

## Reglas de negocio clave

- **Duplicados (RFC/Email)**: garantizados por índices únicos case-insensitive reales en Postgres (columnas generadas con `lower(...)`), con traducción de la violación de constraint a `409 customer.duplicate` — cubre condiciones de carrera, no solo el chequeo previo en memoria.
- **Concurrencia**: `RowVersion` mapeado al `xmin` real de Postgres como concurrency token. Un segundo `PUT` con un `rowVersion` desactualizado devuelve `409 concurrency-conflict` (nunca sobrescribe silenciosamente).
- **Máquina de estados**: la matriz de transiciones permitidas vive en la tabla `CustomerStatusTransitions` (seedeada), consultada en cada cambio de estado — agregar una regla nueva no requiere recompilar. Transición inválida → `400 customer.invalid-status-transition`.
- **Errores**: un `IExceptionHandler` global traduce toda excepción de negocio a `ProblemDetails` consistente (400/401/404/409); cualquier error no contemplado cae a `500` sin filtrar detalles internos.

## Tests

```bash
cd BackEnd
dotnet test
```

39 pruebas: dominio (sin mocks), casos de uso de Application (mockeados con NSubstitute) e integración de Infrastructure (Postgres real y efímero vía Testcontainers — requiere Docker corriendo, se levanta y destruye solo).

## CI/CD

`.github/workflows/ci.yml` corre en cada push/PR a `main`: build + test del backend, y lint + build del frontend.
