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

Wrap native provider output with Waymark metadata such as provider identity, database engine, migration identity, ordering, source/model information, and execution requirements while preserving provider-specific payloads.

### 6. Aggregate migration script — must-have

Provide a `waymark migrations script` workflow that reads ordered artifacts and composes a single provider-aware script suitable for review and execution against an explicitly identified target database.

### 7. CLI workflow — should-have

Provide a consistent initial CLI surface for selecting a provider, identifying application/model inputs, generating artifacts, and composing scripts. Exact command names and options are to be defined by the API/CLI contract task.

### 8. Safety and capability reporting — should-have

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

- **Orchestration engine:** provider selection, workflow coordination, artifact persistence, ordering, script composition, and safety checks.
- **Provider adapter:** native model loading, provider migration tooling, database-specific behavior, payload generation, and capability reporting.
- **Isolated database environment:** Testcontainers lifecycle, configuration, readiness, connection details, and cleanup.
- **Artifact model:** common metadata plus a provider-specific payload; artifacts must remain reviewable and version-controllable.
- **Target execution:** explicit target selection and provider-aware execution rules.
- **CLI:** stable user-facing commands that delegate to typed core contracts.

The initial workflow must prove incremental migration generation using native tooling. Each provider should demonstrate a change between an earlier and later model/migration state, produce a provider-specific payload, persist a Waymark artifact, and participate in aggregate script generation.

Marten/PostgreSQL and EF Core/SQL Server will not necessarily expose the same native inputs or output shapes. Their adapters may differ internally while satisfying the same orchestration lifecycle and artifact envelope.

Sable is a reference for the Marten/PostgreSQL behavior and isolated database approach. Waymark must not mechanically inherit Sable’s Cake build, package identities, signing key, CLI contracts, artifact formats, or repository automation.

## Milestones

1. **Bootstrap the real .NET solution** — establish the actual library, CLI, provider, environment, and test project structure before downstream tasks reference paths.
2. **Define provider and artifact contracts** — document typed orchestration, provider capability, isolated environment, native tooling, artifact, and script composition contracts.
3. **Build the shared orchestration path** — implement provider selection, environment lifecycle coordination, artifact persistence, ordering, and safety boundaries using test doubles where appropriate.
4. **Implement paired provider vertical slices** — implement Marten/PostgreSQL and EF Core/SQL Server Testcontainers environments and native migration-tool integrations against the same narrow workflow.
5. **Prove aggregate script generation** — generate ordered artifacts for both providers, compose one provider-aware script per database engine, and verify deterministic output and safety behavior.

## Acceptance Criteria for the First Slice

- A real Waymark .NET solution and test surface exists.
- Both Marten/PostgreSQL and EF Core/SQL Server implement the agreed provider contract.
- Both providers use disposable Testcontainers environments for migration generation.
- Both providers use their native migration tooling rather than a Waymark-reimplemented schema engine.
- Each provider can produce an incremental, provider-specific migration payload.
- Waymark wraps each payload in a durable artifact with common metadata.
- `waymark migrations script` composes ordered artifacts into one executable script for the selected provider/database engine.
- Tests cover provider contract behavior, artifact metadata, ordering, cleanup, and safety rejection paths.
- No generation or execution uses production, staging, shared, or ambiguous databases.

## Open Questions

1. What exact native Marten APIs and EF Core design-time APIs should each adapter use?
2. How will the first slice provide the earlier and later model/migration states without requiring Git integration?
3. Should artifacts store native migration code, generated SQL, or both where the provider supports both?
4. What common metadata is required to validate that artifacts belong to the selected provider and database engine?
5. Which Testcontainers images, versions, startup options, and authentication defaults are supported?
6. What CLI command names and options best express provider selection and artifact generation?
7. How should provider-specific transaction, idempotence, and non-transactional operation requirements affect script composition?
8. Should provider adapters be built into the initial CLI or distributed as separate packages?
9. Which Sable behaviors should be preserved, redesigned, or explicitly deferred after the vertical slice is proven?
