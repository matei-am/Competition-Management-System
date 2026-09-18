# EPIC-004: Tatami Management

**Status:** Draft
**Created:** 2026-09-18

## User Story

As a competition organizer, I want to add and remove Tatamis (fight areas) for a Competition, so that the competition has the physical fight areas it needs before it can go Active, and can view what happens on each Tatami during upcoming and active status.

## Acceptance Criteria

- [ ] A Tatami can be added to a Competition while it is `Upcoming` or `Active`
- [ ] A Tatami can be removed from a Competition while it is `Upcoming` or `Active`
- [ ] A Competition can transiently have 0 Tatamis (before the first is added); the organizer must add the first Tatami explicitly (no auto-created "Tatami 1")
- [ ] Once a Competition has ≥ 1 Tatami, it cannot be reduced below 1 by removal while `Upcoming` or `Active`
- [ ] No minimum-Tatami floor applies once the Competition is `Finished`
- [ ] Removing a Tatami while `Upcoming` renumbers the remaining Tatamis sequentially (no gaps)
- [ ] Removing a Tatami while `Active` keeps existing numbers stable (gaps allowed)
- [ ] Removing a Tatami with scheduled/in-progress fights auto-unschedules those fights (rescheduling itself is out of scope for this epic)
- [ ] Transitioning a Competition from `Upcoming` to `Active` with 0 Tatamis auto-transitions it to `Cancelled` instead
- [ ] `CompetitionStatus` gains a `Cancelled` terminal value
- [ ] Tatamis for a Competition can be listed via the API
- [ ] All layers are covered: Domain (entity + repository interface), Application (service), Infrastructure (repository), API (endpoints)
- [ ] Boundaries are respected (no forbidden dependencies)

## Domain Model Changes

### New Entities

**Tatami** (new aggregate root, own repository — not nested inside `Competition`)
- `Id`: unique system-generated identifier (GUID)
- `CompetitionId`: GUID, required, references the owning Competition
- `Number`: int, required, display label (e.g. "Tatami 1"); renumbering behavior depends on Competition status at time of removal (see rules below)
- `CreatedAt` / `UpdatedAt`: DateTime (system-generated)

### Modified Entities

**Competition**
- `Status` gains a new enum value: `Cancelled` (terminal, no further transitions out of it)
- `TransitionStatus` rule updated: moving `Upcoming → Active` requires the caller (Application layer) to supply/verify the current Tatami count for that competition; if 0, the Competition transitions to `Cancelled` instead of `Active`. Domain remains pure — it does not query Tatami itself (cross-aggregate), so the Application layer resolves the Tatami count and passes the outcome (or the count) into the domain method.

### Business Rules (Invariants)

1. A Tatami always belongs to exactly one Competition (`CompetitionId` is immutable after creation).
2. Adding a Tatami is only allowed while the Competition is `Upcoming` or `Active` (not `Finished` or `Cancelled`).
3. Removing a Tatami is only allowed while the Competition is `Upcoming` or `Active`.
4. Removal that would bring the count below 1 is rejected while `Upcoming` or `Active`.
5. Removal while `Finished` has no minimum-count restriction (not reachable per rule 3, but the rule exists conceptually for completeness/future flexibility).
6. Removing a Tatami during `Upcoming` triggers sequential renumbering of the remaining Tatamis (1..N, no gaps).
7. Removing a Tatami during `Active` does **not** renumber — remaining Tatami numbers are stable identifiers, since active schedules may reference them.
8. Removing a Tatami with existing scheduled/in-progress fights auto-unschedules those fights (actual fight/schedule entities are out of scope for this epic; this rule is a placeholder invariant to be revisited once the Scheduling epic exists).
9. `Competition.TransitionStatus(Active)` results in `Cancelled` instead of `Active` if the Tatami count for that competition is 0 at the time of transition.
10. `Cancelled` is a terminal status; no further transitions are permitted out of it.
11. Manual, organizer-triggered cancellation (a `Cancel()` action independent of the Tatami count) is explicitly **out of scope** for this epic — deferred to a future "Competition Lifecycle" epic.
12. Authentication/authorization (organizer-only restriction on managing Tatamis) is explicitly **out of scope** for this epic — deferred to a future Auth epic. All endpoints are unauthenticated for now.

## Layer Impact

| Layer | Changes |
|-------|---------|
| **Domain** | New `Tatami` aggregate root in `src/CompetitionManager.Domain/Entities/`. New `ITatamiRepository` interface in `src/CompetitionManager.Domain/Repositories/`. Add `Cancelled` to `CompetitionStatus` enum. Update `Competition.TransitionStatus` to accept the info needed to decide between `Active`/`Cancelled` (exact signature TBD in `/implement`, keeping Domain pure — no direct Tatami repository access from Domain). |
| **Application** | New `TatamiService` (`ITatamiService`) in `src/CompetitionManager.Application/Services/`. Use cases: `AddTatami`, `RemoveTatami`, `GetTatamisForCompetition`. Orchestrates: status checks, min-1 floor, renumbering-vs-stable-numbering branch, and the cross-aggregate Tatami-count lookup used when transitioning a Competition to `Active`. |
| **Infrastructure** | New `TatamiRepository` implementing `ITatamiRepository` in `src/CompetitionManager.Infrastructure/Repositories/`. |
| **API** | New `TatamisController` in `src/CompetitionManager.Api/Controllers/`. Endpoints: `GET /api/competitions/{competitionId}/tatamis`, `POST /api/competitions/{competitionId}/tatamis`, `DELETE /api/competitions/{competitionId}/tatamis/{tatamiId}`. New DTOs: `TatamiResponse`, `AddTatamiRequest` (likely empty body — number is server-assigned). |

## Technical Notes

- **Testing:** Test-first, MSTest. Cover: add/remove happy paths, min-1 floor rejection, renumbering (Upcoming) vs. stable numbering (Active), auto-cancel-on-zero-Tatamis transition, terminal `Cancelled` status rejecting further transitions.
- **Architecture:** Respect 4-layer boundaries. `Tatami` is a separate aggregate root from `Competition` — cross-aggregate consistency (min-1, auto-cancel) is enforced in the Application layer, not by Domain reaching across aggregates.
- **Persistence:** Follow the same repository pattern established for `Competition` in EPIC-001 (decide in-memory vs. EF Core during `/implement`, consistent with existing `CompetitionRepository`).
- **Not in scope:** Fight/schedule entities, live scoring, brackets, breaks, rankings, auth — all deferred to their own future epics (see Open Questions).

## Open Questions

- Exact method signature for `Competition.TransitionStatus` when it needs Tatami-count context to decide `Active` vs `Cancelled` — resolve during `/to-tickets` or `/implement` (e.g., an overload taking a `tatamiCount` parameter, or an explicit `TryActivate(int tatamiCount)` method).
- Whether Tatami removal is addressed by `Guid` id or by `Number` in the API route — recommend `Guid` id (`Number` is display-only).

## Next Epics (not part of this spec, for roadmap visibility)

1. **Scheduling / Fights per Tatami** — athletes, category specifications, fight scheduling, timeline redirects/moves between Tatamis.
2. **Live Scoring** — live score during Active status.
3. **Brackets & Results** — bracket visualization and per-fight results during Finished status.
4. **Breaks** — athlete/referee breaks during a Competition.
5. **Rankings** — per-category fighter ranking based on bracket finish place.
6. **Competition Lifecycle** — manual `Cancel()` action.
7. **Auth** — authentication/authorization, organizer role, athlete/coach/club/referee sign-up.
