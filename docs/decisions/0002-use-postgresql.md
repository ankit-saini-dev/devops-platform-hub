# ADR 0002: Use PostgreSQL as the Initial Database

## Status

Accepted

## Context

DevOps Platform Hub needs relational persistence for users, projects,
environments, builds, deployments, audit records, notifications, and their
relationships. The data includes business rules and state transitions that
benefit from transactions, constraints, indexes, and a consistent query model.

The initial database must work locally without paid services, integrate well
with ASP.NET Core and Entity Framework Core, and remain suitable for automated
integration testing. It should also support container-based development when
Docker Compose is introduced.

The developer already has professional SQL Server and Entity Framework Core
experience. Reusing SQL Server would reduce the initial learning curve, but the
project is also intended to expand practical experience with technologies that
are common in cloud-native and cross-platform environments.

The application will begin as a modular monolith. A single relational database
is therefore operationally simpler than assigning separate databases to
logical modules. Module ownership rules are still needed to prevent unrelated
features from becoming coupled through direct table access.

## Decision

DevOps Platform Hub will use PostgreSQL as its initial relational database.

The ASP.NET Core backend will access PostgreSQL through Entity Framework Core
and its PostgreSQL provider. Database schema changes will be managed through
version-controlled Entity Framework Core migrations.

The initial modular monolith will use one PostgreSQL database. Modules will own
their persistence behavior and must not depend directly on another module's
internal tables merely because the tables share a database. The physical
schema organization and exact enforcement mechanism will be decided when the
persistence structure is implemented.

PostgreSQL will initially run as a local development dependency. Docker Compose
is the planned method for providing a reproducible local database after Docker
is introduced in Phase 1. Credentials and connection strings containing
secrets will be supplied through configuration and will not be committed to
source control.

Production hosting, backup, high availability, replication, and managed
database services are not selected by this decision. Those choices depend on a
future deployment environment and its demonstrated reliability requirements.

## Alternatives Considered

### SQL Server

SQL Server provides mature relational capabilities, strong tooling, and
first-class support in the .NET ecosystem. Existing developer experience would
make initial setup and troubleshooting faster. SQL Server would be a reasonable
choice for this application.

It was not selected because PostgreSQL provides a free, cross-platform local
environment and creates a useful opportunity to deepen experience beyond an
already familiar database. PostgreSQL is also commonly available through
container platforms and managed cloud services without making the early project
dependent on a particular vendor.

This decision is not based on SQL Server being technically unsuitable. A future
requirement for SQL Server-specific integration, tooling, or organizational
standards could justify reconsideration.

### SQLite

SQLite would provide the simplest local setup because it can store the database
in a file without running a separate server. It is useful for prototypes,
desktop applications, and some automated tests.

It was not selected as the primary database because its concurrency behavior,
type system, migration behavior, and operational model differ from a
client-server relational database. Using SQLite during development could allow
tests to pass while hiding behavior that would fail with the eventual deployed
database.

SQLite may still be appropriate for isolated tests that do not claim to verify
PostgreSQL-specific relational behavior. Integration tests for persistence must
use PostgreSQL-compatible behavior.

### Database per Module

Giving each logical module a separate database could enforce stronger data
ownership and prepare the system for independent services.

It was not selected because the modules are not independently deployable
services and currently share one application lifecycle. Multiple databases
would complicate local setup, transactions, migrations, backups, and testing
without a demonstrated isolation requirement.

If a module is later extracted into a service, its data ownership and migration
strategy must be designed as part of that extraction rather than assumed in
advance.

## Consequences

### Positive

- PostgreSQL is open source and can be used locally without database licensing
  fees.
- It runs across common development, container, and cloud environments.
- Entity Framework Core has a maintained PostgreSQL provider.
- Relational constraints and transactions support the planned domain workflows.
- Docker can provide a repeatable database version and local setup.
- The choice broadens practical database experience while retaining familiar
  relational and Entity Framework Core concepts.
- PostgreSQL-compatible integration tests can exercise behavior close to the
  planned runtime database.

### Negative

- The developer must learn PostgreSQL-specific administration, tooling, data
  types, SQL behavior, and troubleshooting.
- Running a database server adds more local setup than using SQLite.
- Some SQL Server knowledge and scripts will not transfer directly.
- PostgreSQL behavior can still differ between local containers and a future
  managed service because of versions, extensions, configuration, and network
  conditions.
- A shared database can weaken module boundaries if features access each
  other's tables directly.
- Database migrations require review and coordination because the backend is
  deployed as one application.

## Implementation Constraints

- The PostgreSQL version must be explicitly selected and documented when the
  database container is introduced.
- Development configuration may contain non-secret defaults, but real passwords
  and complete secret-bearing connection strings must not be committed.
- `.env.example` must contain placeholders only when environment-based database
  configuration is added.
- Integration tests must not rely solely on Entity Framework Core's in-memory
  provider when validating relational behavior.
- Migrations must be version controlled and reviewed with the application
  changes that require them.
- Database constraints should protect important invariants where appropriate;
  application validation alone is not sufficient for data integrity.
- Queries returning potentially large histories must support bounded results or
  pagination when those features are implemented.
- Backup and restore procedures must be documented before any environment is
  treated as holding important persistent data.

## Reconsider When

This decision should be reviewed when evidence shows that PostgreSQL no longer
meets the application's product, operational, or organizational requirements.
Relevant signals include:

- A required integration depends on SQL Server-specific capabilities or
  organizational standards.
- A target deployment environment cannot support PostgreSQL responsibly.
- Measured workload characteristics require a substantially different storage
  model.
- A capability has data with access patterns that are poorly served by a
  relational database and a separate storage technology has a clear benefit.
- A module is extracted into an independently deployed service and requires
  separate data ownership.
- Operational evidence shows that the selected PostgreSQL topology cannot meet
  required availability, recovery, security, or performance objectives.

Reconsideration does not imply replacing PostgreSQL for every module. A future
architecture may use more than one storage technology when each addition has a
specific, tested, and documented purpose.
