## Summary

<!-- Describe the problem and the change. Keep this focused on the linked issue. -->

**Related issue:** #

## Change type

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

## Contract and safety review

- [ ] The PR is limited to the linked issue and does not invent unrelated scope.
- [ ] Public library or CLI contracts are unchanged, or the issue explicitly authorizes the change.
- [ ] Provider-specific behavior remains inside the provider boundary.
- [ ] Migration artifacts and generated SQL were produced through the approved workflow.
- [ ] Artifact ordering, provider identity, transaction, and idempotence requirements were considered where applicable.
- [ ] No production, staging, shared, or ambiguous database was used.
- [ ] Any database execution used an explicitly identified disposable/local database.
- [ ] Breaking changes, accepted deviations, and follow-up work are documented below.

## Verification

Commands run:

```text
# Example:
# dotnet build
# dotnet test
```

- [ ] Build passed.
- [ ] Relevant tests passed.
- [ ] Integration/container tests were run, or the blocking reason is documented.
- [ ] CLI arguments, output, and exit-code behavior were checked where applicable.
- [ ] Documentation, samples, and generated-output checks were run where applicable.

## Details for reviewers

### Breaking changes

<!-- Write "None" if there are no breaking changes. -->

### Risks, limitations, or accepted deviations

<!-- Include provider/database-specific risks and anything deferred. Write "None" if applicable. -->

### Screenshots or artifacts

<!-- Add links or excerpts for CLI output, generated artifacts, or documentation changes. Remove this section if not applicable. -->

## Final checklist

- [ ] I have not included credentials, connection strings, tokens, or other secrets.
- [ ] I have added or updated tests for behavior changes.
- [ ] I have updated documentation when user-visible behavior changed.
- [ ] I have reviewed the complete diff and removed unrelated changes.
