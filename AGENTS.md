# Waymark Project Instructions

**State backend:** github-issues

Waymark is a provider-agnostic database migration tool. Marten/PostgreSQL is the first provider; EF Core/SQL Server is a planned provider. Read the relevant dated design documents under `docs/design/` before planning or implementation.

- Use `/pm-agent` for planning and `/team-lead` for approved-plan execution.
- Follow `instructions/TEAM-ORCHESTRATION.md` and `instructions/TOOL-PI.md` from the AI Engineering Orchestration reference when applying this workflow.
- Use GitHub Issues as the durable source of sprint/task state. Keep temporary issue/comment bodies under `.pi/tmp/`; never commit them.
- Do not invent solution, project, package, test, or verification paths. The first implementation task must bootstrap the real .NET solution.
- Do not change public library or CLI contracts, migration artifacts, provider behavior, database execution, signing, packaging, or CI without an explicit task.
- Treat Sable as a reference for parity analysis, not as an inherited architecture or contract. Reference: https://github.com/gabrewer/sable; local reference: `X:\source\sable`.
- Never run migration updates against production, staging, shared, or ambiguous databases.
- Agents prepare acceptance evidence; humans decide acceptance. Never close GitHub issues or apply final disposition labels.
- Preserve unrelated working-tree changes and follow the repository/global feature-branch and commit safety rules.
