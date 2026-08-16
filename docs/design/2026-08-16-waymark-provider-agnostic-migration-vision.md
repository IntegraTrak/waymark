# Waymark Provider-Agnostic Migration Vision

**Date:** 2026-08-16
**Status:** Vision approved; architecture and provider contracts remain open

## Vision

Waymark will provide database migration tooling for applications whose database schema is derived from an application model. It will use an isolated development or shadow database to materialize the model, generate or capture migration changes, and produce migration artifacts that can be reviewed, versioned, and applied safely.

Waymark must not be permanently coupled to Marten. Marten and PostgreSQL are the first supported provider combination, but the architecture should also support combinations such as Entity Framework Core and SQL Server.

## Core workflow

The provider-neutral workflow is:

1. Identify the application project and migration configuration.
2. Start or provision an isolated development database, commonly through a container.
3. Materialize the current application model in that database using the selected provider.
4. Compare the resulting schema with the relevant migration state or baseline.
5. Generate migration SQL and related artifacts.
6. Store, compose, review, and optionally apply migrations to a target database.

The isolated database is a central design concept. It allows Waymark to inspect the schema produced by the real application configuration without requiring migration generation to run against a shared or production database.

## Initial provider targets

### Marten and PostgreSQL

This is the first provider target and the primary source of behavioral reference from the archived Sable project. Waymark should evaluate Sable's commands, migration formats, generated SQL, multi-database behavior, multi-tenancy behavior, backfills, and database safety as candidate behavior—not as automatically fixed contracts.

### Entity Framework Core and SQL Server

Waymark should be designed to support an EF Core application targeting SQL Server. This may require provider-specific handling for:

- EF Core model discovery and design-time creation;
- SQL Server container provisioning and readiness;
- EF Core migration discovery, generation, and execution;
- SQL Server-specific migration operations and transaction rules;
- connection, authentication, and database initialization conventions.

The EF Core/SQL Server provider should share the general orchestration and artifact contracts while retaining provider-specific implementation details.

## Architectural direction

Waymark should separate:

- **Orchestration engine:** coordinates discovery, isolation, schema materialization, comparison, artifact generation, and application.
- **Provider adapter:** integrates with an ORM or schema technology and its target database.
- **Database environment:** provisions, configures, waits for, and disposes isolated databases or containers.
- **Schema and migration model:** represents provider output without erasing important provider-specific semantics.
- **Artifact system:** defines migration files, generated scripts, metadata, ordering, and composition.
- **Target execution:** applies approved artifacts to explicitly identified target databases.
- **CLI:** exposes stable user-facing commands while delegating provider-specific behavior to adapters.

Provider-specific behavior must not leak into the core through untyped free-form settings when a typed contract is required.

## Safety principles

- Never use production, staging, shared, or ambiguous databases as shadow databases.
- Require an explicitly identified target before applying migrations.
- Prefer isolated containers or disposable local databases for schema inspection and generation.
- Preserve provider-specific transaction and idempotence constraints in generated artifacts.
- Make generated migration files and scripts reviewable and version-controllable.
- Keep provider discovery and execution deterministic wherever possible.

## Relationship to Sable

Sable remains an implementation and behavior reference for the Marten/PostgreSQL provider. Waymark may selectively port useful implementation ideas, including the isolated PostgreSQL workflow, but will independently decide its public APIs, CLI contracts, artifact formats, dependency versions, target frameworks, signing, packaging, versioning, and build/release automation.

Sable's Cake build, package identities, signing key, and repository automation are not part of this vision.

## Open design questions

1. What is the provider adapter contract, and which lifecycle stages are mandatory?
2. Should Waymark model schema snapshots, migration operations, generated SQL, or all three?
3. Which container runtimes and database images should be supported initially?
4. How should EF Core design-time configuration and migration assemblies be discovered?
5. Should providers be built into the CLI, distributed as packages, or both?
6. What migration artifact format can remain portable without hiding provider-specific SQL semantics?
7. How should multi-database and multi-tenant applications be represented across providers?
8. Which operations require explicit confirmation before target execution?
9. What is the minimum Marten/PostgreSQL slice needed to validate the provider-neutral architecture?

## Next step

Define the provider-neutral contracts and a narrow Marten/PostgreSQL vertical slice before implementing broad CLI parity or adding EF Core/SQL Server support.
