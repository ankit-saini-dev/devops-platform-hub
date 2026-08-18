# Project Roadmap

## Status

This roadmap describes the planned direction of DevOps Platform Hub.

The project is currently in Phase 1: Application Foundation. Phase 1 work is
planned and tracked through the ordered GitHub backlog below; its capabilities
must not be treated as implemented until their acceptance criteria and
verification steps pass. Items in future phases are proposals, not implemented
features or guaranteed commitments. Scope and ordering may change when
implementation, testing, security reviews, or user feedback provide new
evidence.

Progress will be reported from verified repository and runtime results. A phase
or capability will not be marked complete merely because its code was written;
its acceptance criteria and applicable tests must also pass.

## Roadmap Principles

- Deliver one understandable and testable increment at a time.
- Establish product and domain behavior before integrating external platforms.
- Use local and free development options during early phases.
- Treat security, testing, logging, and documentation as part of each feature.
- Add technology only when it solves a demonstrated project requirement.
- Keep the application explainable and maintainable by one developer.
- Prefer simulated providers before real credentials and infrastructure are
  required.
- Record durable architecture decisions and their trade-offs.
- Never commit secrets or expose them through logs, APIs, screenshots, or test
  data.
- Do not reproduce proprietary employer code, workflows, terminology, data, or
  architecture.

## Phase Overview

| Phase | Outcome | Status |
| --- | --- | --- |
| Phase 0 | Product and repository foundation | Complete |
| Phase 1 | Secure and testable application foundation | In progress |
| Phase 2 | Project and environment management | Planned |
| Phase 3 | Simulated build and deployment tracking | Planned |
| Phase 4 | Real source-control and CI/CD integrations | Proposed |
| Phase 5 | Container visibility and observability | Proposed |
| Phase 6 | Cloud deployment and orchestration | Proposed |

## Phase 0: Product and Repository Foundation

### Goal

Create an implementation-ready product and repository foundation before
generating application code.

### Outcomes

- Define the product vision, problem, users, MVP, and exclusions.
- Select the initial architecture and technology direction.
- Document system boundaries, module responsibilities, and dependency rules.
- Record foundational architecture decisions.
- Establish a protected, pull-request-based Git workflow.
- Define repository maintenance and documentation standards.
- Create an ordered and testable Phase 1 backlog.

### Completed Work

- Product vision and MVP scope are documented in the README.
- The initial modular-monolith architecture is documented.
- The phased project roadmap is documented.
- ADR 0001 records the modular-monolith decision.
- ADR 0002 records PostgreSQL as the initial database.
- ADR 0003 records Angular Material as the initial UI component library.
- ADR 0004 records GitHub Flow as the repository workflow.
- The `main` branch is protected based on the confirmed repository settings.
- CodeQL default setup is enabled based on the confirmed repository settings.
- Automatic deletion of merged remote branches is enabled based on the
  confirmed repository settings.
- The pull-request checklist, formatting rules, ignore rules, and Phase 1 setup
  expectations are documented.
- The ordered Phase 1 milestone and issues define dependencies, acceptance
  criteria, and verification steps.
- Phase 0 documentation has been reviewed for consistent scope and terminology.

### Exit Criteria

- The maintainer and repository reviewers can understand the product, scope,
  and planned architecture.
- Foundational decisions have accepted ADRs with alternatives and consequences.
- Repository workflow and maintenance expectations are documented.
- Phase 1 work is divided into small issues with dependencies and verification
  steps.
- All documentation links resolve.
- No application code has been introduced during Phase 0.

## Phase 1: Application Foundation

### Goal

Establish a secure, observable, and testable foundation on which product
features can be built.

### Planned Outcomes

- Scaffold the ASP.NET Core backend and Angular frontend as separate,
  reviewable changes.
- Establish local configuration conventions and secret-safe examples.
- Introduce PostgreSQL and version-controlled database migrations.
- Add consistent API error handling and validation foundations.
- Add structured application logging without exposing sensitive values.
- Add health endpoints with clearly defined availability and dependency checks.
- Establish local authentication and backend-enforced role authorization.
- Add backend unit and integration test foundations.
- Add frontend unit and component test foundations.
- Add GitHub Actions checks for real build, test, formatting, and lint commands.
- Confirm CodeQL scans the supported C# and TypeScript source successfully before
  making its result a required merge check.
- Document verified local setup and troubleshooting steps.

### Guardrails

- Do not implement product modules merely to populate the navigation.
- Do not add a durable job system before background work exists.
- Do not add real external integrations or cloud dependencies.
- Do not describe authentication, health checks, logging, or CI as complete
  until their runtime behavior has been verified.

### Ordered Backlog

1. [Complete the Phase 0-to-Phase 1 transition](https://github.com/ankit-saini-dev/devops-platform-hub/issues/5)
2. [Select supported tool versions and scaffold the ASP.NET Core backend](https://github.com/ankit-saini-dev/devops-platform-hub/issues/6)
3. [Scaffold the Angular frontend with Angular Material](https://github.com/ankit-saini-dev/devops-platform-hub/issues/7)
4. [Add initial continuous-integration checks](https://github.com/ankit-saini-dev/devops-platform-hub/issues/8)
5. [Add PostgreSQL development infrastructure and persistence foundation](https://github.com/ankit-saini-dev/devops-platform-hub/issues/9)
6. [Establish API validation, error handling, and structured logging](https://github.com/ankit-saini-dev/devops-platform-hub/issues/10)
7. [Add liveness and PostgreSQL readiness health checks](https://github.com/ankit-saini-dev/devops-platform-hub/issues/11)
8. [Implement local authentication and backend-enforced authorization](https://github.com/ankit-saini-dev/devops-platform-hub/issues/12)
9. [Verify and document the complete Phase 1 developer experience](https://github.com/ankit-saini-dev/devops-platform-hub/issues/13)

The [Phase 1 milestone](https://github.com/ankit-saini-dev/devops-platform-hub/milestone/1)
groups these issues. Dependencies recorded in each issue determine implementation
order; list position alone does not imply that unfinished dependencies may be
skipped.

### Exit Criteria

- A new developer can follow documented steps to run the applications locally.
- Backend and frontend builds succeed using documented commands.
- Automated test suites execute successfully.
- PostgreSQL connectivity and migrations are verified against PostgreSQL.
- Authentication and representative authorization rules are tested.
- Health and error responses are verified.
- Pull requests run stable, relevant CI checks.

## Phase 2: Project and Environment Management

### Goal

Allow authorized users to register software projects and model the environments
in which their applications operate.

### Planned Outcomes

- Create, view, update, and archive registered projects.
- Store repository metadata without storing credentials as ordinary project
  data.
- Create and manage environments such as Development, Staging, and Production.
- Associate environments with their owning projects.
- Add validation rules and protected state transitions.
- Add creation and modification audit fields.
- Record important project and environment changes in the audit history.
- Provide usable Angular forms, lists, details, and permission-aware actions.
- Add bounded list results or pagination where data can grow.

### Guardrails

- Do not execute builds or deployments in this phase.
- Do not store source-control tokens in project or environment records.
- Do not introduce provider-specific fields into the core domain without an
  established integration boundary.
- Do not rely on hidden Angular controls as authorization enforcement.

### Exit Criteria

- Authorized users can complete the project and environment workflows through
  the UI and API.
- Invalid, conflicting, unauthorized, and missing-resource cases are tested.
- Persistence behavior is covered by PostgreSQL-compatible integration tests.
- Audit records exist for the selected state-changing actions.
- API and setup documentation reflect the implemented behavior.

## Phase 3: Simulated Build and Deployment Tracking

### Goal

Validate the central operational workflow without depending on external CI/CD
systems or infrastructure credentials.

### Planned Outcomes

- Request a simulated build for a registered project.
- Process queued work outside the HTTP request lifetime.
- Enforce allowed build status transitions.
- Persist build progress, logs, timestamps, and outcomes.
- Request a simulated deployment of an eligible build to an environment.
- Enforce deployment status and eligibility rules.
- Record deployment history and the currently deployed version where defined.
- Display recent activity and execution details in the Angular application.
- Record audit events for build and deployment requests.
- Record initial notification entries for selected failure events.

### Guardrails

- Simulated execution must not accept arbitrary user-provided shell commands.
- HTTP requests must not remain open for the lifetime of a build or deployment.
- Status transitions must be owned and validated by the relevant module.
- Initial polling is preferred over real-time delivery until its limitations are
  demonstrated.
- Limit log retrieval so one request cannot load unbounded history.

### Exit Criteria

- The complete MVP workflow works locally from project registration through a
  simulated deployment.
- Successful and failed execution paths are reproducible and tested.
- Restart and failure limitations of the initial background worker are
  documented honestly.
- Users can determine what was deployed, where, when, and by whom.
- Logs and audit records exclude secrets and sensitive configuration.

## Phase 4: Real Integrations

### Goal

Replace selected simulated boundaries with real source-control or CI/CD
providers while retaining the core application workflow.

### Candidate Outcomes

- Authenticate to GitHub using a least-privilege integration method.
- Retrieve verified repository metadata through the GitHub API.
- Trigger and observe selected GitHub Actions workflows.
- Receive and validate provider webhooks where they improve reliability.
- Add Jenkins integration only if it provides a distinct learning and product
  use case beyond GitHub Actions.
- Translate provider-specific states and failures into application-owned models.
- Protect integration secrets and document credential rotation and revocation.
- Add timeout, retry, idempotency, and rate-limit behavior based on provider
  requirements.

### Entry Conditions

- Simulated build and deployment workflows are stable and tested.
- Provider interfaces are based on actual application use cases.
- A secret-storage approach has been selected and documented.
- Threats involving webhooks, tokens, replay, and external input have been
  reviewed.

### Exit Criteria

- At least one real integration completes a documented end-to-end workflow.
- Provider failures and unavailable dependencies produce controlled outcomes.
- Credentials can be revoked without corrupting core project data.
- Integration-specific limitations and setup steps are documented.

## Phase 5: Containers and Observability

### Goal

Provide useful operational visibility for the application and selected
container workloads.

### Candidate Outcomes

- Display selected Docker container state through a restricted integration.
- Add live execution updates with SignalR if polling has become a demonstrated
  usability or efficiency problem.
- Define meaningful application and workflow metrics.
- Introduce OpenTelemetry for selected traces, metrics, or logs when there are
  real flows to observe.
- Introduce Prometheus and Grafana only for defined metrics and dashboards.
- Add actionable failure notifications through a justified delivery provider.
- Document data retention, cardinality, access, and sensitive-data controls.

### Entry Conditions

- The application has working processes worth observing.
- Required signals and diagnostic questions are defined before tools are added.
- Docker access has an explicit threat model and least-privilege boundary.
- Local resource requirements remain manageable.

### Exit Criteria

- Dashboards and alerts answer documented operational questions.
- Metrics and screenshots use real or clearly labeled simulated data.
- Container access does not expose unrestricted host control through the API.
- Observability overhead and data-handling limitations are documented.

## Phase 6: Cloud and Orchestration

### Goal

Deploy and operate the application using justified orchestration,
infrastructure-as-code, and cloud practices.

### Candidate Outcomes

- Containerize the verified application components with hardened runtime images.
- Define Kubernetes resources after container deployment requirements are known.
- Introduce Helm if environment-specific Kubernetes packaging requires it.
- Use Terraform for explicitly scoped and reproducible infrastructure.
- Deploy to a selected AWS architecture with cost limits and teardown guidance.
- Integrate a justified secrets-management solution.
- Add production-oriented identity, network, backup, recovery, and hardening
  controls.
- Document deployment, rollback, incident response, and operational ownership.

### Entry Conditions

- Local application behavior and container images are stable.
- Deployment targets, availability objectives, budget, and threat model are
  documented.
- Every proposed cloud service has a product or operational purpose.
- Destructive infrastructure operations have safeguards and recovery guidance.

### Exit Criteria

- Infrastructure can be reproduced from version-controlled definitions.
- Deployment and teardown procedures are verified and documented.
- Secrets are supplied through an approved mechanism rather than source control.
- Health, logs, metrics, backup, restore, and rollback behavior are tested at a
  level appropriate to the selected environment.
- Documentation distinguishes demonstrated behavior from future production
  recommendations.

## MVP Boundary

The MVP is expected at the completion of Phase 3. It should allow a local user
to:

1. Sign in with an authorized role.
2. Register a software project and its repository metadata.
3. Define at least one deployment environment.
4. Request and observe a simulated build.
5. Inspect persisted build logs and the final outcome.
6. Request a simulated deployment of an eligible build.
7. Inspect deployment and audit history.
8. Run the documented automated tests.

Real GitHub, Jenkins, Docker, Kubernetes, AWS, and observability integrations
are outside the MVP.

## Roadmap Maintenance

This document will be reviewed when:

- A milestone is completed.
- A planned capability is materially added, removed, or reordered.
- An ADR changes a foundational assumption.
- Implementation reveals a new dependency or risk.
- A GitHub issue changes the acceptance criteria for a phase outcome.

Completed work must be supported by merged repository changes and applicable
verification evidence. Detailed tasks and acceptance criteria belong in GitHub
issues; this roadmap should remain focused on outcomes, sequencing, entry
conditions, and phase boundaries.
