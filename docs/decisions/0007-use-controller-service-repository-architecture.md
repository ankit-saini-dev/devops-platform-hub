# ADR 0007: Use Controller-Service-Repository Architecture

## Status

Accepted

## Context

DevOps Platform Hub remains a modular monolith operated by one developer. The
previous `Api`, `Application`, `Domain`, and `Infrastructure` project names
introduced architectural terminology before the product had enough modules to
justify those boundaries. Authentication behavior was especially difficult to
follow because controller logic, service orchestration, persistence, and
entities were spread across those layers.

The backend needs a structure that makes the normal request path easy to trace
while retaining separation between HTTP behavior, business workflows,
database access, and entity definitions.

## Decision

Use a controller-service-repository architecture within the existing modular
monolith.

- `DevOpsPlatformHub.Api` hosts the application and contains controllers,
  middleware, HTTP error translation, health-endpoint registration, and
  dependency-injection composition.
- `DevOpsPlatformHub.Application` contains feature workflows, service contracts,
  validation, expected business exceptions, password hashing, and JWT issuance.
- `DevOpsPlatformHub.DataAccess` contains EF Core, PostgreSQL configuration,
  repository contracts and implementations, database-specific failures, and
  readiness checks.
- `DevOpsPlatformHub.Contexts` contains `PlatformDbContext` and EF Core entity
  mappings for the Flyway-owned PostgreSQL schema.
- `DevOpsPlatformHub.Core` contains cross-cutting constants and reusable
  log-sanitization utilities.
- `DevOpsPlatformHub.Entities` contains business entities.
- Unit and integration test projects remain separate and organize tests by
  observable feature behavior.

The normal runtime flow is:

```text
Controller -> Application service -> Repository -> DbContext -> Entity -> PostgreSQL
```

The API project also references DataAccess so that it can register concrete
repository implementations in the dependency-injection container. Flyway
continues to own every database schema change under the repository-root
`database/` directory.

This decision replaces the code-organization portion of ADR 0001. ADR 0001's
decision to remain a modular monolith is unchanged.

## Consequences

Feature code is easier to locate: start at a controller, follow its service,
then inspect the repository that performs database work. The project names
state their responsibilities directly, which is appropriate for the current
team size and learning goals.

Services depend on repository contracts, and repositories contain only data
access behavior. This requires discipline to keep controllers thin and avoid
placing business rules in repositories. If the application later gains many
independently owned modules with complex cross-module rules, the boundaries can
be reconsidered using evidence rather than adopting a more abstract structure
prematurely.
