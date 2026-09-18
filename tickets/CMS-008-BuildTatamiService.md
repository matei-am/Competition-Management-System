# CMS-008: Build TatamiService Vertical Slice

**Epic:** EPIC-004
**Status:** Ready to implement
**Blocked by:** CMS-007

## Objective

Implement the Tatami add/remove/list use cases as one vertical slice: DTOs, `TatamiService`, the cross-aggregate orchestration for status-dependent rules (min-1 floor, renumbering vs. stable numbering, auto-cancel-on-zero-Tatamis), DI wiring, and service tests using the real in-memory repositories.

## Acceptance Criteria

- [ ] `TatamiService` created in `src/CompetitionManager.Application/Services/TatamiService.cs`, depending on `ITatamiRepository` and `ICompetitionRepository`
- [ ] `AddTatamiAsync(competitionId)`:
  - Throws if the Competition does not exist (`NotFoundException`)
  - Throws if the Competition's status is not `Upcoming` or `Active`
  - Computes the next sequential `Number` (current max + 1, or 1 if none exist) and persists the new Tatami
- [ ] `RemoveTatamiAsync(competitionId, tatamiId)`:
  - Throws if the Competition or Tatami does not exist, or the Tatami does not belong to the Competition
  - Throws if the Competition's status is not `Upcoming` or `Active`
  - Throws if removal would bring the count below 1 while `Upcoming` or `Active`
  - If Competition status is `Upcoming`: removes the Tatami and renumbers the remaining ones sequentially (1..N)
  - If Competition status is `Active`: removes the Tatami and leaves remaining numbers unchanged
- [ ] `GetTatamisForCompetitionAsync(competitionId)` returns all Tatamis for a competition ordered by `Number`, throwing `NotFoundException` if the Competition does not exist
- [ ] `ActivateCompetitionAsync(competitionId)` (or equivalent use case) resolves the current Tatami count from `ITatamiRepository` and calls `Competition.ActivateOrCancel(tatamiCount)`, persisting the resulting status
- [ ] `TatamiService` and `TatamiRepository` are registered in `src/CompetitionManager.Api/Program.cs`
- [ ] Service tests use the real in-memory repositories and cover success and failure paths
- [ ] Architecture boundaries pass validation

## Layer Breakdown

### Domain
- Use the existing `Tatami`, `Competition`, `CompetitionStatus`, `ITatamiRepository`, and `ICompetitionRepository` contracts.
- No new Domain code; this ticket is pure orchestration.

### Infrastructure
- Use the existing `TatamiRepository` and `CompetitionRepository` implementations through their interfaces.
- Do not reference Infrastructure types from `TatamiService`.

### Application
- Add `src/CompetitionManager.Application/Services/TatamiService.cs` (and `ITatamiService` interface).
- Add `src/CompetitionManager.Application/DTOs/TatamiResponse.cs`.
- Reuse the existing `NotFoundException`.

### API
- Register `ICompetitionRepository`/`ITatamiRepository` and `ITatamiService`/`TatamiService` in `Program.cs`.
- Do not add controllers or endpoint behavior; those belong to CMS-009.

## Implementation Notes

- **Orchestration owns cross-aggregate rules:** Since `Tatami` and `Competition` are separate aggregates, the min-1 floor, renumber-vs-stable-numbering branch, and auto-cancel decision all live in `TatamiService`, not in either Domain entity individually.
- **Renumbering:** After removing a Tatami while `Upcoming`, fetch the remaining Tatamis ordered by `Number` and call `Renumber` sequentially (1, 2, 3...) via the repository.
- **Stable numbering:** While `Active`, do not call `Renumber` on remaining Tatamis after a removal.
- **Auto-cancel:** `ActivateOrCancel` is a Domain method; this ticket wires the Application-level use case that reads the count and invokes it, then persists the updated Competition via `ICompetitionRepository`.
- **Not found:** Translate missing Competition/Tatami lookups into `NotFoundException`, consistent with `CompetitionService`.
- **Async/await:** Preserve asynchronous repository calls with `async`/`await`.

## Testing

**File:** `tests/CompetitionManager.Tests/Application/Services/TatamiServiceTests.cs`

**Tests to write (AAA pattern):**

```
TatamiServiceTests
├── AddTatami_ToUpcomingCompetition_Succeeds
├── AddTatami_ToActiveCompetition_Succeeds
├── AddTatami_ToFinishedCompetition_ThrowsException
├── AddTatami_ToCancelledCompetition_ThrowsException
├── AddTatami_ToNonExistentCompetition_ThrowsNotFoundException
├── AddTatami_AssignsSequentialNumbers
├── RemoveTatami_LastOneInUpcoming_ThrowsException
├── RemoveTatami_LastOneInActive_ThrowsException
├── RemoveTatami_InUpcoming_RenumbersRemainingSequentially
├── RemoveTatami_InActive_KeepsRemainingNumbersStable
├── RemoveTatami_InFinished_ThrowsException
├── RemoveTatami_NotBelongingToCompetition_ThrowsException
├── GetTatamisForCompetition_ReturnsOrderedByNumber
├── GetTatamisForCompetition_NonExistentCompetition_ThrowsNotFoundException
├── ActivateCompetition_WithAtLeastOneTatami_TransitionsToActive
└── ActivateCompetition_WithZeroTatamis_TransitionsToCancelled
```

## Definition of Done

- [ ] Application project references Domain and contains no Infrastructure dependency
- [ ] Request/response DTOs, service, and interface are implemented
- [ ] API DI wiring resolves the service and repositories without controllers
- [ ] Service tests pass: `dotnet test tests/CompetitionManager.Tests/`
- [ ] Full build succeeds without compiler warnings
- [ ] Architecture validator passes: `python3 scripts/validate_boundaries.py architecture.json --repo-root .`
- [ ] Code follows `AGENTS.md` naming and conventions
- [ ] **NOT committed or pushed**

---

**Next ticket (after code review & commit):** CMS-009 — Add Tatamis API Endpoints
