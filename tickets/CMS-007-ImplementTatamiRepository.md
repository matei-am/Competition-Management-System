# CMS-007: Implement TatamiRepository

**Epic:** EPIC-004
**Status:** Ready to implement
**Blocked by:** CMS-006

## Objective

Implement an in-memory `TatamiRepository` in Infrastructure, satisfying the `ITatamiRepository` contract defined in CMS-006.

## Acceptance Criteria

- [ ] `TatamiRepository` implementation created in `src/CompetitionManager.Infrastructure/Repositories/TatamiRepository.cs`
- [ ] In-memory store (dictionary) holds Tatamis, independent of the Competition repository's storage
- [ ] `AddAsync` stores a new Tatami and returns it
- [ ] `GetByIdAsync` returns the Tatami or `null` if not found
- [ ] `GetByCompetitionIdAsync` returns all Tatamis for a competition, ordered by `Number`
- [ ] `RemoveAsync` removes a Tatami by id; throws `KeyNotFoundException` if not found
- [ ] `CountByCompetitionIdAsync` returns the current count of Tatamis for a competition (0 if none)
- [ ] Async methods used throughout (Task/Task<T>) for future database compatibility
- [ ] Comprehensive tests in `tests/CompetitionManager.Tests/Infrastructure/Repositories/TatamiRepositoryTests.cs`
- [ ] Architecture validator passes

## Layer Breakdown

### Domain
- No changes. Uses `Tatami` and `ITatamiRepository` from CMS-006.

### Infrastructure
- **New file:** `src/CompetitionManager.Infrastructure/Repositories/TatamiRepository.cs`
  - Implements `ITatamiRepository`
  - In-memory `Dictionary<Guid, Tatami>` as backing store
  - `GetByCompetitionIdAsync` filters by `CompetitionId` and orders by `Number`

## Implementation Notes

- **Repository pattern:** Mirror the style of `CompetitionRepository` (CMS-002) for consistency.
- **In-memory storage:** Simple Dictionary; no database yet.
- **Not-found handling:** `GetByIdAsync` returns `null` (nullable) rather than throwing, since callers (Application layer) need to distinguish "not found" from "found" without exception-driven control flow for reads. `RemoveAsync` throws `KeyNotFoundException` since removal of a nonexistent Tatami is a caller error.
- **Independent of Competition storage:** This ticket does not modify `CompetitionRepository`; Tatami is a separate aggregate with its own store.

## Testing

**File:** `tests/CompetitionManager.Tests/Infrastructure/Repositories/TatamiRepositoryTests.cs`

**Tests to write (AAA pattern):**

```
TatamiRepositoryTests
├── AddTatami_WithValidInput_StoresSuccessfully
├── GetById_WithValidId_ReturnsTatami
├── GetById_WithInvalidId_ReturnsNull
├── GetByCompetitionId_WithNoTatamis_ReturnsEmptyList
├── GetByCompetitionId_WithMultipleTatamis_ReturnsAllOrderedByNumber
├── GetByCompetitionId_WithTatamisFromOtherCompetitions_ExcludesThem
├── Remove_WithValidId_RemovesTatami
├── Remove_WithInvalidId_ThrowsKeyNotFoundException
├── CountByCompetitionId_WithNoTatamis_ReturnsZero
└── CountByCompetitionId_WithMultipleTatamis_ReturnsCorrectCount
```

## Definition of Done

- [ ] `ITatamiRepository` implementation written in Infrastructure
- [ ] In-memory store functioning correctly
- [ ] All repository tests passing: `dotnet test tests/CompetitionManager.Tests/`
- [ ] No compiler warnings
- [ ] Architecture validator passes
- [ ] Code follows `AGENTS.md` naming (PascalCase, Repository suffix)
- [ ] **NOT committed or pushed**

---

**Next ticket (after code review & commit):** CMS-008 — Build TatamiService
