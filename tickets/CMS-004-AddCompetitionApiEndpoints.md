# CMS-004: Add Competition API Endpoints

**Epic:** EPIC-001
**Status:** Ready to implement
**Blocked by:** CMS-003

## Objective

Create REST API endpoints for Competition CRUD operations. Thin controller that maps DTOs and delegates to Application services.

## Acceptance Criteria

- [ ] `CompetitionsController` created in `src/CompetitionManager.Api/Controllers/CompetitionsController.cs`
- [ ] Constructor injects `ICompetitionService`
- [ ] Endpoint: `GET /api/competitions` → returns all competitions (HTTP 200)
- [ ] Endpoint: `GET /api/competitions/{id}` → returns specific competition (HTTP 200) or 404
- [ ] Endpoint: `POST /api/competitions` → creates a new competition (HTTP 201 with Location header)
- [ ] Endpoint: `PUT /api/competitions/{id}` → updates competition (HTTP 200) or 404
- [ ] Endpoint: `DELETE /api/competitions/{id}` → deletes competition (HTTP 204) or 404
- [ ] Request/Response DTOs map correctly (no business logic in controllers)
- [ ] Error responses return appropriate HTTP status codes (400 for validation, 404 for not found, 500 for server errors)
- [ ] Integration tests in `tests/CompetitionManager.Tests/Api/Controllers/CompetitionsControllerTests.cs`
- [ ] Architecture validator passes

## Layer Breakdown

### API
- **New file:** `src/CompetitionManager.Api/Controllers/CompetitionsController.cs`
  - Route: `[Route("api/[controller]")]`
  - Methods: GetAll, GetById, Create, Update, Delete
  - Thin: no business logic; only validation of HTTP input and delegation to service
  - DTO mapping: convert incoming DTOs to service requests, service responses to HTTP response DTOs

### Data Transfer Objects (DTOs)
- **Already created in CMS-003:** `CreateCompetitionRequest`, `UpdateCompetitionRequest`, `CompetitionResponse`
- Ensure these are importable from Application layer

## Implementation Notes

- **Thin controllers:** Controller only validates HTTP shape (required fields), maps DTOs, calls service, and returns HTTP response. No business logic.
- **HTTP semantics:**
  - POST: return 201 Created with Location header (e.g., `Location: /api/competitions/{id}`)
  - GET: return 200 OK with body
  - PUT: return 200 OK with updated body (or 204 No Content)
  - DELETE: return 204 No Content (or 200 with deleted object)
  - 404 Not Found when resource doesn't exist
  - 400 Bad Request for validation errors
- **Error handling:** Catch service exceptions and translate to HTTP responses. For now, let application exceptions bubble up (middleware will convert to HTTP).
- **Attribute routing:** Use `[HttpGet("{id}")]`, `[HttpPost]`, `[HttpPut("{id}")]`, `[HttpDelete("{id}")]`

## Testing

**File:** `tests/CompetitionManager.Tests/Api/Controllers/CompetitionsControllerTests.cs`

**Tests to write (AAA pattern, using Arrange-Act-Assert):**

```
CompetitionsControllerTests
├── GetAllCompetitions_ReturnsOkWithList
├── GetCompetitionById_WithValidId_ReturnsOk
├── GetCompetitionById_WithInvalidId_ReturnsNotFound
├── CreateCompetition_WithValidRequest_ReturnsCreatedAtRoute
├── CreateCompetition_WithInvalidRequest_ReturnsBadRequest
├── UpdateCompetition_WithValidRequest_ReturnsOk
├── UpdateCompetition_WithInvalidId_ReturnsNotFound
├── UpdateCompetition_WithInvalidRequest_ReturnsBadRequest
├── DeleteCompetition_WithValidId_ReturnsNoContent
└── DeleteCompetition_WithInvalidId_ReturnsNotFound
```

**Note:** These are integration tests; they should mock or wire the actual service. See `testing-specialist` notes for MSTest patterns.

## Definition of Done

- [ ] CompetitionsController created with all CRUD endpoints
- [ ] HTTP semantics correct (status codes, headers)
- [ ] DTOs map correctly (no business logic in controller)
- [ ] All controller tests passing
- [ ] Manually test endpoints (e.g., via Postman or curl)
- [ ] No compiler warnings
- [ ] Architecture validator passes
- [ ] Code follows AGENTS.md naming
- [ ] **NOT committed or pushed**

---

**Next ticket (after code review & commit):** CMS-005 — Full Integration Tests (optional, or can be part of `/implement` review)
