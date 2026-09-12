# ADR 0005: Use Flyway for Database Schema Delivery

## Status

Accepted

## Context

DevOps Platform Hub uses PostgreSQL and needs a repeatable way to create and
evolve its database schema in local development and CI. The application will
use Entity Framework Core for future application reads and writes, but schema
definition is also an opportunity to keep SQL visible, reviewable, and usable
without the .NET application.

Using Entity Framework Core migrations would couple database definition and
deployment to the backend runtime. The project instead needs a database
delivery boundary that can run before, after, or independently of the API and
frontend. That boundary must work in local Docker Compose and in CI.

## Decision

Flyway Community Edition manages PostgreSQL schema changes from the repository
root `database/` directory.

- `database/migrations/versioned/` contains immutable versioned SQL migrations.
- Future views and stored procedures use repeatable Flyway scripts only when
  the application needs them.
- `database/flyway.conf` contains non-secret Flyway configuration.
- Credentials come from ignored local configuration or CI environment values;
  they are never stored in a migration or tracked configuration file.
- CI starts a disposable PostgreSQL instance, runs `flyway migrate` and
  `flyway validate`, then executes relational SQL smoke tests.
- EF Core migrations and .NET DDL commands are prohibited for this project.

## Consequences

The database can be created and verified without starting the API. SQL changes
are explicit, reviewable, and usable by database tooling. CI validates both
migration history and the resulting relational behavior.

Developers must maintain migration ordering and treat committed versioned
migrations as immutable. Backend changes that require schema changes must ship
with the corresponding Flyway migration and relational verification.
