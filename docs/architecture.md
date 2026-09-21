# Architecture

## Status

This document describes the planned initial architecture of DevOps Platform Hub.

The project is currently in Phase 1: Application Foundation. The components and
technologies described here remain the accepted architectural direction and
planned implementation unless they are explicitly marked as implemented.

The initial ASP.NET Core Web API host, backend solution, .NET SDK policy, and
unit and integration test projects have been introduced during Phase 1. Shared
API error handling produces safe Problem Details responses for expected and
unexpected failures, while request logging records structured request context.
Local authentication uses PostgreSQL-backed users and roles, ASP.NET Core
password hashing, and JWT bearer tokens. Unit tests verify authentication
business behavior; integration tests verify the assembled HTTP flow against
the Flyway-created PostgreSQL schema.

The frontend foundation branch contains a standalone Angular application under
`ui/devops-platform-hub-ui`. Its root component composes a Material toolbar,
an application heading, and a main-content area containing the router outlet.
The route list is empty. Material supplies the Azure/Blue theme and components;
SCSS supplies application layout. ESLint checks TypeScript and templates, and
Prettier formats source files. This shell does not yet communicate with the API.

Feature modules, background execution, and product-operation endpoints remain
planned. The API exposes liveness and PostgreSQL readiness endpoints. The local
PostgreSQL and Flyway foundation is implemented: versioned SQL owns schema
changes, while Entity Framework Core performs runtime reads and writes for the
implemented identity data. .NET and EF Core migrations are not used.

This document will evolve as requirements and architectural assumptions are
validated through verified implementation evidence.

## Architectural Goals

The initial architecture should:

- Remain understandable and maintainable by one developer.
- Support incremental delivery without requiring paid cloud services.
- Separate business rules from HTTP, database, and external integration concerns.
- Keep feature boundaries clear as the application grows.
- Support automated testing of important business behavior.
- Protect secrets and operational data.
- Allow simulated builds and deployments before real integrations are introduced.
- Avoid distributed-system complexity until there is a demonstrated need.
- Allow external tools to be integrated without coupling the core domain to a specific vendor.

## System Context

DevOps Platform Hub is planned as a self-hostable application for developers,
platform engineers, technical leads, and administrators. It will provide a
central interface for registering software projects, defining deployment
environments, tracking build and deployment activity, and reviewing
operational history.

Users will interact with the platform through a web application. The platform
will expose an HTTP API used by that web application and, where appropriate,
future automated integrations. Access to operations and data will be governed
by authenticated identities and role-based permissions.

External systems may eventually include GitHub, GitHub Actions, Jenkins,
Docker, Kubernetes, cloud platforms, and observability tools. These systems
will remain responsible for their specialized work. For example, a CI system
will execute a real build, while DevOps Platform Hub will request the operation
and record its status, logs, and outcome.

During the early phases, builds, deployments, logs, and status changes will be
simulated locally. This allows the application's domain model and workflows to
be validated before external credentials, network dependencies, webhooks, and
provider-specific failure modes are introduced.

## Initial Architecture

The application will begin as a modular monolith. The backend will be one
deployable ASP.NET Core application containing feature-oriented modules with
explicit responsibilities. The Angular frontend will be a separate client
application that communicates with the backend through a REST API.

PostgreSQL is the planned relational database, accessed through Entity
Framework Core. A background worker hosted within the backend process will
initially coordinate simulated builds and deployments. This design does not
prevent a durable job processor or independently deployed worker from being
introduced later if reliability or scaling requirements justify that change.

The planned logical structure is:

```text
Users
  |
Angular web application
  |
ASP.NET Core REST API
  |
Application and domain modules
  |                 |
Entity Framework    Background execution
Core                and provider adapters
  |                 |
PostgreSQL          Simulated providers initially
```

The diagram represents planned logical responsibilities, not currently
implemented or independently deployed services.

## Repository Source Organization

The repository separates deployable applications, database delivery, and
documentation at the root. The backend solution and database files below are
the agreed starting structure. A folder is created when it contains a real
implementation; planned folders are not added as empty placeholders.

### .NET backend solution

```text
backend/DevOpsPlatformHub/
├── global.json                                  # .NET SDK selection
├── DevOpsPlatformHub.slnx                       # Backend project list
├── DevOpsPlatformHub.Api/                       # Deployable HTTP host
│   ├── Constants/                               # API-only stable values
│   ├── Contracts/                               # HTTP request and response types
│   ├── Controllers/                             # Route-level endpoints
│   ├── ErrorHandling/                           # Problem Details translation
│   ├── Extension/                               # DI and pipeline registration
│   ├── Logging/                                 # HTTP request logging
│   ├── Properties/                              # Local launch profiles
│   ├── Program.cs                               # Host entry point
│   ├── appsettings.json                         # Tracked non-secret defaults
│   ├── appsettings.Development.json             # Safe local overrides
│   └── DevOpsPlatformHub.Api.csproj             # API dependencies and references
├── DevOpsPlatformHub.Application/               # Use-case contracts and rules
│   ├── <FeatureName>/                           # One real backend capability
│   │   ├── Contracts/                           # Use-case models and interfaces
│   │   └── Validations/                         # Feature request validation
│   ├── Exceptions/                              # Expected application outcomes
│   └── DevOpsPlatformHub.Application.csproj     # Application dependencies
├── DevOpsPlatformHub.Domain/                    # Technology-independent business model
│   ├── Constants/                               # Domain vocabulary
│   ├── Entities/                                # Business entities
│   └── DevOpsPlatformHub.Domain.csproj          # Domain dependencies
├── DevOpsPlatformHub.Infrastructure/            # Technical implementations
│   ├── <FeatureName>/                           # Feature technical adapters
│   ├── HealthChecks/                            # Infrastructure health checks
│   ├── Logging/                                 # Reusable log safety utilities
│   ├── Persistence/                             # EF Core runtime data access
│   │   ├── Configurations/                      # Entity-to-table mappings
│   │   └── Repositories/                        # Feature persistence implementations
│   └── DevOpsPlatformHub.Infrastructure.csproj  # Infrastructure dependencies
├── DevOpsPlatformHub.UnitTests/                 # Isolated behavior tests
│   ├── <FeatureName>/                           # Unit tests for one capability
│   │   ├── Support/                             # Feature-only fakes
│   │   └── <BehaviorGroup>/                     # Tests grouped when needed
│   └── DevOpsPlatformHub.UnitTests.csproj       # Unit-test dependencies
└── DevOpsPlatformHub.IntegrationTests/          # Assembled API behavior tests
    ├── <FeatureName>/                           # Endpoint and feature tests
    │   ├── Endpoints/                           # HTTP endpoint tests
    │   └── Support/                             # Feature test-host utilities
    ├── ErrorHandling/                           # Error pipeline behavior
    ├── HealthChecks/                            # Liveness and readiness behavior
    ├── Startup/                                 # Host startup behavior
    ├── Support/                                 # Shared integration utilities
    └── DevOpsPlatformHub.IntegrationTests.csproj # Integration-test dependencies
```

#### Solution files

- `global.json` selects the .NET SDK feature band for developer machines and CI.
- `DevOpsPlatformHub.slnx` lists every backend project used by restore, build,
  and test commands. It contains no application behavior.
- Each `*.csproj` defines one project's target framework, package dependencies,
  and permitted project references. References express the dependency flow
  `Api -> Infrastructure -> Application -> Domain`.

#### `DevOpsPlatformHub.Api`

This is the deployable ASP.NET Core HTTP host. It owns HTTP-only concerns and
must not contain business rules or database queries.

- `Program.cs` builds the host and applies the registered pipeline.
- `appsettings.json` holds tracked non-secret defaults such as logging, JWT
  issuer, and audience.
- `appsettings.Development.json` holds safe development-only overrides. Local
  connection strings and signing keys remain in User Secrets.
- `Constants` contains API-only stable values such as route segments.
- `Contracts` contains HTTP request or response types that do not belong in
  Application.
- `Controllers` contains thin route-level endpoints.
- `ErrorHandling` contains global exception-to-Problem-Details translation.
- `Extension` contains focused service-registration and request-pipeline
  extension methods so `Program.cs` remains readable.
- `Logging` contains HTTP request logging. It must never log request bodies,
  passwords, tokens, or connection strings.
- `Properties/launchSettings.json` contains local launch profiles, not
  deployment configuration.

#### `DevOpsPlatformHub.Application`

This project contains feature contracts, use-case validation, and expected
application exceptions. It must not depend on API, EF Core, PostgreSQL, or
Angular.

- `<FeatureName>/Contracts` contains feature request/result models and
  interfaces implemented by Infrastructure.
- `<FeatureName>/Validations` contains validation that protects the use case
  regardless of its caller.
- `Exceptions` contains expected validation, missing-resource, and conflict
  outcomes for the API to translate safely.

#### `DevOpsPlatformHub.Domain`

This is the innermost project. It contains business concepts independent of
database and HTTP technology.

- `Entities` contains domain entities.
- `Constants` contains stable domain vocabulary, such as role names. It must
  not contain route or UI values.

#### `DevOpsPlatformHub.Infrastructure`

This project implements Application contracts using technical concerns such as
PostgreSQL, password hashing, JWT creation, health checks, and log sanitizing.

- `<FeatureName>` contains feature-specific technical implementations that do
  not expose HTTP endpoints.
- `HealthChecks` contains checks that depend on technical infrastructure.
- `Logging` contains reusable technical log sanitization and safe exception
  detail handling.
- `Persistence/PlatformDbContext.cs` is the EF Core runtime mapping entry
  point. It reads and writes the Flyway-owned schema; it never creates it.
- `Persistence/Configurations` contains EF Core entity-to-table mappings.
- `Persistence/Repositories` contains feature-specific persistence
  implementations. Generic repositories and a generic unit of work remain out
  of scope until a demonstrated need exists.

#### Test projects

- `DevOpsPlatformHub.UnitTests` contains fast isolated behavior tests using
  fakes or in-memory collaborators; it does not need PostgreSQL.
- `UnitTests/<FeatureName>` groups tests by feature. `Support` contains
  feature-only fakes; subfolders such as `Services` or `Tokens` are created
  only when several tests need them.
- `DevOpsPlatformHub.IntegrationTests` contains assembled HTTP tests using the
  API host and real infrastructure configuration.
- `IntegrationTests/<FeatureName>` contains endpoint and feature-support tests.
  `ErrorHandling`, `HealthChecks`, and `Startup` group cross-cutting observable
  API behavior. Shared test-host utilities belong in `Support`.

For every upcoming backend capability, use these placement rules only when the
related code is needed:

- Put HTTP routes and HTTP-only request or response types in `Api`.
- Put feature contracts, use-case validation, and expected application outcomes
  in `Application/<FeatureName>`.
- Put business entities and domain vocabulary in `Domain`.
- Put database mappings, repository implementations, token providers, health
  checks, and other technical adapters in `Infrastructure`.
- Put unit and integration tests under the matching behavior category and
  feature name.

The folder structure is a placement rule, not a reason to create empty folders
or copy an existing feature as a template without a requirement.

### Database delivery

```text
database/
├── flyway.conf                                    # Non-secret Flyway configuration
├── migrations/                                    # All deployable database changes
│   ├── versioned/                                 # Immutable one-time changes
│   │   └── VYYYYMMDDHHMMSSffffff__description.sql # Ordered schema or reference-data change
│   └── repeatable/                                # Create only for changing definitions
│       └── R__description.sql                     # Procedure, function, or view definition
└── tests/                                         # Database-level verification SQL
    └── postgresql_relational_smoke.sql            # Relational-constraint smoke test
```

#### Database configuration

- `flyway.conf` is tracked non-secret Flyway configuration. It declares
  migration locations and retry policy; credentials come from Compose, CI, or
  another runtime environment.

#### Versioned migrations

- `migrations/versioned` contains immutable, ordered schema or reference-data
  changes. Each migration runs once and Flyway records its checksum.
- `VYYYYMMDDHHMMSSffffff__description.sql` is the required filename pattern.
  Its timestamp provides ordering and its description explains the change.
- Never edit a migration already applied outside a disposable local database.
  Add a new migration to change the schema instead.

#### Repeatable migrations

- `migrations/repeatable` is created only when the application needs a
  stored procedure, function, view, or another definition that should be
  reapplied after its content changes.
- `R__description.sql` is the repeatable filename pattern. Flyway reruns it
  when its checksum changes.
- The Flyway configuration gains the repeatable location when this directory
  is first introduced. Do not create a separate procedure directory that
  Flyway does not execute.

#### Database verification

- `tests` contains database-level verification SQL. Tests must roll back
  temporary data and verify outcomes instead of duplicating one test file per
  migration.
- `postgresql_relational_smoke.sql` is the current relational-constraint and
  rollback smoke test. Local verification and database CI run it after Flyway.

Flyway owns all production DDL. EF Core owns only runtime reads and writes.
Therefore, this project does not add EF Core migration files, `EnsureCreated`,
or schema-creation code to the backend.

When a real stored procedure, function, or view is required, define it as a
Flyway migration so deployment remains reproducible. A changing definition is
normally a repeatable migration named `R__description.sql`; the Flyway location
configuration will be extended at that time. Do not maintain a separate
stored-procedure directory that Flyway does not execute.

### Angular source organization

The Angular client uses feature-oriented source organization with `core` for
cross-cutting application infrastructure and `shared` for reusable
presentational components. Feature folders are added only when their product
area begins; they are not placeholders for planned work.

```text
ui/devops-platform-hub-ui/src/app/
├── core/                                         # Application-wide technical behavior
│   ├── guards/                                   # Router navigation decisions
│   ├── interceptors/                             # HttpClient request and response behavior
│   ├── services/                                 # Global singleton technical services
│   └── utils/                                    # Stateless cross-feature helpers
├── shared/                                       # UI shared by multiple features
├── features/                                     # Product capabilities
│   └── <feature-name>/                           # One product capability
│       ├── <routed-component>/                   # CLI-generated route component files
│       ├── <feature-name>.models.ts              # Feature interfaces and types
│       ├── <feature-name>.service.ts             # Feature API and state behavior
│       └── <feature-name>.routes.ts              # Feature route definitions
├── app.config.ts                                 # Application-wide Angular providers
├── app.routes.ts                                 # Root route composition
├── app.ts                                        # Root application component
├── app.html                                      # Root application layout
└── app.scss                                      # Root application layout styles
```

For every upcoming Angular feature, put route-level components, feature models,
feature services, and feature routes in its own `features/<feature-name>`
directory. Angular CLI-generated components keep their own folders because each
component has TypeScript, HTML, SCSS, and test files. Other feature files remain
directly under their feature directory until several related files justify a
subfolder. `core` contains application-wide technical behavior and `shared`
contains reusable presentational components; neither should contain
feature-specific business UI.

## Backend Modules

### Identity and Access

Owns user and role concepts. The implemented local flow supports self-
registration with the `User` role, login by username or email, password-hash
verification, JWT issuance, and bearer-token enforcement on the current-user
endpoint. Authorization will continue to be enforced by the backend when
future operations require role-specific permissions.

### Projects

Manages registered software projects and repository metadata. It will describe
the software being operated without storing source-control credentials as
ordinary project data.

### Environments

Manages logical deployment targets such as Development, Staging, and
Production. It will store environment-specific metadata while keeping secrets
outside normal domain records and API responses.

### Builds

Records build requests, status transitions, timestamps, logs, and outcomes. It
will begin with simulated execution and later coordinate with supported CI
providers through explicit integration boundaries.

### Deployments

Records the promotion or deployment of a build to an environment. It will own
deployment status transitions and the history needed to determine what version
was deployed to an environment and when.

### Audit

Records security-relevant and operationally important actions, including the
actor, action, affected resource, timestamp, and outcome where appropriate.
Audit records are historical evidence and must not be used as a replacement for
application logs.

### Notifications

Creates notification requests for events such as build or deployment failures.
Initial behavior may be limited to recording notifications. Email, chat, or
other delivery providers will be introduced only when a concrete requirement
justifies them.

## Component Responsibilities

### Angular frontend

- Presents project, environment, build, deployment, and administration views.
- Collects user input and performs client-side usability validation.
- Calls the backend API and displays returned state.
- Does not enforce security by itself or connect directly to the database or
  external infrastructure providers.

### ASP.NET Core API

- Exposes authenticated HTTP endpoints.
- Validates request shape and translates HTTP concerns into application
  operations.
- Applies authorization policies before protected operations are executed.
- Returns consistent HTTP responses without exposing internal exceptions,
  credentials, or sensitive configuration.
- Maps application failures to documented HTTP Problem Details responses.
- Logs request method, route path, status code, duration, and trace ID without
  logging request bodies, headers, query strings, or response bodies.
- Exposes `/health/live` for process liveness and `/health/ready` for
  PostgreSQL readiness. Health probes are excluded from request-access logs.

The API composition root registers the PostgreSQL readiness implementation. The
implementation remains in Infrastructure because it depends on Npgsql and
executes a database command. Missing or unavailable PostgreSQL is converted to
a controlled readiness result rather than an unhandled API exception.

Controllers should remain thin. They may coordinate request and response
concerns but should not contain business rules.

### Application and domain modules

- Implement use cases and business rules.
- Validate allowed state transitions.
- Define boundaries for persistence and external providers where needed.
- Remain testable without depending on Angular or controller behavior.

Modules may cooperate through explicit application contracts. They should not
reach into another module's internal persistence implementation or modify its
data directly.

### Persistence

Entity Framework Core will map domain data to PostgreSQL for application
reads and writes. Flyway owns versioned SQL schema changes from the repository
root `database/` directory; .NET and EF Core migrations are not used. Database
access should remain behind module-owned application boundaries. A generic
repository abstraction will not be introduced initially because EF Core already
provides unit-of-work and collection-like persistence behavior. More specialized
abstractions may be added when they protect a real domain boundary or improve
testability.

### Background execution

Long-running builds and deployments must not execute inside the lifetime of an
HTTP request. The API will accept a valid request, persist the initial execution
state, and allow background processing to advance it.

The first implementation may use an ASP.NET Core hosted service and simulated
providers. This is suitable for learning and local workflows but does not
guarantee that queued work survives a process crash. Durable job storage will
be considered when that reliability requirement becomes necessary.

### Provider adapters

Provider adapters will translate application operations into calls understood
by external systems. Core build and deployment workflows should depend on
application-defined contracts rather than GitHub, Jenkins, Docker, Kubernetes,
or cloud SDK types.

This boundary allows a simulated provider and future real providers to support
the same application workflow without pretending that every external platform
has identical capabilities.

## Request Flow

A typical synchronous request will follow this path:

1. A user performs an action in the Angular application.
2. Angular sends an HTTP request to the ASP.NET Core API.
3. The API authenticates the caller and checks the required permission.
4. The endpoint validates the request and invokes the appropriate application
   use case.
5. The owning module applies business rules and reads or changes state through
   its persistence boundary.
6. The transaction is committed when the operation succeeds.
7. The API returns an appropriate response for Angular to display.

Validation failures, missing resources, conflicts, and unexpected errors use a
shared Problem Details response contract. Each response includes a safe error
code and trace ID. Internal exception details and sensitive values must not be
returned to clients.

## Background Execution Flow

A simulated build or deployment will follow an asynchronous workflow:

1. An authorized user submits an execution request.
2. The API validates the request and stores an initial queued record.
3. The API returns without waiting for the execution to finish.
4. A background worker selects eligible queued work.
5. The owning module moves the execution through allowed status transitions.
6. A simulated provider produces progress, logs, and a final outcome.
7. Execution state and logs are persisted as processing continues.
8. The frontend retrieves status through ordinary API requests initially.

Real-time delivery with SignalR is deferred. Polling is sufficient until live
updates provide a demonstrated usability benefit.

## Dependency Rules

- The Angular frontend communicates with the backend API only.
- The frontend must not connect directly to PostgreSQL or infrastructure tools.
- API endpoints depend on application use cases rather than database details.
- Business rules belong in their owning modules, not controllers or Angular
  components.
- Modules communicate through explicit contracts and must not access another
  module's internal database implementation directly.
- Infrastructure code may implement contracts defined by the application, but
  domain behavior must not depend on provider SDKs.
- Infrastructure may provide technical health checks and log-sanitization
  utilities used by the API composition root. Those utilities must not contain
  domain or product behavior.
- External provider responses must be translated into application-owned models.
- Cross-module dependencies must be intentional and should not form cycles.
- Shared code should contain genuinely common technical behavior, not become a
  collection of unrelated business logic.

These rules are architectural constraints. Their enforcement may begin through
code review and tests and become more automated as the project structure is
implemented.

## Security Boundaries

The browser, API, database, background worker, and every external provider are
separate trust boundaries.

- All protected API operations require server-side authentication and
  authorization.
- Client-side validation and hidden UI controls are usability features, not
  security controls.
- Input from users, webhooks, provider APIs, and stored integration data is
  untrusted and must be validated.
- Secrets must not be committed, returned in API responses, or included in
  application, execution, or audit logs.
- Integration credentials will use least privilege and remain separate from
  ordinary project and environment metadata.
- Sensitive values must be protected in transit and must not be exposed through
  diagnostic endpoints.
- Operationally important state-changing actions should create audit records.
- Simulated execution will not accept arbitrary shell commands from users.

Detailed authentication, secret storage, retention, and threat-model decisions
will be documented when their implementation work begins.

## Deferred Decisions

The following decisions are intentionally deferred until requirements justify
them:

- Durable background job processing and separate worker deployment.
- SignalR or another real-time update mechanism.
- GitHub, GitHub Actions, Jenkins, and webhook integration details.
- Docker daemon and Kubernetes cluster connectivity.
- Cloud provider and infrastructure-as-code integration.
- Metrics, distributed tracing, and external log aggregation.
- Notification delivery providers.
- Enterprise single sign-on.
- Secrets-management provider.
- Service extraction from the modular monolith.

Deferral means that these technologies are not implemented or guaranteed. Each
will require a concrete use case, security review, acceptance criteria, and an
architecture decision when appropriate.

## Known Trade-offs

### Modular monolith

A modular monolith reduces deployment, networking, and local-development
complexity. However, its module boundaries are easier to violate than network
boundaries and therefore require disciplined structure, review, and testing.

### Shared relational database

A single PostgreSQL database simplifies transactions, migrations, backup, and
local development. It also creates a risk that modules become coupled through
tables. Module-owned persistence boundaries will be used to limit that risk.

### Hosted background service

An in-process hosted service is simple and appropriate for initial simulated
workflows. It does not by itself provide durable execution, independent scaling,
or strong recovery after process failure.

### REST and initial polling

REST endpoints and polling are straightforward to implement, test, and debug.
They may be less efficient or responsive than real-time updates for frequently
changing execution logs. SignalR will be considered only after the basic
workflow is working and measured.

### Provider abstraction

Provider contracts reduce direct vendor coupling and make simulation possible.
An overly generic abstraction could hide important differences between
providers, so contracts will be based on actual application use cases rather
than attempts to model every CI/CD platform in advance.
