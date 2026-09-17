# CMS-003: Build CompetitionService Vertical Slice

**Epic:** EPIC-002
**Status:** In Progress
**Blocked by:** CMS-002

## Objective

Implement the competition CRUD application use cases as one vertical slice: request DTOs, `CompetitionService`, dependency injection wiring, and service tests using the real in-memory repository.

## Acceptance Criteria

- [ ] `CompetitionService` is created in `src/CompetitionManager.Application/Services/CompetitionService.cs` and depends only on `ICompetitionRepository`.
- [ ] `CreateCompetitionAsync` persists valid requests and returns a competition whose status is `Upcoming`.
- [ ] Create requests cannot choose an arbitrary initial status.
- [ ] `GetCompetitionByIdAsync`, `GetAllCompetitionsAsync`, and `DeleteCompetitionAsync` delegate to the repository asynchronously.
- [ ] Missing competitions are exposed as an application-level `NotFoundException`.
- [ ] `UpdateCompetitionAsync` supports name, rules, status, and independent athlete, club, and referee count changes.
- [ ] Omitted update fields retain their current values; an empty update request is rejected.
- [ ] Domain methods enforce status-dependent updates, valid values, count minimums, and one-step forward status transitions.
- [ ] Duplicate names and invalid requests produce descriptive exceptions.
- [ ] `CompetitionService` and `CompetitionRepository` are registered in `src/CompetitionManager.Api/Program.cs` without adding API business logic.
- [ ] Service tests use the real in-memory repository and cover success and failure paths.
- [ ] Architecture boundaries pass validation.

## Layer Breakdown

### Domain
- Use the existing `Competition` aggregate, `CompetitionStatus`, and `ICompetitionRepository` contract.
- Do not add dependencies from Domain to Application or Infrastructure.

### Infrastructure
- Use the existing `CompetitionRepository` implementation through `ICompetitionRepository`.
- Do not reference Infrastructure types from `CompetitionService`.

### Application
- Add `src/CompetitionManager.Application/Services/CompetitionService.cs`.
- Add `src/CompetitionManager.Application/DTOs/CreateCompetitionRequest.cs`.
- Add `src/CompetitionManager.Application/DTOs/UpdateCompetitionRequest.cs`.
- Add `src/CompetitionManager.Application/DTOs/CompetitionResponse.cs`.
- Add an application-level `NotFoundException`.

### API
- Add project references needed for DI wiring.
- Register `ICompetitionRepository` to `CompetitionRepository` and `CompetitionService` in `Program.cs`.
- Do not add controllers or endpoint behavior; those belong to CMS-004.

## Implementation Notes

- **Orchestration:** Service calls the Domain aggregate and repository; Domain remains the source of business rules.
- **Creation:** The `Competition` constructor establishes `Upcoming`; do not expose caller-controlled initial status.
- **Partial counts:** Merge each nullable count in the update request with the current value before calling `UpdateCounts`.
- **Status-aware updates:** Use Domain methods such as `UpdateName`, `UpdateRules`, `UpdateCounts`, and `TransitionStatus`.
- **Empty updates:** Reject a request with no supplied changes using descriptive `InvalidOperationException`.
- **DTO mapping:** Use manual mapping. No AutoMapper dependency is needed.
- **Not found:** Translate repository `KeyNotFoundException` into `NotFoundException`; allow Domain validation exceptions to propagate.
- **Async/await:** Preserve asynchronous repository calls with `async`/`await`.
- **Architecture:** Follow `architecture.json` and `AGENTS.md`; Application may depend on Domain only.

## Testing

**File:** `tests/CompetitionManager.Tests/Application/Services/CompetitionServiceTests.cs`

Add a reference to `CompetitionManager.Application` in the test project and use the real `CompetitionRepository` for service tests.

**Tests to write (AAA pattern):**

```
CompetitionServiceTests
├── CreateCompetition_WithValidRequest_ReturnsUpcomingCompetition
├── CreateCompetition_WithInvalidCount_ThrowsException
├── CreateCompetition_WithDuplicateName_ThrowsException
├── GetCompetitionById_WithValidId_ReturnsCompetition
├── GetCompetitionById_WithInvalidId_ThrowsNotFoundException
├── GetAllCompetitions_WithMultiple_ReturnsAll
├── UpdateCompetition_ChangeName_InUpcomingStatus_Succeeds
├── UpdateCompetition_ChangeName_InActiveStatus_ThrowsException
├── UpdateCompetition_ChangeRules_InActiveStatus_Succeeds
├── UpdateCompetition_ChangeRules_InFinishedStatus_ThrowsException
├── UpdateCompetition_ChangeEachCountIndependently_Succeeds
├── UpdateCompetition_WithBlankNameOrRules_ThrowsException
├── UpdateCompetition_WithNoChanges_ThrowsException
├── UpdateCompetition_TransitionStatusForward_Succeeds
├── UpdateCompetition_TransitionStatusBackward_ThrowsException
├── DeleteCompetition_WithValidId_RemovesSuccessfully
└── DeleteCompetition_WithInvalidId_ThrowsNotFoundException
```

## Definition of Done

- [ ] Application project references Domain and contains no Infrastructure dependency.
- [ ] Request/response DTOs, service, and application exception are implemented.
- [ ] API DI wiring resolves the service and repository without controllers.
- [ ] Service tests pass: `dotnet test tests/CompetitionManager.Tests/`.
- [ ] Full build succeeds without compiler warnings.
- [ ] Architecture validator passes: `python3 scripts/validate_boundaries.py architecture.json --repo-root .`.
- [ ] Code follows `AGENTS.md` naming and conventions.
- [ ] **NOT committed or pushed**.

---

**Next ticket (after code review & commit):** CMS-004 — Add Competition API Endpoints
