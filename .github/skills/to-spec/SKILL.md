---
name: to-spec
description: "Synthesize a grilling session into a formal spec/PRD. Create or update a spec document in the `tickets/` folder."
disable-model-invocation: true
---

Take the shared understanding from a `/grill-me` session and synthesize it into a formal **spec** or **PRD** (Product Requirements Document).

**Do not** re-interview; synthesize what we already discussed.

## Process

1. **Review the grilled decision.** Restate the domain changes, layer impacts, and open questions from the grilling session.
2. **Formalize the spec.** Structure it as a tracker **Epic** (in `tickets/` folder) with:
   - **Title:** Concise feature name (e.g., "Add Competition Management")
   - **Description:** User story format: "As a [role], I want [goal], so that [benefit]."
   - **Acceptance criteria:** Concrete, testable statements about what done looks like.
   - **Domain model:** New/modified entities, value objects, aggregates.
   - **Layer breakdown:** Which layers are touched, and why.
   - **Technical constraints:** Boundary rules, testing requirements, etc.
3. **Create a ticket file.** Save as `tickets/EPIC-NNN-FeatureName.md` (example: `tickets/EPIC-001-CompetitionCrud.md`).
4. **Link to architecture.** Reference `architecture.json` to show which layers are involved.
5. **Hand off.** Name the next stage: **"Once you approve the spec, run `/to-tickets` to break it into vertical-slice tickets."**

## Spec template

```markdown
# EPIC-NNN: <Feature Name>

**Status:** Draft

## User Story
As a [role], I want [goal], so that [benefit].

## Acceptance Criteria
- [ ] Domain model is defined (new entities, invariants, etc.)
- [ ] Application services handle the use case
- [ ] API endpoints expose the feature
- [ ] Tests cover happy path and edge cases
- [ ] Boundaries are respected (no forbidden dependencies)

## Domain Model Changes
- New entities: <list>
- New value objects: <list>
- Modified aggregates: <list>
- Business rules enforced: <list>

## Layer Impact
| Layer | Changes |
|-------|---------|
| **API** | <endpoints, DTOs> |
| **Application** | <services, use cases> |
| **Domain** | <entities, rules> |
| **Infrastructure** | <repositories, external services> |

## Technical Notes
- Test framework: MSTest
- Architecture constraints: [Reference architecture.json boundary rules]
- Dependencies: [External systems, NuGet packages]

## Open Questions
- <If any remain>
```
