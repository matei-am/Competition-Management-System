# CMS-002: Implement CompetitionRepository

**Epic:** EPIC-001
**Status:** Ready to implement
**Blocked by:** CMS-001

## Objective

Create the repository interface in Domain and implement an in-memory repository in Infrastructure. This allows Application services to persist/retrieve Competitions without business logic knowing about data storage.

## Acceptance Criteria

- [ ] `ICompetitionRepository` interface created in `src/CompetitionManager.Domain/Repositories/ICompetitionRepository.cs`
- [ ] Interface methods: GetById, GetAll, Add, Update, Delete, ExistsByName
- [ ] `CompetitionRepository` implementation created in `src/CompetitionManager.Infrastructure/Repositories/CompetitionRepository.cs`
- [ ] In-memory store (dictionary) to hold competitions (no database yet)
- [ ] GetById returns Competition or throws/returns null appropriately
- [ ] GetAll returns all stored competitions
- [ ] Add enforces name uniqueness (throw exception if duplicate)
- [ ] Update replaces an existing competition
- [ ] Delete removes a competition
- [ ] ExistsByName checks for name uniqueness
- [ ] Async methods where appropriate (Task return types for future database compatibility)
- [ ] Comprehensive tests in `tests/CompetitionManager.Tests/Infrastructure/Repositories/CompetitionRepositoryTests.cs`
- [ ] Architecture validator passes

## Layer Breakdown

### Domain
- **New file:** `src/CompetitionManager.Domain/Repositories/ICompetitionRepository.cs`
  - Interface with methods: `Task<Competition> GetByIdAsync(Guid id)`, `Task<IEnumerable<Competition>> GetAllAsync()`, `Task AddAsync(Competition competition)`, `Task UpdateAsync(Competition competition)`, `Task DeleteAsync(Guid id)`, `Task<bool> ExistsByNameAsync(string name)`
  - No implementation; Domain only defines the contract

### Infrastructure
- **New file:** `src/CompetitionManager.Infrastructure/Repositories/CompetitionRepository.cs`
  - Implements ICompetitionRepository
  - In-memory `Dictionary<Guid, Competition>` as backing store
  - Name uniqueness check in Add/Update methods
  - Throws exceptions (e.g., `InvalidOperationException`) for duplicate names or not-found scenarios

## Implementation Notes

- **Repository pattern:** Separates persistence from business logic. Domain defines what it needs; Infrastructure provides the implementation.
- **In-memory storage:** For this ticket, use a simple Dictionary. Real database (EF Core, SQL) comes later.
- **Async/await:** Use async methods (Task/Task<T>) so the API can scale. For in-memory, can just wrap results in Task.CompletedTask or Task.FromResult().
- **Name uniqueness:** Check on Add/Update. If duplicate, throw `InvalidOperationException("Competition with name '{name}' already exists.")`.
- **Not found handling:** GetById can throw `KeyNotFoundException` or return null—decide on one pattern and be consistent.

## Testing

**File:** `tests/CompetitionManager.Tests/Infrastructure/Repositories/CompetitionRepositoryTests.cs`

**Tests to write (AAA pattern):**

```
CompetitionRepositoryTests
├── AddCompetition_WithValidInput_StoresSuccessfully
├── GetById_WithValidId_ReturnsCompetition
├── GetById_WithInvalidId_ThrowsKeyNotFoundException
├── GetAll_WithNoCompetitions_ReturnsEmptyList
├── GetAll_WithMultipleCompetitions_ReturnsAll
├── Update_WithValidCompetition_UpdatesSuccessfully
├── Update_WithNonExistentId_ThrowsException
├── Delete_WithValidId_RemovesCompetition
├── Delete_WithInvalidId_ThrowsException
├── ExistsByName_WithExistingName_ReturnsTrue
├── ExistsByName_WithNonExistentName_ReturnsFalse
├── Add_WithDuplicateName_ThrowsInvalidOperationException
└── Update_WithDuplicateName_ThrowsInvalidOperationException
```

## Definition of Done

- [ ] ICompetitionRepository interface written in Domain
- [ ] CompetitionRepository implementation written in Infrastructure
- [ ] In-memory store functioning correctly
- [ ] Name uniqueness enforced with exceptions
- [ ] All repository tests passing: `dotnet test tests/CompetitionManager.Tests/`
- [ ] No compiler warnings
- [ ] Architecture validator passes
- [ ] Code follows `AGENTS.md` naming (PascalCase, Service/Repository suffixes)
- [ ] **NOT committed or pushed**

---

**Next ticket (after code review & commit):** CMS-003 — Build CompetitionService
