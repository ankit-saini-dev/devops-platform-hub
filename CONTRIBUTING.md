# Contributing to DevOps Platform Hub

Thank you for considering a contribution to DevOps Platform Hub. This project
is developed incrementally as both a usable product and a practical learning
project. Contributions should remain understandable, verifiable, secure, and
consistent with the documented product scope.

## Project Status

The project is currently in Phase 0: Product and Repository Foundation.
Application code, runtime services, and build commands have not yet been
introduced. Do not describe planned capabilities as implemented.

Review the following documents before proposing a material change:

- [README](README.md)
- [Architecture](docs/architecture.md)
- [Roadmap](docs/roadmap.md)
- [Architecture decisions](docs/decisions)

## Before Contributing

- Search existing issues and pull requests before proposing duplicate work.
- Use a GitHub issue for changes that need scope or acceptance criteria agreed
  before implementation.
- Keep changes within the active milestone unless a different priority has been
  explicitly accepted.
- Do not reproduce proprietary code, architecture, endpoints, terminology,
  business logic, test data, credentials, or confidential information.
- Do not add a tool only to increase the number of technologies listed by the
  project.

## Development Workflow

This repository follows the GitHub Flow described in
[ADR 0004](docs/decisions/0004-use-github-flow.md).

1. Synchronize your local `main` branch.
2. Create a short-lived branch for one cohesive unit of work.
3. Implement the change and its applicable tests and documentation.
4. Review the complete diff for scope, generated files, secrets, and unrelated
   changes.
5. Commit using Conventional Commits.
6. Push the branch and open a pull request targeting `main`.
7. Resolve review conversations and satisfy applicable automated checks.
8. Merge only after the acceptance criteria and Definition of Done are met.
9. Synchronize `main`, prune stale references, and remove the merged local
   branch.

Do not commit directly to `main`. Do not rewrite shared `main` history.

Related files and commits should be grouped into one cohesive pull request.
Unrelated application, infrastructure, security, and documentation work should
not be combined merely to reduce the number of pull requests.

## Branch Naming

Use a short category followed by a concise purpose:

- `feature/` for user-facing functionality
- `fix/` for defect corrections
- `docs/` for documentation and decision records
- `refactor/` for behavior-preserving structural changes
- `test/` for test-only work
- `chore/` for repository maintenance
- `ci/` for continuous-integration workflows
- `security/` for focused security improvements

Examples:

```text
docs/repository-foundation
feature/project-registration
fix/build-status-transition
ci/backend-validation
```

Avoid names such as `changes`, `updates`, `work`, or personal names that do not
describe the outcome.

## Commit Messages

Use Conventional Commits:

```text
<type>(optional-scope): <concise description>
```

Examples:

```text
docs: add phased project roadmap
feat(projects): register software projects
fix(builds): reject invalid status transitions
test(api): cover unauthorized project updates
ci: validate backend build and tests
```

Common types include `feat`, `fix`, `docs`, `refactor`, `test`, `chore`, `ci`,
`build`, `perf`, and `security`.

Keep commits focused and explain why a change exists when the reason is not
clear from the diff. Do not claim a feature works until it has been verified.

## Pull Requests

Every pull request should include, where applicable:

- A concise summary and reason for the change.
- A linked issue and closing keyword when the issue is fully resolved.
- Verification commands and their actual results.
- Manual verification evidence when automation is insufficient.
- Security, configuration, migration, and compatibility impacts.
- Documentation files updated or an explanation of why none were needed.
- Known limitations and intentionally deferred work.

Before requesting review:

- Confirm the branch targets the latest appropriate `main` state.
- Review every changed file.
- Remove debugging output and unrelated formatting changes.
- Confirm no secret or sensitive information appears in commits.
- Ensure the pull request represents one cohesive outcome.
- Complete the checklist in the pull request template.

## Testing and Verification

Run all checks relevant to the change. Until application code is introduced,
documentation changes should be verified for:

- Correct Markdown structure and readable rendering.
- Working relative links.
- Consistent terminology and roadmap status.
- Accurate distinction between planned and implemented behavior.
- Absence of secrets, proprietary material, and unsupported claims.

Build, test, lint, migration, and startup commands will be documented after the
corresponding applications exist. Do not invent placeholder commands and report
them as successful verification.

When application code is introduced, pull requests must record the exact
applicable commands and results. A passing check does not replace review of
security, architecture, product behavior, or documentation impact.

## Documentation Responsibilities

For every meaningful change, review whether it affects:

- `README.md`
- `CHANGELOG.md` after versioned application changes begin
- `docs/architecture.md`
- `docs/setup.md`
- `docs/api.md` after application endpoints exist
- `docs/roadmap.md`
- `docs/decisions/`
- `.env.example` after environment-based configuration exists
- Docker, deployment, and CI files after those capabilities exist
- Screenshots or diagrams after the UI and deployment architecture become usable

Create an ADR when a decision is durable, affects multiple future changes, has
meaningful alternatives, or would be difficult to reverse silently.

## Security and Secrets

- Never commit credentials, tokens, passwords, private keys, production data, or
  secret-bearing connection strings.
- Never place secrets in source files, examples, tests, logs, screenshots,
  issue descriptions, or pull requests.
- Use clearly fake placeholders in example configuration.
- Treat webhook payloads, external provider responses, and user input as
  untrusted.
- Do not submit a public issue containing vulnerability details that could put
  users or systems at risk.

A dedicated `SECURITY.md` with a private reporting process will be introduced
before the project accepts security-sensitive runtime integrations. Until then,
contact the repository owner privately through an available GitHub contact
method for sensitive reports.

## Reporting Bugs and Requesting Features

Use the repository issue forms and provide reproducible, non-sensitive details.

Bug reports should include expected behavior, actual behavior, reproduction
steps, environment information, and relevant sanitized output. Feature requests
should describe the problem and desired outcome before proposing a specific
technology.

Security vulnerabilities must not be submitted using the public bug form.

## Code of Conduct

A dedicated Code of Conduct will be added when external contribution becomes a
regular project activity. In the meantime, communicate respectfully, provide
constructive technical feedback, and focus discussions on the work and its
evidence.

## License

By contributing, you agree that your contributions will be licensed under the
repository's [MIT License](LICENSE).
