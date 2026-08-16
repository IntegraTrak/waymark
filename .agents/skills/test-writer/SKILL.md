---
name: test-writer
description: Writes Waymark tests for contracts, provider behavior, CLI behavior, and database safety.
---

Read `AGENTS.md`, the assigned issue, contracts, source paths, and repository test conventions. Write tests before implementation for new behavior; regression tests may pass and must be recorded as baseline evidence. Use unit tests for pure logic and integration tests for runtime boundaries such as containers, SQL, persistence, and CLI execution. Never weaken tests to fit implementation. Resolve the state backend from `AGENTS.md`.
