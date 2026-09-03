# Development Setup

## Status

DevOps Platform Hub is currently in Phase 1: Application Foundation.
The initial ASP.NET Core Web API host, backend solution, SDK policy, unit-test
project, integration-test project, and startup smoke test have been introduced
on the current backend foundation branch.

Angular, PostgreSQL, database migrations, authentication, health endpoints, and
product modules have not yet been implemented. This document distinguishes
verified backend commands and tests from planned setup that has no
implementation yet.

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

The backend targets `net10.0`. The backend `global.json` selects SDK
`10.0.400`, permits compatible updates through the `latestPatch` roll-forward
policy, and rejects preview SDKs.

The following planned tools are not current backend prerequisites:

- A supported Node.js release, package manager, and Angular CLI for the future
  Angular application.
- Docker Desktop or a compatible Docker Engine when PostgreSQL is introduced.

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

Frontend dependencies, local services, configuration examples, and database
migrations will be added to this flow only after their implementations exist.

## Backend Structure

```text
backend/
`-- DevOpsPlatformHub/
    |-- global.json
    |-- DevOpsPlatformHub.slnx
    |-- DevOpsPlatformHub.Api/
    |-- DevOpsPlatformHub.UnitTests/
    `-- DevOpsPlatformHub.IntegrationTests/
```

- `global.json` selects the supported .NET SDK feature band.
- `DevOpsPlatformHub.slnx` groups the backend projects for restore, build, and
  test commands.
- `DevOpsPlatformHub.Api` is the deployable ASP.NET Core Web API host.
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

## Configuration and Secrets

- Real secrets must never be committed.
- `.env.example` will be introduced when environment-based configuration first
  exists and will contain placeholders only.
- Developer-specific configuration files must remain ignored unless they are
  explicitly safe examples.
- Logs, screenshots, test output, and issue reports must be reviewed for secrets
  before sharing.
- Integration tokens must use least privilege and remain separate from ordinary
  project and environment metadata.

The API's committed configuration currently contains logging levels and allowed
host configuration only. The repository does not require an `.env` file,
database credentials, tokens, or another local secret for the backend
foundation.

## Planned Local Services

PostgreSQL is the selected initial relational database. Docker Compose is the
planned local delivery mechanism after database persistence is introduced.

No database container, port, username, password, volume, or connection string
has been finalized. Those values will be documented only after the Compose and
application configuration have been implemented and tested together.

Later services must not be added to local setup until an implemented workflow
requires them.

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
