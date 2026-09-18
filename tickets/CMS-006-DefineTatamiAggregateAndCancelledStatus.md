# CMS-006: Define Tatami Aggregate and Cancelled Status

**Epic:** EPIC-004
**Status:** In Progress
**Blocked by:** (none)

## Objective

Define the `Tatami` aggregate root, add `Cancelled` to `CompetitionStatus`, and add the `Competition` domain logic that decides between activating and auto-cancelling based on Tatami count — all in the Domain layer.

## Acceptance Criteria

- [ ] `Tatami` entity created as its own aggregate root with `Id`, `CompetitionId`, `Number`, `CreatedAt`, `UpdatedAt`
- [ ] `Tatami` constructor validates: `CompetitionId` is not empty, `Number` is a positive integer
- [ ] `Tatami` exposes a way to change its `Number` (for renumbering after removal) without allowing it to become non-positive
- [ ] `CompetitionStatus` enum gains a `Cancelled` value
- [ ] `Competition` gains an `ActivateOrCancel(int tatamiCount)` method: if `tatamiCount < 1`, transitions `Upcoming → Cancelled`; otherwise transitions `Upcoming → Active` (reusing existing forward-transition validation)
- [ ] `ActivateOrCancel` throws if called from any status other than `Upcoming`
- [ ] `Cancelled` is terminal: `TransitionStatus` and `ActivateOrCancel` both throw if current `Status` is `Cancelled`
- [ ] Existing `Competition.TransitionStatus` behavior (Active → Finished, one-step-forward) is unchanged
- [ ] Domain layer has zero dependencies on Application, API, or Infrastructure
- [ ] Comprehensive unit tests for `Tatami` and the new `Competition` behavior
- [ ] Architecture validator passes: `python3 scripts/validate_boundaries.py architecture.json --repo-root .`

## Layer Breakdown

### Domain
- **New file:** `src/CompetitionManager.Domain/Entities/Tatami.cs`
  - Properties: `Id` (Guid), `CompetitionId` (Guid), `Number` (int), `CreatedAt`, `UpdatedAt`
  - Constructor validation: `CompetitionId != Guid.Empty`, `Number >= 1`
  - Method: `Renumber(int newNumber)` — validates `newNumber >= 1`, updates `Number` and `UpdatedAt`
- **New file:** `src/CompetitionManager.Domain/Repositories/ITatamiRepository.cs`
  - `Task<Tatami> AddAsync(Tatami tatami)`
  - `Task<Tatami?> GetByIdAsync(Guid id)`
  - `Task<IReadOnlyList<Tatami>> GetByCompetitionIdAsync(Guid competitionId)`
  - `Task RemoveAsync(Guid id)`
  - `Task<int> CountByCompetitionIdAsync(Guid competitionId)`
- **Modified file:** `src/CompetitionManager.Domain/ValueObjects/CompetitionStatus.cs`
  - Add `Cancelled` as the last enum value
- **Modified file:** `src/CompetitionManager.Domain/Entities/Competition.cs`
  - Add `ActivateOrCancel(int tatamiCount)`:
    - Throws `InvalidOperationException` if `Status != CompetitionStatus.Upcoming`
    - If `tatamiCount < 1`: sets `Status = CompetitionStatus.Cancelled`, calls `Touch()`
    - Else: delegates to the existing one-step-forward transition to `Active`
  - Guard `TransitionStatus` so it throws if `Status == CompetitionStatus.Cancelled` (terminal state, no forward moves out of it)

## Implementation Notes

- **Why a separate method instead of reusing `TransitionStatus` for Cancelled:** `Cancelled` is not reachable via the generic "current + 1" rule used for Upcoming→Active→Finished, since it's a special case only reachable from `Upcoming`. Keep `TransitionStatus` for the normal forward path and add `ActivateOrCancel` as the explicit, intention-revealing entry point used when moving out of `Upcoming`.
- **No cross-aggregate calls:** `Competition` does not query `ITatamiRepository` itself — the Application layer (CMS-008) resolves the Tatami count and passes it in.
- **Tatami numbering:** This ticket only defines `Renumber`; the actual renumber-on-remove orchestration (sequential in Upcoming, stable in Active) happens in the Application layer (CMS-008).
- **No I/O in Domain:** Everything validated in-memory; `ITatamiRepository` is an interface only (no implementation) here.

## Testing

**Files:**
- `tests/CompetitionManager.Tests/Domain/Entities/TatamiTests.cs`
- `tests/CompetitionManager.Tests/Domain/Entities/CompetitionTests.cs` (extend existing file)

**Tests to write (AAA pattern):**

```
TatamiTests
├── Constructor_WithValidInput_CreatesTatami
├── Constructor_WithEmptyCompetitionId_ThrowsArgumentException
├── Constructor_WithNonPositiveNumber_ThrowsArgumentException
├── Renumber_WithPositiveNumber_UpdatesNumber
└── Renumber_WithNonPositiveNumber_ThrowsArgumentException

CompetitionTests (additions)
├── ActivateOrCancel_FromUpcomingWithTatamis_TransitionsToActive
├── ActivateOrCancel_FromUpcomingWithZeroTatamis_TransitionsToCancelled
├── ActivateOrCancel_FromActive_ThrowsException
├── ActivateOrCancel_FromCancelled_ThrowsException
├── TransitionStatus_FromCancelled_ThrowsException
```

## Definition of Done

- [ ] `Tatami.cs`, `ITatamiRepository.cs` written
- [ ] `CompetitionStatus.Cancelled` added
- [ ] `Competition.ActivateOrCancel` implemented and `TransitionStatus` guarded against `Cancelled`
- [ ] All unit tests passing: `dotnet test tests/CompetitionManager.Tests/`
- [ ] No compiler warnings
- [ ] Architecture validator passes
- [ ] Code follows `AGENTS.md` naming conventions
- [ ] **NOT committed or pushed** (you will commit after code review)

---

**Next ticket (after code review & commit):** CMS-007 — Implement TatamiRepository
