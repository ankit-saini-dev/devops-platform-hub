# DevOps Platform UI

This Angular application is the Phase 1 frontend foundation for DevOps Platform
Hub. The displayed application name is "DevOps Platform". It contains a Material
toolbar and a main-content area, with no product routes or API integration yet.
The foundation is implemented on its feature branch and awaits pull-request
review. It was generated using Angular CLI 22.1.7.

## Installation

From the repository root in PowerShell:

```powershell
cd ui\devops-platform-hub-ui
npm.cmd ci
```

Current inspected tool versions are Node.js 24.21.0 and npm 11.19.0. Use the
lockfile for dependency installation. For verification status, see
[development setup](../../docs/setup.md#frontend-setup).

## Development server

To start a local development server, run:

```powershell
npm.cmd start
```

Once the server is running, open your browser and navigate to `http://localhost:4200/`. The application will automatically reload whenever you modify any of the source files.

## Code scaffolding

Angular CLI includes powerful code scaffolding tools. To generate a new component, run:

```powershell
npx.cmd ng generate component component-name
```

For a complete list of available schematics (such as `components`, `directives`, or `pipes`), run:

```powershell
npx.cmd ng generate --help
```

## Building

To build the project run:

```powershell
npm.cmd run build
```

This will compile your project and store the build artifacts in the `dist/` directory. By default, the production build optimizes your application for performance and speed.

## Running unit tests

To execute unit tests with the [Vitest](https://vitest.dev/) test runner, use the following command:

```powershell
npm.cmd test -- --watch=false
```

## Running end-to-end tests

No end-to-end framework or runnable end-to-end target has been configured.
The current component tests do not replace browser verification.

## Files in source control

Commit application source, public assets, package manifests and lockfile,
Angular/TypeScript/ESLint/Prettier configuration, and documentation. Shared
editor configuration is optional and must describe working tools.

Do not commit `node_modules`, `dist`, or `.angular/cache`; the existing ignore
rules exclude them. Tooling recreates these directories. Do not commit local
secrets or credentials. Inspect both staged and unstaged changes before a commit;
staging a file does not include edits made afterward.

## Linting and formatting

Run these commands in PowerShell from `ui/devops-platform-hub-ui`:

```powershell
npm.cmd run lint
npm.cmd run format
npm.cmd run format:check
```

- `lint` checks TypeScript and Angular templates using `eslint.config.mjs`.
  Warnings and errors cause the command to fail.
- `format` applies available ESLint fixes, then runs Prettier with `--write`.
  Review the resulting changes. If ESLint reports an error it cannot fix,
  the command stops before Prettier runs.
- `format:check` runs lint followed by Prettier's read-only formatting check.
  It fails when either step fails and does not modify source files.

These scripts currently target `src`; they do not check formatting of root
configuration files or this README. Prettier reads `.prettierrc` and supported
`.editorconfig` settings. The current conventions include two-space indentation,
spaces instead of tabs, a preferred width of 140 characters, and LF line endings.

The ESLint Stylistic `padding-line-between-statements` rule requires a blank
line after `if`, `for` (including `for...of` and `for...in`), `while`, `do...while`,
and `switch` statements when another statement follows in the same block.
It does not require a blank line before an enclosing closing brace or between
an `if` branch and its `else`. Prettier handles indentation and reduces repeated
blank lines; it does not insert the required statement separation on its own.

If `format:check` reports a formatting difference, run `format`, review the
changes, and run `format:check` again. Passing formatting and lint checks does
not replace tests or browser verification.

## Additional Resources

For more information on using the Angular CLI, including detailed command references, visit the [Angular CLI Overview and Command Reference](https://angular.dev/tools/cli) page.
