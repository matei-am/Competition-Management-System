# CMS-001: Define Competition Aggregate

**Epic:** EPIC-001
**Status:** Ready to implement
**Blocked by:** (none)

## Objective

Define the Competition aggregate root with business rules, invariants, and value objects in the Domain layer. This is the foundation for all other tickets.

## Acceptance Criteria

- [ ] `Competition` entity created as aggregate root with all properties (Id, Name, Date, Location, Status, Counts, Rules, Categories, Timestamps)
- [ ] `CompetitionStatus` enum (or value object) with states: Upcoming, Active, Finished
- [ ] Constructor enforces invariants: name is required/non-empty, counts ≥ 4, name is validated
- [ ] Status transition logic: only allows forward moves (Upcoming → Active → Finished)
- [ ] Business rule methods (CanUpdateName, CanUpdateRules, CanChangeCount, etc.)
- [ ] Category value objects or nested collections defined
- [ ] All business rules are validated in the Domain (fail fast)
- [ ] Domain layer has zero dependencies on Application, API, or Infrastructure
- [ ] Comprehensive unit tests in `tests/CompetitionManager.Tests/Domain/Entities/CompetitionTests.cs`
- [ ] Architecture validator passes: `python3 scripts/validate_boundaries.py architecture.json --repo-root .`

## Layer Breakdown

### Domain
- **New file:** `src/CompetitionManager.Domain/Entities/Competition.cs`
  - Properties: Id, Name, Date, Location, Status, AthleteCount, ClubCount, RefereeCount, Rules, WeightCategories, AgeCategories, CreatedAt, UpdatedAt
  - Constructor with full validation
  - Methods: CanUpdateName(), CanUpdateRules(), CanChangeCount(), TransitionStatus(), UpdateName(), UpdateRules(), etc.
- **New file:** `src/CompetitionManager.Domain/ValueObjects/CompetitionStatus.cs`
  - Enum or class representing Upcoming/Active/Finished
- **New file (optional):** `src/CompetitionManager.Domain/ValueObjects/CategoryDefinition.cs`
  - Value object for weight/age categories

## Implementation Notes

- **Status machine:** Use explicit validation; throw `InvalidOperationException` if status transition is invalid
- **Constructor:** Fail fast — validate all invariants in constructor. Throw `ArgumentException` for invalid inputs
- **Name uniqueness:** For now, just document in the domain model. Repository will enforce at the database level (handled in CMS-002)
- **Categories:** Can be simple value objects (name, description) or a more complex type. Keep it minimal for now.
- **No I/O:** Domain must not call repositories, services, or external APIs. Everything is in-memory validation.

## Testing

**File:** `tests/CompetitionManager.Tests/Domain/Entities/CompetitionTests.cs`

**Tests to write (AAA pattern):**

```
CompetitionTests
├── Constructor_WithValidInput_CreatesCompetition
├── Constructor_WithNullName_ThrowsArgumentException
├── Constructor_WithEmptyName_ThrowsArgumentException
├── Constructor_WithAthletesCountLessThan4_ThrowsArgumentException
├── Constructor_WithClubsCountLessThan4_ThrowsArgumentException
├── Constructor_WithRefereesCountLessThan4_ThrowsArgumentException
├── Status_DefaultsToUpcoming (or as specified)
├── TransitionStatus_FromUpcomingToActive_Succeeds
├── TransitionStatus_FromActiveToFinished_Succeeds
├── TransitionStatus_FromUpcomingToFinished_ThrowsException (no backward)
├── TransitionStatus_BackwardMove_ThrowsException
├── CanUpdateName_InUpcomingStatus_ReturnsTrue
├── CanUpdateName_InActiveStatus_ReturnsFalse
├── CanUpdateName_InFinishedStatus_ReturnsFalse
├── CanUpdateRules_InUpcomingStatus_ReturnsTrue
├── CanUpdateRules_InActiveStatus_ReturnsTrue
├── CanUpdateRules_InFinishedStatus_ReturnsFalse
├── CanChangeCount_InUpcomingStatus_ReturnsTrue
├── CanChangeCount_InActiveStatus_ReturnsFalse
└── UpdateName_InUpcomingStatus_UpdatesSuccessfully
```

## Definition of Done

- [ ] Competition.cs written with full constructor validation
- [ ] CompetitionStatus enum/value object created
- [ ] All business rule methods implemented
- [ ] All unit tests passing: `dotnet test tests/CompetitionManager.Tests/`
- [ ] No compiler warnings
- [ ] Architecture validator passes: `python3 scripts/validate_boundaries.py architecture.json --repo-root .`
- [ ] Code follows `AGENTS.md` naming conventions (PascalCase, no external dependencies)
- [ ] **NOT committed or pushed** (you will commit after code review)

---

**Next ticket (after code review & commit):** CMS-002 — Implement CompetitionRepository
