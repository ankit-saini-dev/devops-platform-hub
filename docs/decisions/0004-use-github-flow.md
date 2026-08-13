# ADR 0004: Use GitHub Flow for Repository Changes

## Status

Accepted

## Context

DevOps Platform Hub is currently maintained by one developer, but the repository
is intended to demonstrate professional software delivery practices. Changes
need to remain reviewable, traceable, and safe without introducing a branching
model designed for several long-lived release streams.

Committing directly to the default branch makes accidental or incomplete
changes easier to publish and removes the opportunity to review a focused diff
before it becomes part of the main history. At the other extreme, maintaining
permanent development, release, and hotfix branches would add synchronization
and merge overhead that the project does not currently need.

The repository uses GitHub for source control and pull requests. The `main`
branch represents the current integrated state of the project. Branch
protection, pull requests, CodeQL default setup, and automatic deletion of
merged remote branches have been enabled through repository settings.

The workflow must support documentation-only work during Phase 0 and later
expand to include automated build, test, lint, and security checks when the
application code and commands exist.

## Decision

DevOps Platform Hub will use GitHub Flow with short-lived branches and pull
requests.

All planned changes will begin from an up-to-date `main` branch and be developed
on a focused branch. The branch will be pushed to GitHub and merged through a
pull request after applicable review and verification are complete.

The `main` branch must remain protected against direct changes. A pull request
is required before merging. Force pushes and branch deletion must remain
blocked for `main`, and configured protection rules should apply without an
unintentional administrator bypass.

The project currently has one maintainer, so pull requests do not require a
second person's approval. Repository write and merge permissions determine who
can merge; requiring zero approving reviews does not grant merge access to
untrusted users. If trusted collaborators begin maintaining the project, human
review requirements will be reconsidered.

Each branch and pull request should represent one cohesive unit of work. A
cohesive change may contain multiple commits or files when they belong to the
same outcome. Unrelated application, infrastructure, and documentation changes
should not be grouped merely to reduce the number of pull requests.

Conventional Commits will be used for commit messages. Commit types will
describe the nature of the change, including `feat`, `fix`, `docs`, `refactor`,
`test`, `chore`, `ci`, `build`, `perf`, and `security` where appropriate.

Merged remote branches will be deleted automatically. Local branches will be
deleted after the merge is verified and the local `main` branch is updated.

## Workflow

The normal workflow is:

1. Confirm that the working tree is clean and switch to `main`.
2. Pull the latest accepted changes from the remote repository.
3. Create a short-lived branch with a name describing the change.
4. Implement and verify the bounded unit of work.
5. Review the diff for scope, secrets, generated files, and documentation
   impact.
6. Create one or more clear Conventional Commits.
7. Push the branch and open a pull request targeting `main`.
8. Record the change, rationale, verification evidence, documentation impact,
   and known limitations in the pull request.
9. Resolve review conversations and satisfy required automated checks.
10. Merge the pull request using the repository's allowed merge method.
11. Verify the result on `main` and confirm remote branch cleanup.
12. Synchronize local `main`, prune stale remote references, and delete the
    merged local branch.

For a change that has already been merged, subsequent corrections must use a
new branch and pull request rather than rewriting `main` history.

## Branch Naming

Branches will use a short category followed by a concise purpose. Initial
categories include:

- `feature/` for user-facing functionality
- `fix/` for defect corrections
- `docs/` for documentation and decision records
- `refactor/` for behavior-preserving structural changes
- `test/` for test-only improvements
- `chore/` for repository maintenance
- `ci/` for continuous-integration workflows
- `security/` for focused security improvements

Examples include:

- `docs/initial-architecture`
- `feature/project-registration`
- `fix/build-status-transition`
- `ci/backend-validation`

Branch names should describe the outcome rather than a person's name or a vague
term such as `changes`, `work`, or `updates`.

## Pull Request Requirements

Every pull request should include, where applicable:

- A concise summary of the outcome.
- The reason for the change.
- Verification commands and their results.
- Manual test evidence when automation is not sufficient.
- Security, configuration, migration, and compatibility impacts.
- Documentation files that were updated or an explanation of why none were
  required.
- Known limitations or intentionally deferred work.
- The related GitHub issue and appropriate closing keyword when the issue is
  fully resolved.

A pull request may be marked ready only when its acceptance criteria are met and
the author has reviewed the complete diff. Passing automation will not replace
human reasoning about scope, architecture, security, or product behavior.

## Merge Strategy

The repository may use a merge commit or squash merge as long as the resulting
history remains understandable and the selected method is applied consistently
enough for contributors to follow it.

Squash merge is preferred when a branch contains temporary correction commits
that do not provide useful long-term history. A merge commit may be retained
when the branch contains a small sequence of meaningful commits that should
remain individually visible.

The pull request title must be suitable for the resulting commit when squash
merge is used. The repository should not claim that a merge strategy is
enforced until the relevant GitHub setting has been verified.

## Automated Checks

CodeQL default setup is enabled as an initial code-scanning control. Because the
repository does not yet contain the planned C# or TypeScript application code,
the project will not require a CodeQL result as a merge check until a supported
language exists on `main` and a successful scan has established the check name
and behavior.

As implementation proceeds, relevant build, test, formatting, lint, dependency,
and security checks will be added to pull requests. A check will become required
only after it runs reliably and its failure behavior is understood. Empty or
placeholder workflows will not be added solely to display a CI badge or claim a
capability.

Required checks must not be bypassed to merge ordinary work. Emergency-process
rules will be documented only when the project has a deployment and operational
context that makes such a process necessary.

## Alternatives Considered

### Direct Commits to Main

Direct commits would minimize steps for a single developer and may be acceptable
for disposable experiments.

This option was not selected because it removes the pull request as a reviewable
unit, increases the chance of accidental changes to the integrated branch, and
does not exercise the repository controls expected in a professional delivery
workflow.

### Git Flow

Git Flow uses long-lived `main` and `develop` branches with additional feature,
release, and hotfix branches. It can support products with scheduled releases
and multiple maintained versions.

It was not selected because this project currently has one integrated version,
one maintainer, and no parallel release trains. A permanent `develop` branch
would add merging and synchronization work without solving a demonstrated
problem.

### Trunk-Based Development with Direct Main Integration

Trunk-based development favors very small changes integrated into the mainline
frequently, often supported by extensive automated tests and feature flags.

Its emphasis on small, frequent integration is valuable and is compatible with
short-lived pull request branches. However, direct mainline integration was not
selected because the project currently benefits from an explicit pull request
review point and does not yet have the automated safety net needed for a more
permissive workflow.

### Long-Lived Feature Branches

Long-lived branches could allow an entire phase or large feature to be developed
before integration.

This option was not selected because it delays feedback, increases merge
conflicts, hides intermediate architectural drift, and produces pull requests
that are harder to review and test. Large milestones will instead be divided
into small issues and cohesive pull requests.

## Consequences

### Positive

- `main` has a controlled integration path through pull requests.
- Every merged change has a reviewable discussion and verification record.
- Small branches reduce merge conflicts and make failures easier to isolate.
- The workflow is simple enough for one maintainer and can accommodate future
  collaborators.
- Conventional Commits make repository history and future changelog generation
  easier to understand.
- Automatic remote branch deletion reduces stale branch clutter.
- Required CI checks can be introduced incrementally after they are proven.

### Negative

- Even small changes require branch, push, pull request, and merge steps.
- Zero required approvals means the sole maintainer must review their own work
  critically until collaborators are available.
- Repository settings exist outside the Git history and must be documented and
  periodically verified.
- Automatic remote deletion does not remove local branches or stale references
  without local cleanup.
- Inconsistent merge methods can make history less predictable if repository
  conventions are not followed.
- Required checks can block valid changes when workflows or external services
  fail for reasons unrelated to the code.

## Implementation Constraints

- No user may receive write or administrative repository access without a
  justified maintenance role.
- Secrets, credentials, tokens, and sensitive logs must be excluded before a
  branch is pushed; branch protection cannot undo secret exposure.
- Force pushing or rewriting shared feature-branch history should be avoided
  after review has begun unless collaborators are informed.
- Required checks must be relevant, deterministic enough for normal use, and
  documented before enforcement.
- GitHub repository settings affecting review, merging, security, and branch
  deletion should be reviewed when the contributor model changes.
- The pull request template and contribution guide should reflect this decision
  when those files are introduced.
- Documentation-only changes require appropriate document verification even
  when application tests are not applicable.
- A change must not be described as committed, merged, deployed, or working
  until the corresponding evidence has been confirmed.

## Reconsider When

This decision should be reviewed when the repository's team structure, release
model, or operational risk changes materially. Relevant signals include:

- Multiple maintainers participate regularly and a human approval requirement
  becomes practical.
- The project maintains multiple supported release lines or scheduled release
  branches.
- Deployment frequency and test maturity justify a stricter trunk-based model
  with feature flags.
- Compliance requirements mandate signed commits, additional reviewers,
  separation of duties, or different retention rules.
- Pull request volume creates measurable merge contention that requires a merge
  queue or different integration strategy.
- Required automated checks become unreliable enough to block delivery and need
  redesign rather than routine bypassing.
- Repository ownership moves to an organization with centrally managed
  rulesets and contributor roles.

Reconsidering this decision may strengthen review or automation requirements
without replacing GitHub Flow entirely.
