## Summary

<!-- Describe the problem and the change. Keep this focused on the approved task. -->

**Authoritative sprint/epic issue:** #
**Task ID:** `TASK-`
**Related design/PRD:**

## Change type

- [ ] Bootstrap/build infrastructure
- [ ] Bug fix
- [ ] New feature
- [ ] Provider implementation or behavior
- [ ] Public API or CLI contract change
- [ ] Migration artifact or SQL behavior change
- [ ] Documentation or maintenance

## Provider scope

- [ ] Provider-neutral
- [ ] Marten / PostgreSQL
- [ ] EF Core / SQL Server
- [ ] Other: <!-- explain below -->
- [ ] Not applicable

## Scope and contract impact

- [ ] This PR implements only the approved task scope.
- [ ] The original acceptance criteria are still represented in the linked issue.
- [ ] Public library or CLI contracts are unchanged, or the issue explicitly authorizes the change.
- [ ] Provider-specific behavior remains inside the provider boundary.
- [ ] Artifact metadata, provider identity, ordering, transaction, and idempotence requirements were considered where applicable.
- [ ] Any intentional deviation from the PRD, design, or Sable parity audit is documented in the issue.

## Database and migration safety

- [ ] Migration artifacts and generated SQL were produced through the approved workflow.
- [ ] No production, staging, shared, or ambiguous database was used.
- [ ] Any database execution used an explicitly identified disposable/local database.
- [ ] The change does not expose credentials, connection strings, tokens, or other secrets.

## Verification evidence

Commands run:

```text
# List exact commands and results.
# Example: dotnet build
# Example: dotnet test
```

- [ ] Build passed.
- [ ] Relevant tests passed.
- [ ] Integration/container tests were run, or the blocking reason is documented.
- [ ] CLI arguments, output, and exit-code behavior were checked where applicable.
- [ ] Documentation, samples, and generated-output checks were run where applicable.

## Quality gates

Link the corresponding reports or issue comments when the project workflow assigns them.

- [ ] Destroyer/adversarial review completed, or not applicable with an explanation.
- [ ] Review-agent verdict is recorded as `SHIP IT`, or the maintainer has reviewed the deviation.
- [ ] Task-owned changes are committed and unrelated working-tree changes are excluded.
- [ ] Branch-size checkpoint was considered.

## Details for reviewers

### Breaking changes

<!-- Write "None" if there are no breaking changes. -->

### Risks, limitations, or accepted deviations

<!-- Include provider/database-specific risks and deferred work. Write "None" if applicable. -->

### Screenshots or artifacts

<!-- Add links or excerpts for CLI output, generated artifacts, or documentation changes. Remove if not applicable. -->

## Acceptance handoff

- [ ] The linked issue contains the original acceptance criteria and current implementation evidence.
- [ ] A `Ready for Acceptance Verification` checklist has been prepared for human review.
- [ ] I understand that tests, commits, and this PR are implementation evidence; final acceptance and issue disposition remain human decisions.

## Final checklist

- [ ] I have reviewed the complete diff and removed unrelated changes.
- [ ] I have added or updated tests for behavior changes.
- [ ] I have updated documentation when user-visible behavior changed.
- [ ] I have not closed the issue or applied a final disposition label as part of this PR.
