# Waymark Project Foundation

**Date:** 2026-08-16
**Status:** Foundation approved; implementation design remains open

## Context

The archived Sable project is being used as a reference for a new, independent project. Waymark is not intended to be a mechanical rename of Sable or an inheritance of the original repository's tooling and architectural choices.

The original Sable repository remains untouched and is retained separately as a read-only reference.

## Decisions

### Project identity

- Project name: **Waymark**
- Domain: database migration tooling for Marten and PostgreSQL
- The name intentionally does not directly include Marten.
- `Burrow` was considered, but rejected as an inaccurate association because martens generally use tree hollows, rock crevices, logs, or abandoned nests rather than digging burrows.

Proposed package and API identities:

```text
Library package: Waymark
CLI package: Waymark.Cli
CLI command: waymark
Namespace: Waymark.Extensions (provisional)
```

The final NuGet publisher is intentionally undecided. Candidates discussed were `gabrewer`, `integratrak`, and `lessi`; this must be decided before the first publication.

### Repository

- Local repository: `/Users/gabrewer/Source/repos/waymark`
- GitHub repository: to be created later
- Git history: fresh history
- Initial branch: `bootstrap/waymark-foundation`
- The repository currently contains only foundational files and the `docs/design` directory.

### Build and tooling

Waymark will not use Cake initially. The preferred build approach is the native .NET SDK and MSBuild commands:

```text
dotnet restore
dotnet build
dotnet test
dotnet pack
```

CI, packaging, and publishing workflows will be designed explicitly for Waymark. No Sable build, release, or repository automation will be copied unchanged.

### Source reuse

The Sable implementation may be consulted and selectively ported, but Waymark will not inherit the original project's choices automatically. The new project will explicitly decide its:

- Public API and CLI contracts
- Migration file and artifact formats
- Project and solution layout
- Dependency versions
- Versioning strategy
- Assembly signing strategy
- Package metadata
- Documentation and CI workflows

The existing Sable signing key must not be reused. Waymark will later choose a new signing key or intentionally disable signing.

### Licensing

The MIT license is preserved in the new repository. Required upstream attribution and any other applicable license obligations must remain intact.

## Open decisions

1. Which Sable behaviors, CLI commands, migration formats, and database semantics should Waymark preserve?
2. Which behaviors or contracts should be redesigned?
3. What .NET target framework should Waymark support?
4. Should Waymark use strong-name signing with a new key?
5. What versioning strategy should replace the inherited setup?
6. Which GitHub owner and repository visibility should be used?
7. Which NuGet publisher should own `Waymark` and `Waymark.Cli`?
8. What documentation and sample projects belong in the initial release?

## Next step

Define Waymark's intended public contracts and supported behavior before scaffolding the solution or porting implementation code.
