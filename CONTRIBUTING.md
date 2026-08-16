# Contributing to Waymark

Thank you for contributing to Waymark. Waymark is an independent project inspired by Sable and is being designed to support multiple migration providers, beginning with Marten/PostgreSQL and EF Core/SQL Server.

## Before starting

- Search existing issues and design documents before proposing work.
- For substantial changes, open or comment on an issue before implementation.
- Do not assume Sable behavior is a Waymark contract; parity, redesign, or deferral should be explicit.
- Never use production, staging, shared, or ambiguous databases for development or testing.

## Development workflow

1. Fork the repository or create a feature branch if you have collaborator access.
2. Keep changes focused on one issue or coherent task.
3. Follow `AGENTS.md` and the relevant dated design documents.
4. Add or update tests for behavior changes.
5. Run the repository's documented build and test commands.
6. Open a pull request against `main` using the pull request template.

Until the .NET solution is bootstrapped, do not invent project or test paths. The initial implementation task will establish the real solution structure and commands.

## Pull requests

Pull requests should explain:

- the problem and intended behavior;
- provider and database-engine scope;
- public API, CLI, migration-artifact, or database-safety impact;
- exact verification commands and results;
- any limitations, accepted deviations, or follow-up work.

Maintainers review contributions for correctness, provider-boundary integrity, migration safety, test evidence, and maintainability. A pull request is not accepted solely because automated checks pass; maintainers make the final acceptance decision.

## Reporting security issues

Do not disclose security-sensitive details in a public issue. Follow [SECURITY.md](SECURITY.md).
