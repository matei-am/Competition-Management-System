# CMS-003: Build CompetitionService (Application)

**Epic:** EPIC-001
**Status:** Ready to implement
**Blocked by:** CMS-002

## Objective

Implement the `CompetitionService` application service with CRUD use cases. This service orchestrates the Domain model and repository, handling the business logic coordination.

## Acceptance Criteria

- [ ] `CompetitionService` created in `src/CompetitionManager.Application/Services/CompetitionService.cs`
- [ ] Constructor injects ICompetitionRepository
- [ ] Method: `CreateCompetitionAsync(CreateCompetitionRequest request)` → returns created Competition
- [ ] Method: `GetCompetitionByIdAsync(Guid id)` → returns Competition or throws NotFoundException
- [ ] Method: `GetAllCompetitionsAsync()` → returns list of all competitions
- [ ] Method: `UpdateCompetitionAsync(Guid id, UpdateCompetitionRequest request)` → returns updated Competition with status-dependent constraints
- [ ] Method: `DeleteCompetitionAsync(Guid id)` → deletes competition (no return value or returns success flag)
- [ ] CreateCompetitionRequest DTO: Name, Date, Location, AthleteCount, ClubCount, RefereeCount, Rules, Status (optional, defaults to Upcoming)
- [ ] UpdateCompetitionRequest DTO: only updatable fields (Name, Rules, Counts based on status)
- [ ] Service enforces business rules via Domain (e.g., status transitions, field updates only at specific statuses)
- [ ] Validation errors throw `InvalidOperationException` with descriptive messages
- [ ] Comprehensive unit tests in `tests/CompetitionManager.Tests/Application/Services/CompetitionServiceTests.cs`
- [ ] Architecture validator passes

## Layer Breakdown

### Application
- **New folder:** `src/CompetitionManager.Application/Services/`
- **New file:** `src/CompetitionManager.Application/Services/CompetitionService.cs`
  - Public methods for CRUD operations
  - Orchestrates Domain entities and repository calls
  - Validation logic (delegates to Domain where possible)
- **New folder:** `src/CompetitionManager.Application/DTOs/`
- **New files:**
  - `src/CompetitionManager.Application/DTOs/CreateCompetitionRequest.cs`
  - `src/CompetitionManager.Application/DTOs/UpdateCompetitionRequest.cs`
  - `src/CompetitionManager.Application/DTOs/CompetitionResponse.cs` (for API returns)

### Dependency Injection (in Program.cs or DI setup)
- Register `CompetitionService` in the DI container

## Implementation Notes

- **Orchestration pattern:** Service calls repository and Domain. Domain does validation; repository does persistence.
- **Status-aware updates:** When updating Name or Rules, check the current status (via Domain methods like `CanUpdateName()`) and reject if not allowed.
- **DTO mapping:** Convert incoming DTOs to Domain objects. Keep this simple (manual mapping for now; AutoMapper can come later).
- **Error handling:** Use exceptions for validation errors. Application should not catch Domain exceptions; let them bubble up to the API layer.
- **Async/await:** Use `async/await` for repository calls. Example: `await _repository.GetByIdAsync(id)`

## Testing

**File:** `tests/CompetitionManager.Tests/Application/Services/CompetitionServiceTests.cs`

**Tests to write (AAA pattern):**

```
CompetitionServiceTests
├── CreateCompetition_WithValidRequest_ReturnsCreatedCompetition
├── CreateCompetition_WithInvalidAthleteCount_ThrowsException
├── CreateCompetition_WithDuplicateName_ThrowsException
├── GetCompetitionById_WithValidId_ReturnsCompetition
├── GetCompetitionById_WithInvalidId_ThrowsNotFoundException
├── GetAllCompetitions_WithMultiple_ReturnsAll
├── UpdateCompetition_ChangeName_InUpcomingStatus_Succeeds
├── UpdateCompetition_ChangeName_InActiveStatus_ThrowsException
├── UpdateCompetition_ChangeRules_InUpcomingStatus_Succeeds
├── UpdateCompetition_ChangeRules_InActiveStatus_Succeeds
├── UpdateCompetition_ChangeRules_InFinishedStatus_ThrowsException
├── UpdateCompetition_TransitionStatus_Succeeds
├── UpdateCompetition_BackwardStatusTransition_ThrowsException
├── DeleteCompetition_WithValidId_RemovesSuccessfully
└── DeleteCompetition_WithInvalidId_ThrowsException
```

## Definition of Done

- [ ] CompetitionService class written with all CRUD methods
- [ ] Request/Response DTOs created
- [ ] Service methods use async/await
- [ ] Validation enforced (delegates to Domain where possible)
- [ ] All service tests passing: `dotnet test tests/CompetitionManager.Tests/`
- [ ] No compiler warnings
- [ ] Architecture validator passes
- [ ] Code follows AGENTS.md naming and conventions
- [ ] **NOT committed or pushed**

---

**Next ticket (after code review & commit):** CMS-004 — Add Competition API Endpoints
