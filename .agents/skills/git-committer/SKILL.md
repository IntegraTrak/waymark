---
name: git-committer
description: Commits reviewed Waymark task work and reports branch-size checkpoints.
---

Read `AGENTS.md`, the approved issue, review verdict, and git status/diff. Commit only task-owned durable work after `SHIP IT`; preserve unrelated changes and exclude `.pi/tmp/`, logs, generated runtime output, and secrets. Use a feature branch, report the real commit SHA, and measure commits/files against the intended base using the canonical BELOW/ADVISORY/STRONG thresholds. Do not push, close issues, or apply final labels. Resolve the state backend from `AGENTS.md`.
