---
description: Execute an approved Waymark sprint through sequential Pi roles and quality gates
argument-hint: "<sprint-issue-or-id>"
---

Execute the approved Waymark sprint `$1` using the configured `github-issues` backend from `AGENTS.md`; do not ask for the backend.

Read `AGENTS.md`, the approved issue and comments, `docs/design/waymark-pi-project-context.md`, and the applicable dated design documents. Confirm human approval, dependencies, exact file scopes, verification commands, and current branch/status before editing. Do not invent paths.

Run sequentially: domain modeler, API/CLI contract, task test-writer, assigned dotnet-builder, deterministic build gate, destroyer, review-agent remediation loop, git-committer, full smoke test, and PM summary. Read each role skill immediately before adopting it. Use Sable only for explicitly scoped parity audits. Keep provider-neutral contracts distinct from Marten/PostgreSQL or EF Core/SQL Server details.

Use `.pi/tmp/` for long GitHub comment drafts. Never commit temporary files, close issues, apply final disposition labels, or self-approve acceptance. Completion requires real commit SHAs, verification evidence, `## 🚀 Sprint Complete`, and `## 🧑‍⚖️ Ready for Acceptance Verification` mapped to the original criteria.
