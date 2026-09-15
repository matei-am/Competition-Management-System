# Tickets — Competition Management System

This folder contains all feature specifications (Epics) and implementation tickets for the Competition Management System.

## File structure

```
tickets/
├── EPIC-NNN-FeatureName.md    # Feature spec / Epic
├── CMS-NNN-TicketName.md      # Implementation ticket
├── README.md                   # This file
└── STATUS.md                   # Current workflow status (optional)
```

## How to use this folder

1. **Create an Epic:** Run `/to-spec` to formalize a grilled feature into `EPIC-NNN.md`.
2. **Create Tickets:** Run `/to-tickets` to break an Epic into `CMS-NNN.md` files.
3. **Implement:** Run `/implement CMS-NNN` to implement one ticket.
4. **Review & Commit:** Run `/code-review`, then commit manually.

## Status tracking

Update ticket status as you work:

```markdown
**Status:** Ready / In Progress / Blocked / Done
```

Track Epic status too:

```markdown
**Status:** Draft / Ready / In Progress / Done
```

## Example workflow

```
EPIC-001-CompetitionCrud.md
├── Status: Ready
├── CMS-001-DefineCompetitionAggregate.md (Status: Done)
├── CMS-002-BuildCompetitionRepository.md (Status: In Progress)
├── CMS-003-BuildCompetitionService.md (Status: Ready)
└── CMS-004-AddCompetitionEndpoints.md (Status: Ready)
```

For detailed ticket structure, see [docs/agents/issue-tracker.md](../agents/issue-tracker.md).

## Architecture reference

All tickets must respect:

- **`architecture.json`** — Layer definitions and boundary rules.
- **`AGENTS.md`** — Naming conventions, code style, testing discipline.

Before implementing, always read these files.

## Quick start

1. Read [docs/agents/workflow-guide.md](../agents/workflow-guide.md) to understand the pipeline.
2. Run `/grill-me` to start your first feature.
3. Follow the pipeline: `/grill-me` → `/to-spec` → `/to-tickets` → `/implement` → `/code-review` → commit.

## Questions?

- **How do I structure a ticket?** See [docs/agents/issue-tracker.md](../agents/issue-tracker.md).
- **What's the workflow?** See [docs/agents/workflow-guide.md](../agents/workflow-guide.md).
- **How do I learn .NET/C#?** Run `/teach` or ask the `.NET/C# Specialist` during `/implement`.
