# Architecture

## Status

This document describes the planned initial architecture of DevOps Platform Hub.

The project is currently in Phase 1: Application Foundation. The components and
technologies described here remain the accepted architectural direction and
planned implementation unless they are explicitly marked as implemented.

The initial ASP.NET Core Web API host, backend solution, .NET SDK policy, and
unit and integration test projects have been introduced during Phase 1. The
integration-test project contains a startup smoke test that boots the assembled
API in memory and verifies the development OpenAPI document can be served as
JSON. The unit-test project remains empty because no business rules exist yet.

Feature modules, persistence, background execution, authentication, health
endpoints, and product endpoints remain planned. The startup test verifies the
current application host, not those future capabilities.

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

## Backend Modules

### Identity and Access

Authenticates users and authorizes protected operations. It will own user,
role, and permission concepts needed by the application. Authorization will be
enforced by the backend even when the frontend also hides unavailable actions.

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

Entity Framework Core will map domain data to PostgreSQL and manage database
migrations. Database access should remain behind module-owned application
boundaries. A generic repository abstraction will not be introduced initially
because EF Core already provides unit-of-work and collection-like persistence
behavior. More specialized abstractions may be added when they protect a real
domain boundary or improve testability.

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

Validation failures, missing resources, conflicts, and unexpected errors should
produce consistent responses. Internal exceptions and sensitive details must
not be returned to clients.

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
