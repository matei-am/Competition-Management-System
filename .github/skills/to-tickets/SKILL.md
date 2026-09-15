---
name: to-tickets
description: "Break a spec (Epic) into vertical-slice tickets with dependencies. Create tickets in `tickets/` folder."
disable-model-invocation: true
---

Break the Epic spec into **tracer-bullet** (vertical-slice) tickets. Each ticket should:
- Be completable in one fresh session (one context window)
- Pierce all layers (API → Application → Domain → Infrastructure)
- Have clear acceptance criteria
- Have explicit "blocked by" dependencies

The philosophy: vertical slices, not horizontal layers. Each ticket is a thin end-to-end slice, not a full-layer sprint.

## Process

1. **Analyze the Epic.** Read the spec, domain model changes, and layer impact.
2. **Identify tracer bullets.** Break it into the smallest vertical slices. Example for "Add Competition Management":
   - CMS-001: Define Competition aggregate (Domain layer only).
   - CMS-002: Create CompetitionRepository interface and in-memory impl (Domain + Infrastructure).
   - CMS-003: Build CompetitionService use cases (Application layer).
   - CMS-004: Add GET/POST /api/competitions endpoints (API layer).
   - CMS-005: Add tests for all layers.
3. **Create ticket files.** Save each as `tickets/CMS-NNN-TicketName.md`.
4. **Order with dependencies.** Use "blocked by" edges to ensure a fresh agent can start with CMS-001 and finish CMS-NNN without surprises.
5. **Hand off.** Name the next stage: **"Tickets are ready. Run `/implement` in a fresh session for each ticket, starting with CMS-001."**

## Ticket template

```markdown
# CMS-NNN: <Ticket Title>

**Epic:** EPIC-NNN (from spec)
**Status:** Ready to implement
**Blocked by:** CMS-XXX (if any)

## Objective
One sentence: what does this ticket accomplish?

## Acceptance Criteria
- [ ] <Testable criterion>
- [ ] <Testable criterion>
- [ ] Architecture boundaries are respected
- [ ] Tests pass: `dotnet test`

## Layer Breakdown

### Domain (if applicable)
- New entities: <list>
- New rules: <list>
- New repository interfaces: <list>

### Application (if applicable)
- New services: <list>
- New use cases: <list>

### Infrastructure (if applicable)
- New repository implementations: <list>
- New external integrations: <list>

### API (if applicable)
- New endpoints: <list>
- New DTOs: <list>

## Implementation Notes
- Test-first: write tests before code.
- Reference `AGENTS.md` for naming conventions and layer boundaries.
- Run `python3 scripts/validate_boundaries.py architecture.json --repo-root .` to verify layers.

## Testing
Add tests to `tests/CompetitionManager.Tests/` mirroring the layer structure.
Naming: `<TargetClass>Tests.cs` with `<Method>_<Scenario>_<Expected>` test methods.

## Definition of Done
- Code written and tests passing
- Architecture boundaries validated
- No commits yet (you will commit manually)
```

## Principles for tracer bullets

- **Vertical, not horizontal.** A ticket should touch multiple layers, not concentrate on one layer.
- **Dependency order.** Domain first, then Application, then API. Infrastructure can go with Application.
- **One context.** Each ticket must fit in one session. If it doesn't, split it further.
- **Acceptance criteria are testable.** Not "code is written" but "CompetitionRepository returns a Competition with all properties set."
