# ADR 0001: Use a Modular Monolith

## Status

Accepted

## Context

DevOps Platform Hub is being developed incrementally by one developer as both a
usable product and a practical learning project. The application is expected to
contain several related business capabilities, including identity and access,
projects, environments, builds, deployments, audit records, and notifications.

These capabilities need clear ownership and dependency boundaries so that the
codebase remains understandable as it grows. At the same time, early
development must remain straightforward to run, test, debug, and deploy without
requiring paid cloud services or a complex local infrastructure environment.

A distributed architecture would introduce service discovery, network failure,
inter-service authentication, distributed tracing, data ownership, messaging,
deployment coordination, and eventual-consistency concerns before the product
has demonstrated a need for them. Those concerns are valuable to learn, but
introducing them now would slow validation of the core product workflows.

A traditional layered monolith would be simple to begin with, but organizing
the entire backend primarily around technical layers could make business
boundaries less visible as the number of features increases. The project needs
the operational simplicity of one backend application while retaining explicit
feature ownership.

## Decision

DevOps Platform Hub will begin with a modular-monolith backend.

The backend will be one deployable ASP.NET Core application organized into
feature-oriented modules. The initial planned modules are:

- Identity and Access
- Projects
- Environments
- Builds
- Deployments
- Audit
- Notifications

Each module will own its business behavior and internal persistence details.
Modules may collaborate through explicit application contracts, but they must
not modify another module's data by reaching into its internal implementation.
Cross-module dependencies must be intentional and must not form cycles.

The Angular frontend will remain a separate client application and will
communicate with the backend through its HTTP API. The frontend is not a module
inside the backend and will not connect directly to the database or external
infrastructure providers.

The modules are logical code and ownership boundaries, not independently
deployable services. The backend will initially be built, tested, versioned,
and deployed as one application. External systems will be accessed through
application-defined provider contracts when integrations are introduced.

This decision does not guarantee that every planned module must exist from the
beginning. Modules will be implemented incrementally when their capabilities
become part of an active milestone.

## Alternatives Considered

### Traditional Layered Monolith

A traditional layered monolith would organize the backend mainly into layers
such as controllers, services, repositories, and models.

This option would be familiar and quick to scaffold. It would also retain the
deployment and debugging simplicity of a single application. However, as the
product grows, code belonging to one business capability could become spread
across broad technical folders. Generic service and repository layers could
also hide feature ownership without providing a meaningful boundary beyond the
behavior already supplied by ASP.NET Core and Entity Framework Core.

This alternative was not selected because feature-oriented modules make the
product's business boundaries more explicit while retaining the benefits of a
single deployable application. Technical layering may still be used inside a
module where it improves clarity.

### Microservices

A microservices architecture would deploy capabilities such as projects,
builds, deployments, and notifications as separate services.

This option could provide independent deployment, scaling, failure isolation,
and team ownership. It could become appropriate if different parts of the
platform later develop materially different operational requirements.

It was not selected for the initial architecture because the project currently
has one developer, one release lifecycle, and no measured need for independent
scaling or deployment. Starting with microservices would require solving
distributed data consistency, network reliability, service authentication,
observability, local orchestration, and deployment coordination before the core
workflows have been validated.

Microservices are deferred rather than rejected permanently. A future service
extraction must be based on demonstrated product or operational requirements.

## Consequences

### Positive

- Local development requires fewer running processes and infrastructure
  dependencies.
- The backend can be built, tested, debugged, and deployed as one unit.
- Related operations can use straightforward relational transactions where
  appropriate.
- Feature-oriented modules make business ownership more visible than a purely
  technical folder structure.
- Simulated providers and future real providers can be introduced behind
  explicit application boundaries.
- The team can focus first on product workflows, security, tests, and
  maintainability rather than distributed-system coordination.
- Module boundaries provide possible extraction points if independent services
  become justified later.

### Negative

- Module boundaries are not protected by network isolation and can be violated
  through careless dependencies or direct database access.
- A change to one module may require building and deploying the entire backend.
- A failure that exhausts shared process resources may affect all backend
  modules.
- Modules cannot be scaled independently while they remain in the same process.
- Shared database usage can create coupling if table ownership and access rules
  are not maintained.
- The architecture requires ongoing review and tests to prevent feature
  boundaries from degrading into an unstructured monolith.
- Extracting a module later may require changes to transactions, communication,
  data ownership, and failure handling.

## Reconsider When

This decision should be reviewed when evidence shows that the modular monolith
no longer meets the product's operational or organizational needs. Relevant
signals include:

- A module requires independent scaling because measured load differs
  materially from the rest of the backend.
- Separate teams need independent ownership and release schedules for specific
  capabilities.
- One module requires a substantially different availability, isolation, or
  security boundary.
- Backend deployments become frequent or risky primarily because unrelated
  modules must be released together.
- A module needs a technology or runtime that cannot be supported responsibly
  within the existing application.
- The in-process background execution model cannot meet demonstrated durability
  or resource-isolation requirements.
- Module coupling remains a delivery bottleneck despite documented boundaries,
  dependency checks, and focused refactoring.

Reconsideration does not imply that the entire application must be converted to
microservices. The preferred response may be to strengthen module boundaries,
separate background workers, or extract only the capability with a proven need.
