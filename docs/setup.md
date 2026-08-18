# Development Setup

## Status

DevOps Platform Hub is currently in Phase 1: Application Foundation.
Application source code, runtime services, database containers, and executable
setup commands have not yet been introduced.

This document records the planned development prerequisites and setup standards.
It will be updated with exact verified commands as each Phase 1 issue introduces
an application component or dependency. Do not interpret a listed technology as
an implemented dependency.

## Supported Development Environment

The project should remain usable on a developer workstation without paid cloud
services during its early phases. Windows with PowerShell is the initial
development environment, while source code and containerized dependencies
should remain cross-platform where practical.

Commands in this document must identify the shell they target. Platform-specific
instructions should be separated clearly rather than assumed to work
everywhere.

## Planned Prerequisites

The following tools are expected as the project progresses:

- Git for version control.
- A current supported .NET SDK for the ASP.NET Core backend.
- A current supported Node.js release and package manager for Angular.
- Angular CLI where justified by the frontend workflow.
- Docker Desktop or a compatible Docker Engine for PostgreSQL and later local
  dependencies.
- A code editor or IDE with C#, TypeScript, and EditorConfig support.

Exact supported versions will be selected and documented when the corresponding
component is scaffolded. Version compatibility will be verified from official
documentation at that time.

## Repository Setup

The repository can currently be cloned and its documentation reviewed, but
there are no application restore, build, test, migration, or startup commands
yet.

The future setup flow is expected to include:

1. Clone the repository.
2. Checkout the intended branch.
3. Create local configuration from committed examples.
4. Start required local dependencies.
5. Restore backend and frontend dependencies.
6. Apply database migrations.
7. Start the backend and frontend applications.
8. Run applicable automated checks.

Each step will receive exact, verified commands when the required files exist.

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

The repository does not currently require an `.env` file or database
credentials.

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
- Backend startup and health responses.
- Frontend startup and browser access.
- Clear failure guidance for common setup problems.

Command output should be summarized truthfully. A planned command must not be
reported as successful before it has been executed.

## Current Verification

During the documentation-only Phase 0-to-Phase 1 transition, verify changes by
checking:

- Markdown renders clearly.
- Relative links resolve.
- Terminology agrees with the README, architecture, roadmap, and ADRs.
- Planned technologies are not described as installed or running.
- No secret, proprietary material, or unsupported claim is present.

## Troubleshooting

Detailed troubleshooting guidance will be introduced after reproducible setup
or runtime failures exist. When reporting a future setup problem, include:

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
