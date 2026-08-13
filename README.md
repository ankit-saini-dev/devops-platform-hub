# DevOps Platform Hub

DevOps Platform Hub is a self-hostable platform operations application for
registering software projects, modeling deployment environments, and tracking
build and deployment activity.

The project is being developed incrementally as a practical exploration of
software architecture, DevOps, platform engineering, security, testing, and
observability.

> Status: Product and repository foundation. Application development has not
> started.

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

Architecture decisions and their trade-offs will be recorded under
[`docs/decisions`](docs/decisions) as the project progresses.

## Roadmap

1. Product and repository foundation
2. Application foundation
3. Project and environment management
4. Simulated build and deployment tracking
5. External CI/CD integrations
6. Containers and observability
7. Cloud and orchestration

Detailed milestone planning will be maintained in
[`docs/roadmap.md`](docs/roadmap.md) when that document is introduced.

## Project principles

- Build incrementally and verify every milestone
- Keep secrets out of source control and logs
- Treat security, testing, logging, and documentation as first-class concerns
- Add tools only when they solve a demonstrated problem
- Prefer understandable architecture over premature complexity
- Avoid proprietary code, workflows, terminology, and confidential information
- Do not claim features are implemented until they have been verified

## Repository status

Phase 0 is in progress. The product charter, architecture decisions,
contribution workflow, and Phase 1 backlog are being established before
application code is generated.

## License

This project is licensed under the [MIT License](LICENSE).