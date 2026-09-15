# EPIC-001: Competition CRUD

**Status:** Ready
**Created:** 2026-09-12

## User Story

As a competition organizer, I want to create, read, update, and delete competitions, so that I can manage my competitive events and their details throughout their lifecycle.

## Acceptance Criteria

- [ ] Competitions can be created with name, date, location, athlete/club/referee counts, rules, and custom weight/age categories
- [ ] Competitions start in "Upcoming" status (or any status)
- [ ] Competitions can be retrieved individually by ID
- [ ] Competitions can be listed (all or filtered)
- [ ] Competitions can be updated with status-dependent field constraints
- [ ] Competitions can be deleted in any status
- [ ] Status transitions are one-way: Upcoming → Active → Finished
- [ ] Name updates only allowed in Upcoming status
- [ ] Rule updates only allowed in Upcoming & Active (not Finished)
- [ ] Count updates: by registration if Upcoming, manual if Active/Finished
- [ ] All operations respect domain business rules (name uniqueness, min 4 counts, etc.)
- [ ] All layers are covered: Domain (entity), Application (services), Infrastructure (repository), API (endpoints)

## Domain Model Changes

### New Entities

**Competition** (aggregate root)
- `Id`: unique system-generated identifier (GUID)
- `Name`: string, unique, required, no max length
- `Date`: DateTime, required
- `Location`: string, required
- `Status`: enum (Upcoming, Active, Finished)
- `AthleteCount`: int, min 4, no max
- `ClubCount`: int, min 4, no max
- `RefereeCount`: int, min 4, no max
- `Rules`: string (text document with fixed + custom rules)
- `WeightCategories`: collection of category definitions (per-competition)
- `AgeCategories`: collection of category definitions (per-competition)
- `CreatedAt`: DateTime (system-generated)
- `UpdatedAt`: DateTime (system-generated)

### Business Rules (Invariants)

1. Name must be unique across all competitions
2. Status transitions only forward: Upcoming → Active → Finished (no backward moves)
3. Fields updatable only at specific statuses:
   - Name: Upcoming only
   - Rules: Upcoming & Active (not Finished)
   - Counts: by registration (Upcoming) or manual (Active/Finished)
4. Min count constraints: Athletes ≥ 4, Clubs ≥ 4, Referees ≥ 4
5. Categories are defined per-competition (not system-wide)
6. Deletable in any status

## Layer Impact

| Layer | Changes |
|-------|---------|
| **Domain** | New `Competition` aggregate root in `src/CompetitionManager.Domain/Entities/`. Include `CompetitionStatus` enum as value object. Domain services to enforce status transitions and validation rules. |
| **Application** | New `CompetitionService` in `src/CompetitionManager.Application/Services/`. Implement use cases: `CreateCompetition`, `GetCompetitionById`, `GetAllCompetitions`, `UpdateCompetition`, `DeleteCompetition`. Orchestrate Domain and call repositories. |
| **Infrastructure** | New `ICompetitionRepository` interface in Domain. Implement `CompetitionRepository` in `src/CompetitionManager.Infrastructure/Repositories/`. Handle data persistence and queries. |
| **API** | New `CompetitionsController` in `src/CompetitionManager.Api/Controllers/`. Endpoints: `GET /api/competitions`, `GET /api/competitions/{id}`, `POST /api/competitions`, `PUT /api/competitions/{id}`, `DELETE /api/competitions/{id}`. Map DTOs to domain objects. |

## Technical Notes

- **Testing:** Test-first. Write tests for every public API in Domain and Application layers. Use MSTest.
- **Architecture:** Respect 4-layer boundaries (API → Application → Domain ← Infrastructure). Domain must be pure (no I/O).
- **Validation:** All business rules enforced in Domain layer (fail fast in constructor and methods).
- **Status machine:** Use explicit state transition logic; prevent invalid moves.
- **Categories:** Categories are value objects or nested entities within Competition (not separate aggregates yet).
- **Persistence:** TBD (SQL Server, SQLite, or EF Core approach — decide during `/implement`).
- **No registrations yet:** Athlete/Club/Referee counts are initialized manually; registration tracking is a separate feature.

## Linked Tickets

These vertical-slice tickets will be created by `/to-tickets`:

- **CMS-001:** Define Competition aggregate (Domain layer only)
- **CMS-002:** Implement CompetitionRepository interface & in-memory impl (Infrastructure)
- **CMS-003:** Build CompetitionService use cases (Application layer)
- **CMS-004:** Add Competition API endpoints (API layer)
- **CMS-005:** Integration tests & validation

## Open Questions

None — ready to break into tickets.

---

**Next step:** Run `/to-tickets` to create vertical-slice implementation tickets.
