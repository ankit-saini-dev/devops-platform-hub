# DevOps Platform Hub

DevOps Platform Hub is a self-hostable platform operations application for
registering software projects, modeling deployment environments, and tracking
build and deployment activity.

The project is being developed incrementally as a practical exploration of
software architecture, DevOps, platform engineering, security, testing, and
observability.

> Status: Phase 1 application foundation. The ASP.NET Core backend host,
> modular backend project structure, Angular Material shell, initial CI checks,
> local PostgreSQL/Flyway database foundation, shared API error and logging
> behavior, and liveness and PostgreSQL readiness checks are implemented.
> Product features have not started.

## Why this project exists

Operational information is often distributed across source-control systems,
CI/CD tools, cloud consoles, monitoring products, and team communication.

DevOps Platform Hub aims to provide a consistent view of projects,
environments, builds, deployments, and operational history while integrating
progressively with existing engineering tools.

It is not intended to replace GitHub, Jenkins, Kubernetes, or monitoring
platforms.

## Target users

- Developers inspecting build, deployment, and environment status
- DevOps and platform engineers managing operational workflows
- Technical leads reviewing release and deployment history
- Administrators managing access and auditability

## Planned MVP

The initial product will support:

- Local user authentication and role-based access
- Project and repository registration
- Environment management
- Simulated build execution
- Simulated deployment execution
- Build and deployment history
- Persisted execution logs
- Basic health checks
- Audit records for important actions
- Automated testing foundations
- Local development without paid cloud services

Real external integrations will be introduced only after the core domain and
simulated workflows are stable.

## Planned technology direction

- Angular and TypeScript
- ASP.NET Core Web API
- Entity Framework Core
- PostgreSQL
- Docker and Docker Compose
- GitHub Actions

Later technologies such as Jenkins, Kubernetes, Helm, Terraform, AWS,
Prometheus, Grafana, and OpenTelemetry are roadmap candidates, not currently
implemented features.

## Architecture direction

The application will begin as a modular monolith with clear feature boundaries.
This keeps local development and deployment manageable while allowing the
system to evolve as real requirements emerge.

Architecture decisions and their trade-offs are recorded under
[`docs/decisions`](docs/decisions).

## Roadmap

1. Product and repository foundation
2. Application foundation
3. Project and environment management
4. Simulated build and deployment tracking
5. External CI/CD integrations
6. Containers and observability
7. Cloud and orchestration

See the [project roadmap](docs/roadmap.md) for phase outcomes, boundaries, and
progress.

## Documentation

- [Architecture](docs/architecture.md)
- [API conventions](docs/api.md)
- [Project roadmap](docs/roadmap.md)
- [Development setup](docs/setup.md)
- [Architecture decisions](docs/decisions)

## Project principles

- Build incrementally and verify every milestone
- Keep secrets out of source control and logs
- Treat security, testing, logging, and documentation as first-class concerns
- Add tools only when they solve a demonstrated problem
- Prefer understandable architecture over premature complexity
- Avoid proprietary code, workflows, terminology, and confidential information
- Do not claim features are implemented until they have been verified

## Repository status

Phase 0 is complete. The product charter, architecture decisions, roadmap,
repository workflow, and ordered Phase 1 backlog are established.

Phase 1, Application Foundation, is active. The backend host, modular backend
project structure, backend test foundation, Angular Material shell, and initial
backend/frontend CI checks are implemented. Local PostgreSQL 18.6 runs through
Docker Compose; Flyway applies and validates versioned SQL migrations, and a
PostgreSQL smoke test verifies the resulting schema. The API produces safe,
consistent error responses, structured request logs, and separate liveness and
PostgreSQL readiness endpoints. Authentication and product capabilities remain
planned.

## License

This project is licensed under the [MIT License](LICENSE).
