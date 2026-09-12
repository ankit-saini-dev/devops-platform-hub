# Development Setup

## Status

DevOps Platform Hub is currently in Phase 1: Application Foundation. The
backend host, modular backend project structure, startup smoke test, Angular
Material frontend foundation, and initial backend/frontend CI checks are in
place.

PostgreSQL 18.6 now runs locally through Docker Compose. Flyway applies and
validates versioned SQL migrations, and a relational smoke test verifies the
resulting database state. Authentication, health endpoints, runtime persistence
behavior, and product modules remain planned. The Database CI job is configured
on the current feature branch but is not described as verified until its pull
request run succeeds.

## Supported Development Environment

The project should remain usable on a developer workstation without paid cloud
services during its early phases. Windows with PowerShell is the initial
development environment, while source code and containerized dependencies
should remain cross-platform where practical.

Commands in this document must identify the shell they target. Platform-specific
instructions should be separated clearly rather than assumed to work
everywhere.

## Current Prerequisites

The implemented backend foundation currently requires:

- Git for version control.
- .NET SDK `10.0.400` or a compatible later patch in the same SDK feature band
  for the ASP.NET Core backend.
- A code editor or IDE with C#, TypeScript, and EditorConfig support.
- Docker Desktop with Docker Compose v2 for the local PostgreSQL and Flyway
  workflow.

The backend targets `net10.0`. The backend `global.json` selects SDK
`10.0.400`, permits compatible updates through the `latestPatch` roll-forward
policy, and rejects preview SDKs.

The following tools are also required when working on the frontend:

- Node.js and npm, as described below.

Exact versions for planned tools will be selected and verified from official
documentation when their implementing issue begins.

## Repository Setup

The current repository setup flow is:

1. Clone the repository.
2. Checkout the intended branch.
3. Change to the backend solution directory.
4. Confirm the selected .NET SDK.
5. Restore and build the backend solution.
6. Run the automated backend tests.
7. Start the API when manual runtime verification is required.

For the frontend, follow the independent setup below. For PostgreSQL and
versioned database migrations, follow the local database workflow below.

## Frontend Setup

The frontend lives in `ui/devops-platform-hub-ui`. The current machine reports
Node.js `24.21.0` and npm `11.19.0`; installed Angular is `22.1.6`, Angular CLI
is `22.1.7`, Angular Material/CDK are `22.1.6`, and TypeScript is `6.0.3`.
The package lock records the resolved dependency tree. `package.json` supports
Node.js 24 and npm 11, while `packageManager` records npm 11.19.0 as the package
manager version used for this foundation.

Run in PowerShell from the repository root, stopping if a command fails:

```powershell
cd ui\devops-platform-hub-ui
npm.cmd ci
npm.cmd run build
npm.cmd test -- --watch=false
npm.cmd run format:check
npm.cmd start
```

Open `http://localhost:4200/` to inspect the shell. Stop the server with `Ctrl+C`.
The shell does not request Google Fonts or Material Icons. An install-script
policy warning from npm should be evaluated separately from installation, build,
or test errors.

Local verification passed the source lint/format check, production build, and
two component tests. Browser inspection at 1280px and 375px widths showed no
horizontal overflow or captured console warnings/errors. The maintainer also
completed a clean `npm ci` installation with zero reported vulnerabilities.
These are local results; no fresh repository checkout or frontend CI run is
claimed. See the [frontend README](../ui/devops-platform-hub-ui/README.md) for
lint and format behavior and source-control boundaries.

## Backend Structure

```text
backend/
`-- DevOpsPlatformHub/
    |-- global.json
    |-- DevOpsPlatformHub.slnx
    |-- DevOpsPlatformHub.Api/
    |-- DevOpsPlatformHub.Application/
    |-- DevOpsPlatformHub.Domain/
    |-- DevOpsPlatformHub.Infrastructure/
    |-- DevOpsPlatformHub.UnitTests/
    `-- DevOpsPlatformHub.IntegrationTests/
```

- `global.json` selects the supported .NET SDK feature band.
- `DevOpsPlatformHub.slnx` groups the backend projects for restore, build, and
  test commands.
- `DevOpsPlatformHub.Api` is the deployable ASP.NET Core Web API host.
- `DevOpsPlatformHub.Application` will contain use cases and application
  contracts.
- `DevOpsPlatformHub.Domain` will contain entities, value objects, and domain
  rules.
- `DevOpsPlatformHub.Infrastructure` contains runtime technical integrations,
  including the selected PostgreSQL EF Core provider for future data access.
- `DevOpsPlatformHub.UnitTests` is reserved for isolated business-rule tests.
- `DevOpsPlatformHub.IntegrationTests` is reserved for assembled API and
  infrastructure tests and currently contains the startup smoke test.

The unit-test project currently contains no tests because no business rules have
been introduced. The integration-test project contains one startup smoke test.
It starts the assembled API in memory with `WebApplicationFactory<Program>` and
verifies that `/openapi/v1.json` returns HTTP `200` with the `application/json`
media type.

## Backend Commands

Run these commands in Windows PowerShell from the repository root:

```powershell
cd backend\DevOpsPlatformHub
dotnet --version
dotnet restore DevOpsPlatformHub.slnx
dotnet build DevOpsPlatformHub.slnx --no-restore
dotnet test DevOpsPlatformHub.slnx --no-build --no-restore
dotnet run --project DevOpsPlatformHub.Api
```

The test command assumes the preceding build completed successfully. The
`--no-build` and `--no-restore` options prevent the command from repeating work
already completed in this documented sequence.

The expected SDK version is `10.0.400`. The verified HTTP launch profile listens
on `http://localhost:5164` in the Development environment. While the API is
running, the development OpenAPI document is available at:

```text
http://localhost:5164/openapi/v1.json
```

Press `Ctrl+C` to stop an API started in PowerShell. Use the red stop button to
stop an API started through Rider.

The generated HTTPS launch profile also defines `https://localhost:7180`, but
local HTTPS certificate trust has not yet been verified. Do not report the HTTPS
profile as working until that verification is performed.

The startup smoke test uses ASP.NET Core's in-memory test server, so it does not
open port `5164` or verify Kestrel, TLS certificates, or a deployed environment.
Those concerns require separate runtime and deployment verification.

## Local PostgreSQL and Flyway

The root `docker-compose.yaml` starts PostgreSQL 18.6 and exposes it on the
port defined by the ignored root `.env` file. Copy `.env.example` to `.env`,
choose a local password, and do not commit `.env`.

Run these commands in Windows PowerShell from the repository root:

```powershell
Copy-Item .env.example .env
docker compose up -d
docker compose ps

docker compose --profile database-tools run --rm flyway migrate
docker compose --profile database-tools run --rm flyway validate

Get-Content database\tests\postgresql_relational_smoke.sql |
  docker compose exec -T postgres psql -U devops_platform -d devops_platform_hub
```

The Compose health check waits until PostgreSQL accepts connections. The
`database-tools` profile keeps Flyway from starting during ordinary
`docker compose up -d` usage. `migrate` applies immutable versioned SQL files,
`validate` checks their recorded checksums, and the relational smoke test
verifies the resulting schema before rolling back its temporary test data.

If a disposable local database must be recreated after changing its initial
database name, username, or password, run `docker compose down -v` and then
`docker compose up -d`. This removes the local PostgreSQL volume and all its
data.

The API does not yet connect to PostgreSQL because no application entity or
runtime persistence use case exists. When one is introduced, keep its local
connection string in .NET user secrets or environment variables, never in a
tracked settings file.

## Configuration and Secrets

- Real secrets must never be committed.
- `.env.example` contains placeholders only; `.env` is local and ignored.
- Developer-specific configuration files must remain ignored unless they are
  explicitly safe examples.
- Logs, screenshots, test output, and issue reports must be reviewed for
  secrets before sharing.
- Integration tokens must use least privilege and remain separate from ordinary
  project and environment metadata.

## Verification Standards

Setup instructions are considered verified only when a clean or appropriately
controlled development environment can follow them successfully.

For each introduced component, verification should eventually cover:

- Tool and runtime version output.
- Dependency restoration.
- Build completion.
- Automated tests.
- Database startup and connectivity where applicable.
- Migration application where applicable.
- Backend startup and HTTP responses; health responses when their implementing
  issue is completed.
- Frontend startup and browser access.
- Clear failure guidance for common setup problems.

Command output should be summarized truthfully. A planned command must not be
reported as successful before it has been executed.

## Current Verification

The current backend foundation has been verified on Windows with PowerShell:

- `global.json` resolves .NET SDK `10.0.400` and disables preview SDKs.
- Backend package restoration completes successfully.
- The API, unit-test project, and integration-test project build with no
  warnings or errors.
- The API starts on `http://localhost:5164`.
- `GET /openapi/v1.json` returns HTTP `200` with an OpenAPI `3.1.1` JSON
  document.
- The OpenAPI document contains no product paths because no product endpoint is
  implemented.
- Stopping the API releases the HTTP port.
- `dotnet test DevOpsPlatformHub.slnx --no-build --no-restore` completes
  successfully.
- The integration suite runs one startup smoke test, which starts the API in
  memory and verifies `/openapi/v1.json` returns HTTP `200` with the
  `application/json` media type.
- The unit-test project reports that no tests are available because business
  logic has not yet been introduced; no unit-test coverage is claimed.
- Docker Compose starts a healthy PostgreSQL 18.6 development container.
- Flyway successfully applies and validates the initial `platform` schema
  migration.
- The PostgreSQL relational smoke test completes successfully and rolls back
  its temporary test data.

These results verify the current developer machine and branch. Clean-checkout
reproduction and CI verification remain pending.

## Troubleshooting

If API startup fails with an address-in-use error for port `5164`, check whether
another Rider or terminal-launched API process is already running. Stop the
existing process or choose an intentionally documented alternate port; do not
treat a port collision as an application compilation failure.

When reporting another setup problem, include:

- The exact sanitized command.
- Complete relevant error output.
- Operating system and shell.
- Relevant tool versions.
- The current commit or branch.
- What was expected and what occurred.

Do not include passwords, tokens, private keys, proprietary data, or unredacted
connection strings.

## Related Documentation

- [Project overview](../README.md)
- [Architecture](architecture.md)
- [Roadmap](roadmap.md)
- [Architecture decisions](decisions)
