# Issue Tracker & Ticket Structure — Competition Management System

This document defines how tickets and epics are managed in the `tickets/` folder. All work is tracked here using local markdown files.

## Directory structure

```
tickets/
├── EPIC-NNN-FeatureName.md          # Epic (feature spec)
├── CMS-NNN-TicketName.md            # Ticket (implementation slice)
├── README.md                         # This file
└── STATUS.md                         # Current workflow status (optional)
```

## Epic structure

**File naming:** `EPIC-NNN-FeatureName.md` (e.g., `EPIC-001-CompetitionCrud.md`)

**Metadata:**
- **Status:** Draft, Ready, In Progress, Done
- **Description:** One-sentence feature summary
- **Created:** Date
- **Owner:** (optional) who is leading this

**Sections:**
- User story (as a [role], I want [goal], so that [benefit])
- Acceptance criteria (checklist, each must be testable)
- Domain model changes (new entities, rules, etc.)
- Layer impact (which layers are touched and how)
- Technical notes (constraints, dependencies, risks)
- Linked tickets (which CMS-NNN tickets compose this Epic)

**Example:**

```markdown
# EPIC-001: Competition CRUD

**Status:** Ready
**Created:** 2026-09-12

## User Story
As a competition organizer, I want to create, read, update, and delete competitions, 
so that I can manage my events.

## Acceptance Criteria
- [ ] Competitions can be created with name, date, location, max participants
- [ ] Competitions can be listed and filtered
- [ ] Individual competitions can be retrieved by ID
- [ ] Competitions can be updated
- [ ] Competitions can be deleted
- [ ] All operations respect domain rules (e.g., no negative participant counts)

## Domain Model Changes
- New entity: `Competition` aggregate root
- Properties: Name, StartDate, Location, MaxParticipants, CurrentParticipants, Status
- Rules: Name is required; MaxParticipants > 0; can't delete if in progress

## Layer Impact
| Layer | Changes |
|-------|---------|
| **API** | GET/POST/PUT/DELETE /api/competitions endpoints |
| **Application** | CompetitionService: Create, GetAll, GetById, Update, Delete |
| **Domain** | Competition entity, business rules |
| **Infrastructure** | CompetitionRepository implementation, EF/SQL queries |

## Technical Notes
- No external dependencies at launch
- Database: SQL (TBD: SQL Server vs SQLite for dev)
- Testing: 100% coverage of Domain, >80% Application, >50% API

## Linked Tickets
- CMS-001: Define Competition aggregate
- CMS-002: Implement CompetitionRepository
- CMS-003: Build CompetitionService use cases
- CMS-004: Add API endpoints
```

## Ticket structure

**File naming:** `CMS-NNN-TicketName.md` (e.g., `CMS-001-DefineCompetitionAggregate.md`)

**Metadata:**
- **Status:** Ready, In Progress, Blocked, Done
- **Epic:** EPIC-NNN (which Epic this belongs to)
- **Blocked by:** CMS-XXX (if any dependency)
- **Created:** Date
- **Assigned to:** (optional) who is implementing

**Sections:**
- Objective (one sentence: what does this ticket accomplish?)
- Acceptance criteria (checklist, each testable)
- Layer breakdown (what changes in each layer)
- Implementation notes (hints, gotchas, patterns to use)
- Testing (specific tests to write)
- Definition of Done (checklist for completion)

**Example:**

```markdown
# CMS-001: Define Competition Aggregate

**Status:** Ready
**Epic:** EPIC-001
**Blocked by:** (none)
**Created:** 2026-09-12

## Objective
Define the Competition aggregate root with business rules and invariants in the Domain layer.

## Acceptance Criteria
- [ ] Competition entity is defined with properties: Name, StartDate, Location, MaxParticipants, Status
- [ ] Business rule: Name is required (not null/empty)
- [ ] Business rule: MaxParticipants > 0
- [ ] Business rule: Status is an enum (Planned, InProgress, Completed, Cancelled)
- [ ] Domain tests verify all rules are enforced
- [ ] No external dependencies (Domain is pure)

## Layer Breakdown

### Domain
- New file: `src/CompetitionManager.Domain/Entities/Competition.cs`
- New file: `src/CompetitionManager.Domain/ValueObjects/CompetitionStatus.cs`
- Validations in constructor and method guards

## Implementation Notes
- Use value object for Status (not string enum) for type safety
- Fail fast in constructor: throw ArgumentException if invariants violated
- Keep Competition simple: name, date, location, max, status. Participants are a separate aggregate.

## Testing
Add to `tests/CompetitionManager.Tests/Domain/Entities/`:
- `CompetitionTests.cs`
  - `Constructor_WithValidInput_CreatesCompetition`
  - `Constructor_WithNullName_ThrowsArgumentException`
  - `Constructor_WithZeroMaxParticipants_ThrowsArgumentException`
  - `Status_DefaultsToPlanned`

## Definition of Done
- [ ] Competition.cs is written with constructor validation
- [ ] CompetitionStatus.cs is written as value object or enum
- [ ] All domain tests pass: `dotnet test`
- [ ] No compiler warnings
- [ ] Architecture validator passes: `python3 scripts/validate_boundaries.py architecture.json --repo-root .`
- [ ] Code follows AGENTS.md naming and style conventions
- [ ] **NOT yet committed** (you will commit after code review)
```

## Status labels

Use these labels in ticket metadata to track progress:

| Status | Meaning |
|--------|---------|
| **Ready** | Ticket is defined, dependencies are clear, ready to implement |
| **In Progress** | Someone is actively implementing this ticket |
| **Blocked** | Waiting on a dependency (CMS-XXX) or external event |
| **Done** | Implemented, tested, reviewed, and committed |

## Ticket dependencies

Use "blocked by" to link tickets with ordering constraints:

```markdown
**Blocked by:** CMS-001, CMS-002
```

This means: implement CMS-001 and CMS-002 before starting this ticket.

**Rule:** Always order tickets from Domain → Application → Infrastructure → API. A ticket implementing Domain should not be blocked by Application.

## Workflow: from Epic to Ticket to Code

1. **Epic created:** `/to-spec` creates `EPIC-NNN.md` with full spec.
2. **Tickets created:** `/to-tickets` creates `CMS-NNN.md` files with vertical slices.
3. **Ticket ready:** Mark status as "Ready".
4. **Implement:** `/implement` runs in a fresh session for one ticket.
5. **Review:** `/code-review` audits the implementation.
6. **Commit:** User commits manually after review.
7. **Mark done:** Update ticket status to "Done".

## Quick reference

- **Epic file:** `EPIC-NNN-<name>.md`
- **Ticket file:** `CMS-NNN-<name>.md`
- **Ticket naming:** Verb-first (e.g., "Define", "Build", "Add", "Test")
- **Status check:** `grep "^| Status" tickets/*.md` to see all statuses
- **Dependency check:** Ensure tickets are ordered: Domain → Application → Infrastructure → API
