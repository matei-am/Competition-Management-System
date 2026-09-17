# EPIC-002: Competition Application Service

**Status:** Ready
**Created:** 2026-09-17

## User Story

As a competition organizer, I want an application service to manage competition CRUD use cases, so that the application can coordinate persistence and domain rules without exposing storage details to callers.

## Acceptance Criteria

- [ ] `CompetitionService` is implemented in the Application layer and depends on `ICompetitionRepository`.
- [ ] A valid create request returns a persisted `Competition` with status `Upcoming`.
- [ ] Create requests cannot choose an arbitrary initial status.
- [ ] Invalid competition values are rejected through Domain validation and surfaced to the caller.
- [ ] Duplicate competition names are rejected by the repository and surfaced by the service.
- [ ] A competition can be retrieved by ID.
- [ ] Missing competitions are exposed as an application-level `NotFoundException`.
- [ ] All competitions can be retrieved.
- [ ] An update request can change name, rules, status, and each participant count independently when the Domain permits it.
- [ ] Unspecified update fields retain their existing values.
- [ ] Name and rules updates reject null or blank values through Domain validation.
- [ ] Count updates preserve unspecified counts and validate the resulting complete count set through the Domain.
- [ ] Status changes only permit one-step forward transitions: `Upcoming` to `Active`, or `Active` to `Finished`.
- [ ] An empty update request is rejected with a descriptive `InvalidOperationException`.
- [ ] A competition can be deleted by ID.
- [ ] Service unit tests cover successful operations, validation failures, duplicate names, missing IDs, status-dependent updates, partial count updates, and invalid transitions.
- [ ] The architecture validator passes and no forbidden layer dependencies are introduced.

## Domain Model Changes

- **New entities:** None.
- **New value objects:** None.
- **Modified aggregates:** None. The existing `Competition` aggregate remains the source of business rules.
- **Business rules enforced:**
  - New competitions always start in `Upcoming` status.
  - Status transitions move forward by exactly one state.
  - Names may be changed only while `Upcoming`.
  - Rules may be changed while `Upcoming` or `Active`, but not `Finished`.
  - Athlete, club, and referee counts may be changed independently while preserving the other existing counts.
  - All count values must satisfy the existing Domain minimum.
  - Names and rules must remain valid non-blank values.
  - Empty update requests are invalid.
  - Competition names remain unique across stored competitions.

## Layer Impact

| Layer | Changes |
|-------|---------|
| **API** | Register `CompetitionService` and its repository dependency in the DI container. API endpoints are outside this epic. |
| **Application** | Add `CompetitionService`, create/update request DTOs, a response DTO, and application-level `NotFoundException`. Orchestrate repository calls and invoke Domain methods for validation and state changes. |
| **Domain** | No code changes. Use the existing `Competition` aggregate, `CompetitionStatus`, and `ICompetitionRepository` contract. |
| **Infrastructure** | No code changes. Use the existing in-memory `CompetitionRepository` through the Domain interface. |

## Technical Notes

- **Architecture:** Follow `architecture.json`: API depends on Application and Domain; Application depends on Domain; Infrastructure depends on Domain; Domain remains independent of all upper layers.
- **Dependency direction:** `CompetitionService` must depend on `ICompetitionRepository`, never on `CompetitionRepository` or another Infrastructure implementation.
- **DTOs:** Create requests contain competition creation fields. Initial status is not caller-controlled and defaults to `Upcoming`. Update fields are nullable/optional so omitted values remain unchanged.
- **Partial counts:** If one count is supplied, merge it with the competition's current athlete, club, and referee counts before calling the Domain's validated count update operation.
- **Error handling:** Translate repository `KeyNotFoundException` into Application `NotFoundException`; allow Domain validation exceptions to propagate. Use descriptive `InvalidOperationException` messages for invalid application requests.
- **Async:** Preserve asynchronous repository APIs with `async`/`await`.
- **Testing:** Follow AAA structure and test real service behavior using the in-memory repository. The current test project uses xUnit packages, despite the general `AGENTS.md` MSTest guideline; follow the existing project configuration unless it is deliberately changed.
- **Validation:** Run `dotnet test tests/CompetitionManager.Tests/` and `python3 scripts/validate_boundaries.py architecture.json --repo-root .`.
- **Dependencies:** No new external packages are required.

## Open Questions

None. The domain and layer decisions were confirmed during `/grill-me`.

## Linked Work

- **EPIC-001:** Competition CRUD
- **CMS-002:** Implement CompetitionRepository
- **CMS-003:** Build CompetitionService

---

**Next step:** Once you approve the spec, run `/to-tickets` to break it into vertical-slice tickets.
