# EPIC-003: Competition API Endpoints

**Status:** Ready
**Created:** 2026-09-17

## User Story

As a competition organizer, I want HTTP endpoints for competition CRUD operations, so that clients can manage competitions through a stable REST API.

## Acceptance Criteria

- [ ] The API exposes `GET /api/competitions` and returns all competitions with HTTP 200.
- [ ] The API exposes `GET /api/competitions/{id}` and returns a competition with HTTP 200 when it exists.
- [ ] `GET /api/competitions/{id}` returns HTTP 404 when the competition does not exist.
- [ ] The API exposes `POST /api/competitions` and returns HTTP 201 with the created competition in the response body.
- [ ] A successful `POST` includes a `Location` header generated from the get-by-ID action.
- [ ] The API exposes `PUT /api/competitions/{id}` and returns HTTP 200 with the updated competition.
- [ ] `PUT` preserves the existing partial-update contract: omitted fields remain unchanged.
- [ ] `PUT` returns HTTP 404 when the competition does not exist.
- [ ] The API exposes `DELETE /api/competitions/{id}` and returns HTTP 204 when deletion succeeds.
- [ ] `DELETE` returns HTTP 404 when the competition does not exist.
- [ ] Application validation failures are returned as HTTP 400 responses.
- [ ] Unexpected exceptions are not misclassified by the controller and reach the host's HTTP 500 handling.
- [ ] The controller depends on `ICompetitionService`, not an Infrastructure implementation.
- [ ] Controller tests cover successful operations, validation failures, and missing competitions.
- [ ] The architecture validator passes with no forbidden dependencies.

## Domain Model Changes

- **New entities:** None.
- **New value objects:** None.
- **Modified aggregates:** None. The existing `Competition` aggregate remains the source of business rules.
- **Business rules enforced:** No new rules. Existing Application and Domain validation continues to govern create and update operations.

## Layer Impact

| Layer | Changes |
|-------|---------|
| **API** | Add `CompetitionsController` with CRUD routes, DTO delegation, HTTP status mapping, and `CreatedAtAction` for creation. Register controllers and expose the service through dependency injection. |
| **Application** | Add `ICompetitionService`; have the existing `CompetitionService` implement it without changing its use-case behavior. Existing request and response DTOs are reused. |
| **Domain** | No changes. The controller uses the existing Domain types only through Application contracts. |
| **Infrastructure** | No changes. The API must not depend directly on repository implementations. |

## Technical Notes

- **Architecture:** Follow `architecture.json`: API may depend on Application and Domain; Application may depend on Domain; Infrastructure depends on Domain; Domain remains independent of upper layers.
- **Controller boundary:** Keep the controller thin. It validates HTTP input shape, delegates to `ICompetitionService`, maps service outcomes to HTTP responses, and contains no business rules.
- **Error handling:** Map `NotFoundException` to 404 and `ArgumentException` or `InvalidOperationException` to 400. Let unexpected exceptions reach the host-level 500 handling.
- **Update semantics:** The existing `UpdateCompetitionRequest` is partial. Keep the `PUT` route specified by the ticket while preserving that Application-layer behavior.
- **Creation response:** Use `CreatedAtAction(nameof(GetCompetitionById), new { id = response.Id }, response)` so the framework generates the resource URL.
- **Dependency injection:** Register `ICompetitionService` to resolve `CompetitionService`; retain the repository registration required by the concrete service.
- **Testing:** The repository currently uses xUnit, so add focused controller-level xUnit tests with a fake or mock `ICompetitionService`. Full host-level integration tests remain suitable for CMS-005.
- **Dependencies:** No new external packages are required unless the chosen test double requires one. Reference `architecture.json` for boundary rules.

## Linked Tickets

- **EPIC-001:** Competition CRUD
- **EPIC-002:** Competition Application Service
- **CMS-003:** Build CompetitionService use cases
- **CMS-004:** Add Competition API endpoints
- **CMS-005:** Integration tests and validation

## Open Questions

None. The endpoint semantics, dependency abstraction, error mapping, update behavior, and test scope were confirmed during `/grill-me`.

---

**Next step:** Tickets are ready. Run `/implement` in a fresh session for CMS-004 after CMS-003 is complete.
