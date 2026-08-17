# Waymark Provider Migration PRD

**Date:** 2026-08-16
**Status:** Approved for planning

## Product Name & One-Liner

**Waymark** — provider-agnostic migration orchestration for applications using native database migration tooling.

## Problem & Audience

Application teams need reliable, reviewable database migrations without running schema discovery or migration generation against shared or production databases. Existing migration behavior is often tied to a single framework or database engine, making it difficult to provide a consistent workflow across technologies.

Waymark is for .NET developers maintaining applications that use Marten/PostgreSQL or Entity Framework Core/SQL Server. They want to use the migration tooling they already trust while gaining isolated database environments, durable migration artifacts, and a consistent script-generation workflow.

## Core Features

### 1. Provider-neutral migration orchestration — must-have

Coordinate model loading, isolated database lifecycle, native migration tooling, artifact creation, and script composition without embedding Marten- or EF-specific behavior in the core.

### 2. Marten/PostgreSQL provider — must-have

Use Marten’s native schema and migration capabilities to materialize model changes in an isolated PostgreSQL environment and produce a provider-owned migration payload.

### 3. EF Core/SQL Server provider — must-have

Use EF Core’s native migration and SQL-generation tooling to materialize model changes in an isolated SQL Server environment and produce a provider-owned migration payload.

### 4. Isolated provider database environments — must-have

Use Testcontainers for PostgreSQL and SQL Server providers to provision disposable databases, wait for readiness, run provider operations, collect results, and clean up safely.

### 5. Durable migration artifacts — must-have

Start from the proven Sable artifact model rather than inventing a new generic migration format. Preserve the durable, ordered migration artifact and aggregate-script workflow: a provider payload, a stable migration identity derived from its artifact name, provider-specific SQL, and execution directives for idempotence and transaction handling. Extend that model only where EF Core requires additional information or where the shared orchestration contract needs it. The artifact envelope must identify the provider and database engine while preserving the native provider payload and any provider-specific metadata.

### 6. Aggregate migration script — must-have

Provide a `waymark migrations script` workflow that reads ordered artifacts and composes a single provider-aware script suitable for review and execution against an explicitly identified target database.

### 7. CLI workflow — should-have

Provide a consistent initial CLI surface for explicitly selecting a provider, identifying application/model inputs, generating artifacts, and composing scripts. Provider selection must be supplied by the caller rather than inferred from project references, configuration, or database connection details. Exact command names and options are to be defined by the API/CLI contract task.

### 8. Provider sample applications — must-have

Create small, representative sample applications for both Marten/PostgreSQL and EF Core/SQL Server. Each sample must provide the model/configuration inputs needed to exercise an earlier and later migration state, and must be usable for local testing, automated integration tests, and end-to-end dogfooding of the CLI workflow.

### 9. Safety and capability reporting — should-have

Expose provider capabilities and execution requirements rather than assuming parity. Prevent use of production, staging, shared, or ambiguous databases for isolated generation or target execution.

## Non-Goals

- Reimplementing Marten or EF Core migration engines.
- Converting migration artifacts or SQL between PostgreSQL and SQL Server.
- Requiring identical migration semantics across providers.
- Supporting every Sable command or behavior in the first vertical slice.
- Building a Git integration or requiring Waymark to understand source-control revisions initially.
- Running migrations automatically against production, staging, shared, or ambiguous databases.
- Adding providers beyond Marten/PostgreSQL and EF Core/SQL Server in the first slice.
- Finalizing package publisher, signing, versioning, or release automation as part of the first provider slice.

## Technical Considerations

Waymark should be structured around these boundaries:

- **Orchestration engine:** explicit provider selection, workflow coordination, artifact persistence, ordering, script composition, and safety checks.
- **Provider adapter:** native model loading, provider migration tooling, database-specific behavior, payload generation, and capability reporting.
- **Isolated database environment:** Testcontainers lifecycle, configuration, readiness, connection details, and cleanup.
- **Artifact model:** common metadata plus a provider-specific payload; artifacts must remain reviewable and version-controllable.
- **Target execution:** explicit target selection and provider-aware execution rules.
- **CLI:** stable user-facing commands that delegate to typed core contracts.

The initial workflow must prove incremental migration generation using native tooling. Each provider should demonstrate a change between an earlier and later model/migration state, produce a provider-specific payload, persist a Waymark artifact, and participate in aggregate script generation. The earlier state is represented by the provider's existing migration state, and the later state is produced by the provider's native migration tooling. The sample applications are the controlled fixtures for this workflow: EF Core uses its migrations and model snapshot tooling, while Marten follows the shadow-database and native `marten-patch` workflow demonstrated by Sable. Waymark orchestrates these tools through an extensible provider contract; the Marten and EF Core adapters delegate model comparison and migration generation to their native tooling. The contract must also support future providers with different native capabilities or provider-owned comparison and generation steps, without requiring changes to the orchestration engine.

The initial artifact contract should be an evolution of Sable's model, not a wholesale redesign. Sable's ordered SQL migration files, migration naming, generated headers, idempotence and transaction directives, migration tracking, and aggregate script composition are the baseline behaviors. Waymark should add only the common provider/engine envelope and the minimum EF Core metadata needed to validate and compose artifacts safely. Native migration code, snapshots, hashes, and richer source metadata should be added only when required by a provider or an explicit workflow.

Marten/PostgreSQL and EF Core/SQL Server will not necessarily expose the same native inputs or output shapes. Their adapters may differ internally while satisfying the same orchestration lifecycle and artifact envelope.

Provider selection is an explicit input to the initial CLI and core workflow. Waymark must not guess based on installed packages, project references, discovered `DbContext` types, connection strings, or database engine names: a project may contain multiple contexts or provider-related dependencies, and inference could select the wrong migration system. The initial provider identifiers are `marten-postgresql` and `efcore-sqlserver`; configuration-based defaults may be added later, but an explicit command-line value must take precedence.

Sable is a reference for the Marten/PostgreSQL behavior and isolated database approach. Waymark must not mechanically inherit Sable’s Cake build, package identities, signing key, CLI contracts, artifact formats, or repository automation.

## Milestones

1. **Bootstrap the real .NET solution** — establish the actual library, CLI, provider, environment, and test project structure before downstream tasks reference paths.
2. **Define provider and artifact contracts** — document typed orchestration, provider capability, isolated environment, native tooling, artifact, and script composition contracts.
3. **Build the shared orchestration path** — implement provider selection, environment lifecycle coordination, artifact persistence, ordering, and safety boundaries using test doubles where appropriate.
4. **Create provider sample applications** — create dogfoodable Marten/PostgreSQL and EF Core/SQL Server samples with controlled earlier and later model/migration states.
5. **Implement paired provider vertical slices** — implement Marten/PostgreSQL and EF Core/SQL Server Testcontainers environments and native migration-tool integrations against the sample applications and the same narrow workflow.
6. **Prove aggregate script generation** — generate ordered artifacts for both providers, compose one provider-aware script per database engine, and verify deterministic output and safety behavior using the samples.

## Sprint 1 Acceptance Criteria: CLI Bootstrap

Sprint 1 is intentionally limited to establishing a runnable CLI and the real solution structure. It does not require provider migration behavior yet.

- A real Waymark .NET solution and test surface exists.
- The `waymark` CLI builds and starts successfully.
- The CLI exposes a minimal help or version command and returns a successful exit code.
- The CLI project can be exercised by an automated smoke test.
- Provider-dependent commands fail clearly when no provider is supplied rather than attempting inference.
- The solution and CLI have no dependency on Sable implementation identities or inherited build tooling.

## Acceptance Criteria for the First Provider Slice

The following criteria apply after the shared contracts, sample applications, and paired provider vertical slices are implemented—not to Sprint 1.

- A real Waymark .NET solution and test surface exists.
- Both Marten/PostgreSQL and EF Core/SQL Server implement the agreed provider contract.
- Both providers use disposable Testcontainers environments for migration generation.
- Both providers use their native migration tooling rather than a Waymark-reimplemented schema engine.
- Each provider can produce an incremental, provider-specific migration payload.
- Waymark wraps each payload in a durable artifact with common metadata.
- `waymark migrations script` composes ordered artifacts into one executable script for the selected provider/database engine.
- Tests cover provider contract behavior, artifact metadata, ordering, cleanup, and safety rejection paths.
- Both sample applications can be used to generate and compose migrations through the CLI without bespoke provider-specific workflow steps.
- The sample applications provide repeatable earlier and later model/migration states for automated integration and end-to-end dogfood tests.
- No generation or execution uses production, staging, shared, or ambiguous databases.

## Open Questions

1. What exact native Marten APIs and EF Core design-time APIs should each adapter use?
2. Which additional EF Core metadata, if any, is required beyond the Sable-derived artifact envelope?
3. What common metadata is required to validate that artifacts belong to the selected provider and database engine?
4. Which Testcontainers images, versions, startup options, and authentication defaults are supported?
5. What exact CLI command names and option syntax should expose the required provider selection and artifact generation inputs?
6. How should provider-specific transaction, idempotence, and non-transactional operation requirements affect script composition?
7. Should provider adapters be built into the initial CLI or distributed as separate packages?
8. Which Sable behaviors should be preserved, redesigned, or explicitly deferred after the vertical slice is proven?
