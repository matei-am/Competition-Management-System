# CMS-010: Integration Tests & Full Validation

**Epic:** EPIC-004
**Status:** Ready to implement
**Blocked by:** CMS-009

## Objective

Write end-to-end integration tests that verify the complete Tatami management flow from API through Application to Domain and back, and validate the feature against the Epic acceptance criteria.

## Acceptance Criteria

- [ ] Full end-to-end tests covering Add → List → Remove workflows
- [ ] Min-1 floor tests (cannot remove the last Tatami while Upcoming or Active)
- [ ] Renumbering tests (sequential renumber on remove while Upcoming; stable numbers on remove while Active)
- [ ] Auto-cancel test (transitioning to Active with 0 Tatamis results in `Cancelled`, not `Active`)
- [ ] Status-dependent constraint tests (cannot add/remove while `Finished` or `Cancelled`)
- [ ] Error scenarios tested (nonexistent competition, nonexistent Tatami, Tatami belonging to a different competition)
- [ ] All EPIC-004 acceptance criteria verified by tests
- [ ] All tests passing: `dotnet test tests/CompetitionManager.Tests/`
- [ ] Full build succeeds: `dotnet build`
- [ ] Architecture validator passes: `python3 scripts/validate_boundaries.py architecture.json --repo-root .`

## Layer Breakdown

### Testing
- **New file:** `tests/CompetitionManager.Tests/Integration/TatamiManagementIntegrationTests.cs`
  - Wire up all 4 layers (API, Application, Domain, Infrastructure)
  - Test real workflows (not mocked)
  - Verify business rules are enforced end-to-end

### Validation
- Run `dotnet build` (all layers compile)
- Run `dotnet test` (all tests pass)
- Run `python3 scripts/validate_boundaries.py` (no boundary violations)

## Implementation Notes

- **Integration tests:** Wire up the DI container, create in-memory repositories, call services, verify results.
- **Test scenarios:**
  - Happy path: Create competition (Upcoming) → Add Tatami → List → Remove → error (last one)
  - Add a second Tatami, then remove the first while Upcoming → remaining renumbers to 1
  - Activate with ≥1 Tatami → Competition becomes `Active`
  - Activate with 0 Tatamis → Competition becomes `Cancelled`
  - Add Tatami while Active → succeeds; remove while Active → stable numbering, no renumber
  - Add/remove while `Finished`/`Cancelled` → rejected
- **Fixture/Setup:** Reuse or extend the existing integration test helper for DI container setup.
- **AAA pattern:** Arrange (set up), Act (call service), Assert (verify).

## Testing

**File:** `tests/CompetitionManager.Tests/Integration/TatamiManagementIntegrationTests.cs`

**Test scenarios (AAA pattern):**

```
TatamiManagementIntegrationTests
├── AddListRemove_HappyPath_Succeeds
├── RemoveLastTatami_WhileUpcoming_Fails
├── RemoveLastTatami_WhileActive_Fails
├── RemoveTatami_WhileUpcoming_RenumbersRemainingSequentially
├── RemoveTatami_WhileActive_KeepsNumbersStable
├── ActivateCompetition_WithTatamis_BecomesActive
├── ActivateCompetition_WithZeroTatamis_BecomesCancelled
├── AddTatami_WhileFinished_Fails
├── AddTatami_WhileCancelled_Fails
├── RemoveTatami_WhileFinished_Fails
├── RemoveTatami_NotBelongingToCompetition_Fails
└── EndToEnd_CompleteLifecycle_Succeeds
```

## Definition of Done

- [ ] Integration tests written (at least 10+ test scenarios)
- [ ] All tests passing: `dotnet test tests/CompetitionManager.Tests/`
- [ ] Full build succeeds: `dotnet build`
- [ ] No compiler warnings
- [ ] Architecture validator passes: `python3 scripts/validate_boundaries.py architecture.json --repo-root .`
- [ ] All EPIC-004 acceptance criteria verified by tests
- [ ] Code follows `AGENTS.md` conventions
- [ ] **NOT committed or pushed**

---

## Completion

After this ticket is complete, approved, and committed:
- ✅ EPIC-004 (Tatami Management) is **DONE**
- ✅ All 5 tickets (CMS-006 through CMS-010) are complete
- ✅ Feature is ready for the next iteration

**What's next?**
- Run `/grill-me` again for the next capability: Scheduling / Fights per Tatami

---

**End of EPIC-004 tickets**
