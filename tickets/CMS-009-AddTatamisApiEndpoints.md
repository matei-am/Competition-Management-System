# CMS-009: Add Tatamis API Endpoints

**Epic:** EPIC-004
**Status:** Ready to implement
**Blocked by:** CMS-008

## Objective

Create a thin REST API vertical slice for Tatami management, nested under Competitions. The controller maps HTTP requests to `ITatamiService` and maps service outcomes to HTTP responses.

## Acceptance Criteria

- [ ] `TatamisController` created in `src/CompetitionManager.Api/Controllers/TatamisController.cs` with route `api/competitions/{competitionId}/tatamis`
- [ ] The controller depends on `ITatamiService` only (no repository references)
- [ ] `GET /api/competitions/{competitionId}/tatamis` returns all Tatamis for the competition with HTTP 200, ordered by `Number`
- [ ] `POST /api/competitions/{competitionId}/tatamis` adds a Tatami and returns HTTP 201 with the created response and a `Location` header
- [ ] `DELETE /api/competitions/{competitionId}/tatamis/{tatamiId}` returns HTTP 204, or HTTP 404 for `NotFoundException`
- [ ] `NotFoundException` from the Application service becomes HTTP 404
- [ ] `ArgumentException` and `InvalidOperationException` (e.g. min-1 floor, wrong status) become HTTP 400
- [ ] Unexpected exceptions reach host-level HTTP 500 handling
- [ ] Controller tests pass and the architecture validator reports no forbidden dependencies

## Layer Breakdown

### Domain
- No code changes.

### Infrastructure
- No code changes. The API must not reference `TatamiRepository` or `CompetitionRepository` directly.

### Application
- No new services; reuses `ITatamiService` and `TatamiResponse` from CMS-008.

### API
- Add `src/CompetitionManager.Api/Controllers/TatamisController.cs`.
- Register `ITatamiService` resolution in `Program.cs` (already wired in CMS-008; confirm controller resolves correctly).
- Implement `GetTatamisForCompetition`, `AddTatami`, `RemoveTatami`.

## Implementation Notes

- **Thin controller:** Validates HTTP shape, maps DTOs, calls `ITatamiService`, returns HTTP responses. No business logic.
- **HTTP semantics:**
  - GET: 200 OK with the list.
  - POST: `CreatedAtAction(nameof(GetTatamisForCompetition), new { competitionId }, response)`, HTTP 201.
  - DELETE: 204 No Content.
  - 404 for `NotFoundException`.
  - 400 for `ArgumentException` and `InvalidOperationException`.
- **Route nesting:** Use `[Route("api/competitions/{competitionId}/tatamis")]` to keep the Tatami sub-resource explicit under its owning Competition.
- **Request body:** `POST` needs no request body (Number is server-assigned) — an empty `AddTatamiRequest` or no body at all is acceptable; decide during implementation and keep it consistent with `AGENTS.md`.

## Testing

**File:** `tests/CompetitionManager.Tests/Api/Controllers/TatamisControllerTests.cs`

Use a fake or mock `ITatamiService`; these are focused controller tests, not full host integration tests.

**Tests to write (AAA pattern):**

```
TatamisControllerTests
├── GetTatamisForCompetition_ReturnsOkWithList
├── GetTatamisForCompetition_WithInvalidCompetitionId_ReturnsNotFound
├── AddTatami_WithValidCompetition_ReturnsCreatedWithLocation
├── AddTatami_WhenServiceRejectsRequest_ReturnsBadRequest
├── AddTatami_WithInvalidCompetitionId_ReturnsNotFound
├── RemoveTatami_WithValidIds_ReturnsNoContent
├── RemoveTatami_WithInvalidTatamiId_ReturnsNotFound
└── RemoveTatami_WhenServiceRejectsRequest_ReturnsBadRequest
```

## Definition of Done

- [ ] `TatamisController` created with list/add/remove endpoints
- [ ] HTTP semantics and `Location` header are correct
- [ ] DTOs are delegated correctly with no business logic in the controller
- [ ] All controller tests pass
- [ ] `dotnet build` completes without compiler warnings
- [ ] `python3 scripts/validate_boundaries.py architecture.json --repo-root .` passes
- [ ] Code follows `AGENTS.md` naming and layer boundaries
- [ ] **NOT committed or pushed**

---

**Next ticket (after code review & commit):** CMS-010 — Full Integration Tests and Validation
