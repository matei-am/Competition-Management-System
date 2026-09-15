# CMS-005: Integration Tests & Full Validation

**Epic:** EPIC-001
**Status:** Ready to implement
**Blocked by:** CMS-004

## Objective

Write end-to-end integration tests that verify the complete flow from API through Application to Domain and back. Validate the entire feature against the Epic acceptance criteria.

## Acceptance Criteria

- [ ] Full end-to-end tests covering Create → Read → Update → Delete workflows
- [ ] Status transition tests (Upcoming → Active → Finished)
- [ ] Status-dependent constraint tests (name updates only in Upcoming, etc.)
- [ ] Name uniqueness tests
- [ ] Count validation tests (min 4, max unlimited)
- [ ] Category handling tests
- [ ] Error scenarios tested (invalid status transitions, duplicate names, invalid counts, etc.)
- [ ] All Epic acceptance criteria verified by tests
- [ ] Test coverage >80% for Domain and Application layers
- [ ] All tests passing: `dotnet test tests/CompetitionManager.Tests/`
- [ ] Full build succeeds: `dotnet build`
- [ ] Architecture validator passes: `python3 scripts/validate_boundaries.py architecture.json --repo-root .`

## Layer Breakdown

### Testing
- **New file:** `tests/CompetitionManager.Tests/Integration/CompetitionCrudIntegrationTests.cs`
  - Wire up all 4 layers (API, Application, Domain, Infrastructure)
  - Test real workflows (not mocked)
  - Verify business rules are enforced end-to-end

### Validation
- Run `dotnet build` (all layers compile)
- Run `dotnet test` (all tests pass, coverage)
- Run `python3 scripts/validate_boundaries.py` (no boundary violations)

## Implementation Notes

- **Integration tests:** Wire up the DI container, create in-memory repository, call service, verify results
- **Test scenarios:**
  - Happy path: Create competition (Upcoming) → Activate → Finish
  - Name uniqueness: Try to create two with same name → exception
  - Count validation: Create with 3 athletes → exception
  - Status-dependent updates: Try to change name when Active → exception
  - Deletions: Delete works in any status
- **Fixture/Setup:** Create a test helper to set up the DI container and initialize repositories
- **AAA pattern:** Arrange (set up), Act (call service), Assert (verify)

## Testing

**File:** `tests/CompetitionManager.Tests/Integration/CompetitionCrudIntegrationTests.cs`

**Test scenarios (AAA pattern):**

```
CompetitionCrudIntegrationTests
├── CreateAndRetrieve_HappyPath_Succeeds
├── CreateUpdateDelete_Workflow_Succeeds
├── ActivateAndFinish_StatusTransitions_Succeed
├── UpdateNameInUpcoming_Succeeds_UpdateNameInActive_Fails
├── UpdateRulesInUpcomingAndActive_SucceedsInFinished_Fails
├── CreateWithDuplicateName_Fails
├── CreateWithInvalidCounts_Fails
├── TransitionStatusBackward_Fails
├── ListAllCompetitions_Returns_All
├── DeleteInAnyStatus_Succeeds
└── EndToEnd_CompleteLifecycle_Succeeds
```

## Definition of Done

- [ ] Integration tests written (at least 10+ test scenarios)
- [ ] All tests passing: `dotnet test tests/CompetitionManager.Tests/`
- [ ] Test coverage report shows >80% Domain/Application coverage
- [ ] Full build succeeds: `dotnet build`
- [ ] No compiler warnings
- [ ] Architecture validator passes: `python3 scripts/validate_boundaries.py architecture.json --repo-root .`
- [ ] All Epic acceptance criteria verified by tests
- [ ] Code follows AGENTS.md conventions
- [ ] **NOT committed or pushed**

---

## Completion

After this ticket is complete, approved, and committed:
- ✅ EPIC-001 (Competition CRUD) is **DONE**
- ✅ All 5 tickets (CMS-001 through CMS-005) are complete
- ✅ Feature is ready for production or next feature iteration

**What's next?**
- Define a new feature (run `/grill-me` again)
- Or refactor based on learnings
- Or add registrations (Participant management)

---

**End of EPIC-001 tickets**
