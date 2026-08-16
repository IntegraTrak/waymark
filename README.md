# Waymark

Provider-agnostic database migration orchestration for .NET applications.

Waymark uses native migration tooling inside isolated development databases to produce reviewable, version-controlled migration artifacts. Marten/PostgreSQL and Entity Framework Core/SQL Server are the initial provider targets.

## Vision

Waymark coordinates the migration workflow without reimplementing provider-specific schema or migration engines:

1. Identify the application and migration configuration.
2. Start an isolated provider database, typically with Testcontainers.
3. Materialize the application model using the provider's native tooling.
4. Generate an incremental, provider-owned migration payload.
5. Wrap it in a durable Waymark migration artifact.
6. Compose ordered artifacts into a provider-aware deployment script.

Provider-specific SQL, model discovery, transaction rules, and migration semantics remain inside each provider adapter. Waymark provides the common orchestration, artifact, ordering, CLI, and safety boundaries.

## Initial providers

- **Marten / PostgreSQL** — first provider implementation and Sable parity reference.
- **Entity Framework Core / SQL Server** — first additional provider, using EF Core's native migration tooling.

The providers do not need identical behavior or portable SQL. Each provider produces artifacts and scripts for its own database engine.

## Safety principles

- Use disposable or explicitly isolated databases for migration generation.
- Never use production, staging, shared, or ambiguous databases as shadow environments.
- Require an explicitly identified target before applying migrations.
- Preserve provider-specific transaction and idempotence requirements.
- Keep generated artifacts reviewable and version-controllable.

## Project status

Waymark is in the foundation and architecture phase. The repository currently contains design documents and Pi-based AI orchestration resources; the .NET solution and first provider vertical slice have not yet been bootstrapped.

The current implementation plan is based on:

- [Provider migration PRD](docs/design/2026-08-16-waymark-provider-migration-prd.md)
- [Provider-agnostic migration vision](docs/design/2026-08-16-waymark-provider-agnostic-migration-vision.md)
- [Project foundation](docs/design/20260816-waymark-project-foundation.md)

## Sable reference

Waymark is an independent project. The archived [Sable repository](https://github.com/gabrewer/sable) is used as a read-only reference for Marten/PostgreSQL behavior and isolated database workflows. Waymark will make its own decisions about APIs, CLI contracts, migration artifacts, dependencies, versioning, signing, packaging, and build automation.

## Development

The first implementation task will bootstrap the real .NET solution. Once available, the intended native commands are:

```bash
dotnet restore
dotnet build
dotnet test
dotnet pack
```

Project planning and execution use Pi prompts and skills backed by GitHub Issues. See [AGENTS.md](AGENTS.md) and the project design documents for current constraints and decisions.

## License

Licensed under the MIT License. See [LICENSE](LICENSE).
