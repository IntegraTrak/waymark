# Waymark Marten sample

This sample exposes two controlled model states without requiring Git discovery:

- `ModelStates.CreateEarlier()` registers `CustomerV1`.
- `ModelStates.CreateLater()` registers `CustomerV2`, which adds `Email`.

The Marten provider task uses these inputs with the native Marten tooling and a disposable PostgreSQL environment.
