# Copilot Instructions — Competition Management System

This file configures GitHub Copilot to work with the agentic workflow for the Competition Management System.

## Before making any changes

1. **Read `AGENTS.md`** to understand layer boundaries, naming conventions, and code style.
2. **Read `architecture.json`** to see which layers exist and what they can depend on.
3. **Identify your target file's layer** and check what it's allowed to depend on.
4. **Match existing patterns** — read 2–3 files in the same layer before writing new code.

## The workflow

This project uses an **agentic engineering workflow** with six stages:

```
/grill-me → /to-spec → /to-tickets → /implement → /code-review → (commit manually)
```

**Key principles:**
- **Predictability comes from process.** Skills are written playbooks; they follow the same steps every time.
- **Rules are enforced by tooling.** Architecture boundaries live in `architecture.json` and are validated automatically.
- **One source of truth.** Conventions in `AGENTS.md`; structure in `architecture.json`. Everything else references them.
- **A human at every decision point.** Each stage hands control back to you.

## Slash commands (skills)

- **`/grill-me`** — Interview about a feature idea, decision, or requirement. Sharpen your thinking.
- **`/to-spec`** — Turn a grilled session into a formal spec (Epic) in `tickets/`.
- **`/to-tickets`** — Break an Epic into vertical-slice tickets.
- **`/implement`** — Implement one ticket end-to-end. Test-first, cross-layer, validation included. **No auto-commit.**
- **`/code-review`** — Review implementation against house standards and spec conformance.
- **`/teach`** — Learn .NET/C# concepts interactively. Integrated with `/implement`.

## Key files

| File | Purpose |
|------|---------|
| `AGENTS.md` | Canonical conventions (boundaries, naming, style, testing) |
| `architecture.json` | Machine-readable layer structure and rules |
| `scripts/validate_boundaries.py` | Validator (checks boundary violations) |
| `.github/skills/*/SKILL.md` | The six slash commands |
| `.github/agents/*.agent.md` | Specialist sub-agents (conductor, .NET specialist, testing specialist) |
| `tickets/` | Epic and ticket tracking (local markdown) |
| `docs/agents/issue-tracker.md` | How to structure tickets |
| `docs/agents/workflow-guide.md` | Detailed workflow guide |

## Architecture summary

**4-layer structure:**

| Layer | Path | Responsibility |
|-------|------|-----------------|
| **API** | `src/CompetitionManager.Api/` | HTTP endpoints, DTOs, thin controllers |
| **Application** | `src/CompetitionManager.Application/` | Use cases, orchestration, services |
| **Domain** | `src/CompetitionManager.Domain/` | Business logic, entities, rules (pure, no I/O) |
| **Infrastructure** | `src/CompetitionManager.Infrastructure/` | Data access, external integrations |

**Dependency flow:** API → Application → Domain ← Infrastructure

**Boundary rules:** 
- Domain never imports from Application, API, or Infrastructure.
- Infrastructure depends only on Domain (interfaces).
- Application depends only on Domain.
- API depends on Application and Domain.

Check with: `python3 scripts/validate_boundaries.py architecture.json --repo-root .`

## Specialist sub-agents

When `/implement` runs, these agents are available:

- **Conductor** — Orchestrates the workflow, ensures each stage completes properly.
- **`.NET/C# Specialist`** — Teaches architecture decisions, design patterns, idiomatic C#. Explains why Domain is pure, why Application orchestrates, why API is thin.
- **Testing Specialist** — Ensures test-first discipline, teaches MSTest patterns, audits coverage.

## Getting started

1. **Bootstrap the environment:** `./tools/agent-bootstrap.sh`
2. **Read `AGENTS.md`** to understand conventions.
3. **Read `docs/agents/workflow-guide.md`** to understand the pipeline.
4. **Start your first feature:** Type `/grill-me` and describe the feature.
5. **Follow the pipeline:** Each stage names the next one.

## Key constraints

- **No auto-commit:** You commit manually so you understand every change.
- **Test-first:** Every `/implement` starts by writing tests.
- **One ticket per session:** Fresh session per ticket; everything you need is in the ticket.
- **Architecture is law:** Boundary violations are caught by the validator and must be fixed.
- **Learning is the goal:** The `.NET/C# Specialist` and Testing Specialist teach as you implement.

## Questions?

- **How does the 4-layer architecture work?** → Read `AGENTS.md` and ask the `.NET/C# Specialist` with `/teach`.
- **How do I write tests?** → Ask the **Testing Specialist** during `/implement`, or use `/teach`.
- **What's the workflow?** → Read `docs/agents/workflow-guide.md`.
- **How do I structure a ticket?** → Read `docs/agents/issue-tracker.md`.
- **Is my code respecting boundaries?** → Run `python3 scripts/validate_boundaries.py architecture.json --repo-root .`.

## Credits

Skills adapted from Matt Pocock's `mattpocock/skills` (aihero.dev).
Architecture-governance spine, specialist agents, and instruction layering are original to this project.

---

**Next step:** Read `AGENTS.md`, then type `/grill-me` to start your first feature.
