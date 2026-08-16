# Waymark Pi Project Context

**Purpose:** Project-specific context for configuring AI orchestration in Pi.

## Project state

Waymark is a greenfield .NET database-migration tool for Marten and PostgreSQL. The repository currently contains foundational documentation only; no solution, project files, source code, tests, build scripts, or CI workflows exist yet.

The approved foundation document is `docs/design/20260816-waymark-project-foundation.md`. It defines Waymark as an independent project that may selectively consult the archived Sable repository, but must make its own public API, CLI, migration formats, dependencies, target framework, versioning, signing, packaging, documentation, and CI decisions.

Sable reference:

- GitHub: https://github.com/gabrewer/sable
- Local reference on the development machine: `X:\source\sable`

## Repository conventions and current tooling

- Current branch: `bootstrap/waymark-foundation`
- Mainline synchronization and feature-branch safety follow the global Pi instructions.
- No repository-specific `AGENTS.md`, solution, project, package manager, test framework, lint command, or verification command exists yet.
- The first implementation task must explicitly bootstrap the real .NET solution/project structure before downstream build or test automation is planned.
- Intended native build commands are `dotnet restore`, `dotnet build`, `dotnet test`, and `dotnet pack`, subject to the target framework and solution structure decisions.

## Sable reference audit

The archived Sable source was reviewed at `X:\source\sable\main` and https://github.com/gabrewer/sable. It is a .NET 10 solution with:

- `src/Sable/`: Marten service-registration integration and connection-string override behavior.
- `src/Sable.Cli/`: the `sable` .NET tool, Spectre.Console commands, migration generation, SQL composition, shadow PostgreSQL containers, and database updates.
- `tests/Sable.Tests/` and `tests/Sable.Cli.Tests/`: xUnit coverage, currently focused on service registration and filesystem utilities.
- `samples/`: runnable Marten examples and generated migration/script artifacts.
- `_docs/`: VitePress documentation source; `docs/` is generated output.
- `build.cake`, GitHub workflows, package metadata, and an existing strong-name key.

The primary user-facing Sable workflow is `sable init`, `sable migrations add`, `sable migrations script`, and `sable database update`, with support for multiple databases, multi-tenancy, backfills, generated migration files, and generated SQL scripts. These behaviors are reference inputs for Waymark parity analysis, not automatic contracts. The Pi setup must therefore include migration/domain and public API/CLI contract roles, plus explicit source-delta auditing before implementation.

Sable-specific build and release choices (Cake, inherited package identities, signing key, generated docs pipeline, and CI) are reference material only; Waymark's foundation explicitly requires independent decisions.

## Orchestration configuration

- **State backend:** github-issues
- Planning is owned by `/pm-agent`.
- Approved-plan execution is owned by `/team-lead`.
- Worker specialization belongs in project-local skills and must remain subordinate to those two front doors.
- Temporary GitHub issue/comment bodies belong under `.pi/tmp/` and must not be committed.
- Acceptance is human-owned: agents prepare evidence and a `Ready for Acceptance Verification` record but do not close issues or apply final completion labels.

## Initial orchestration priorities

1. Make the first bootstrap/build task explicit and avoid inventing paths that do not exist.
2. Keep project foundation decisions, public contracts, and Sable parity/deviation analysis ahead of implementation.
3. Use conservative worker scopes until the solution and test surfaces exist.
4. Require exact verification commands once the solution is created.
