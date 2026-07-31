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

**Backend:** .NET 8, FastEndpoints, EF Core + Npgsql, FluentValidation, JWT Bearer, PBKDF2 (hashing propio), xUnit + NSubstitute + Testcontainers.

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

## Decisiones de diseño

**Backend**
- **FastEndpoints en vez de MVC Controllers**: patrón REPR (Request-Endpoint-Response), un endpoint = una clase, sin controladores con múltiples acciones. Endpoints seguros por defecto (requieren JWT) salvo `AllowAnonymous()` explícito.
- **Casos de Uso**: los casos de uso son clases simples (`ICreateCustomerUseCase`/`CreateCustomerUseCase`) inyectadas directo en el endpoint. Menos indirección, cero dependencias extra, más fácil de seguir para una prueba de este tamaño.
- **Users**: entidad `User` propia (Id, Username, PasswordHash, CreatedAt) + hashing PBKDF2 hecho a mano con `Rfc2898DeriveBytes` (nativo de .NET, sin librerías externas). Identity trae mucho más de lo que este alcance necesita (tablas, roles, claims, UserManager/SignInManager).
- **Entidades de dominio sin validación de formato**: `Customer`/`User` solo asignan estado; los guard clauses de "requerido"/"formato" viven exclusivamente en Application vía FluentValidation. Evita duplicar la misma regla en dos capas.
- **Concurrencia real, no simulada**: `RowVersion` está mapeado directo al `xmin` de Postgres (columna de sistema), no a un shadow column inventado — el conflicto optimista lo resuelve la base de datos, no una comparación manual en código.
- **Duplicados con garantía de base de datos**: columnas generadas (`lower(TaxId)`, `lower(Email)`) + índice único real en Postgres, no solo un `SELECT` previo en Application (que por sí solo no cubre condiciones de carrera).
- **Máquina de estados data-driven**: la matriz de transiciones vive en una tabla (`CustomerStatusTransitions`), no en un `switch` en código — agregar/quitar una regla de negocio es un `UPDATE`, no un deploy.
- **Manejo de errores centralizado**: un único `IExceptionHandler` traduce todas las excepciones de negocio a `ProblemDetails`; ningún endpoint tiene `try/catch` propio.

**Frontend**
- **Fetch nativo**: por requisito explícito de la vacante ("Fetch" como skill obligatorio) — wrapper delgado propio (`httpClient.ts`) que centraliza el Bearer token y el parseo de `ProblemDetails`.
- **Formulario dinámico (`DynamicForm`)**: el alta/edición de cliente y los forms de auth se renderizan desde un arreglo de schema (`{name, label, type, required}`) + Zod, no como un formulario hardcodeado — reutilizable para cualquier entidad futura.
- **Mantine como UI kit**: velocidad de desarrollo (formularios, tablas, modales, notificaciones ya resueltos) sin escribir CSS desde cero para un CRUD.

**Testing**
- **Testcontainers para integración, no la base de dev**: cada corrida de `dotnet test` levanta un Postgres efímero propio — nunca ensucia (ni depende de) los datos que usas para probar manualmente con Swagger.

## Qué haría distinto con más tiempo

- **Autorización por roles**: hoy cualquier usuario autenticado puede hacer todo; con más tiempo agregaría roles/claims (ej. solo un rol "Admin" puede cambiar estado o eliminar).
- **Refresh tokens**: el JWT actual expira y no hay forma de renovarlo sin volver a hacer login — agregaría un endpoint de refresh.
- **Rate limiting en login**: no hay protección contra fuerza bruta en `/api/auth/login`.
- **CD, no solo CI**: el pipeline actual solo valida (build+test); agregaría un paso de deploy a un ambiente real (contenedor a un registry, o un servicio tipo Azure App Service).
- **Auditoría**: campos `UpdatedAt`/`UpdatedBy` en `Customer` para trazabilidad de cambios, no solo `CreatedAt`.
- **Validaciones de formato más estrictas**: el `TaxId` hoy solo valida longitud/requerido; con la definición real de RFC mexicano aplicaría su patrón exacto.
- **Secrets fuera de `appsettings.json`**: el secreto JWT y la connection string están en el repo por simplicidad de la prueba — en un entorno real irían a user-secrets/Key Vault/variables de entorno del pipeline.
- **Dockerizar también Api y frontend**: hoy `docker-compose.yml` solo levanta Postgres/Adminer; con más tiempo agregaría un `Dockerfile` por proyecto para un `docker compose up` que levante todo el stack de un solo comando.

## Tests

```bash
cd BackEnd
dotnet test
```

39 pruebas: dominio (sin mocks), casos de uso de Application (mockeados con NSubstitute) e integración de Infrastructure (Postgres real y efímero vía Testcontainers — requiere Docker corriendo, se levanta y destruye solo).

## CI/CD

`.github/workflows/ci.yml` corre en cada push/PR a `main`: build + test del backend, y lint + build del frontend.
