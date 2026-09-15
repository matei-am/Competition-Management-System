# Workflow Guide — Competition Management System

A step-by-step guide to using the agentic workflow for building the Competition Management System.

## The pipeline

The workflow consists of six stages, each with a slash command:

```
/grill-me  →  /to-spec  →  /to-tickets  →  /implement  →  /code-review  →  (commit)
```

Each stage ends by handing control back to you. **There is no auto-advancement.** You decide when to move to the next stage.

## Stage 1: `/grill-me` (Sharpen)

**What:** Interview with the agent to lock down a feature idea.

**When to use:**
- Starting a new feature: *"I want to add user authentication."*
- Refining a vague requirement: *"How should we structure the scoring system?"*
- Stress-testing a design: *"Can we scale this to 10,000 competitors?"*

**How to use:**
1. Type `/grill-me` and describe the feature or decision.
2. Answer questions one at a time. The agent will ask you to walk down the decision tree.
3. Don't jump ahead; let it finish the interview.
4. When done, the agent summarizes the **shared understanding**.
5. Confirm you're aligned, then move to the next stage.

**Output:** Shared understanding documented (you can copy it into a ticket).

**Next stage:** Run `/to-spec` to formalize this into a spec.

---

## Stage 2: `/to-spec` (Spec)

**What:** Turn the grilled understanding into a formal spec (Epic).

**When to use:**
- After `/grill-me` session, to document the feature spec.
- To attach a spec to a tracker Epic.

**How to use:**
1. Type `/to-spec` (no arguments).
2. The agent synthesizes the grilled understanding into a spec.
3. The agent creates `tickets/EPIC-NNN-FeatureName.md` with:
   - User story
   - Acceptance criteria
   - Domain model changes
   - Layer impact
   - Technical notes
4. Review the spec. Iterate if needed.

**Output:** `tickets/EPIC-NNN.md` (tracker Epic).

**Next stage:** Run `/to-tickets` to break the spec into implementation tickets.

---

## Stage 3: `/to-tickets` (Slice)

**What:** Break the Epic into vertical-slice tickets.

**When to use:**
- After `/to-spec` is approved, to create implementation tickets.

**How to use:**
1. Type `/to-tickets`.
2. The agent reads the Epic and breaks it into tracer-bullet tickets.
3. Tickets are saved as `tickets/CMS-NNN-TicketName.md`.
4. Each ticket:
   - Is a vertical slice (touches multiple layers)
   - Is sized to one session
   - Has explicit "blocked by" dependencies
   - Has clear acceptance criteria
5. Review the ticket list. Iterate if any ticket is too large or dependencies are wrong.

**Output:** `tickets/CMS-NNN.md` files (one per ticket).

**Next stage:** Implement the first ticket. Run `/implement` in a fresh session.

---

## Stage 4: `/implement` (Build)

**What:** Implement one ticket end-to-end, test-first.

**When to use:**
- To implement a single ticket from `tickets/CMS-NNN.md`.
- Always start a fresh session per ticket (new context window).

**How to use:**
1. Type `/implement CMS-NNN` (ticket number).
2. The agent reads the ticket and implements it:
   - Writes tests first (Arrange-Act-Assert).
   - Writes code to make tests pass.
   - Validates architecture boundaries.
   - Runs `dotnet build` and `dotnet test`.
3. The `.NET/C# Specialist` will interleave teaching:
   - *"Here's why we use the Repository pattern..."*
   - *"This is idiomatic async/await..."*
   - *"Notice how Domain stays pure..."*
4. When done, the agent shows what was implemented and **does NOT commit**.

**Output:** Changed code (staged in git, not committed).

**Next stage:** Review with `/code-review` before committing.

---

## Stage 5: `/code-review` (Review)

**What:** Audit the implementation for house standards and spec conformance.

**When to use:**
- After `/implement` is done, before committing.

**How to use:**
1. Type `/code-review`.
2. The agent reviews the diff:
   - **House standards:** Does it follow `AGENTS.md`, `architecture.json`, naming, style?
   - **Spec conformance:** Does it satisfy the ticket's acceptance criteria?
3. The agent provides:
   - ✓ What's working well
   - ⚠ Suggestions for improvement
   - ❌ Blockers (must fix before commit)
4. Address any blockers, then move to commit.

**Output:** Review report with actionable feedback.

**Next stage:** Commit manually (you choose what to commit), then loop back to `/implement` for the next ticket, or start a new feature with `/grill-me`.

---

## Commit (Manual)

**What:** You commit the changes.

**When to do this:**
- After `/code-review` is clear (no blockers).
- For every ticket (so you see exactly what changed).

**How to do this:**
1. Review the diff: `git diff`
2. Stage the changes: `git add .`
3. Commit with a ticket reference: `git commit -m "[CMS-NNN] Description of change"`
4. Example: `git commit -m "[CMS-001] Define Competition aggregate with business rules"`

**Benefit:** You understand exactly what changed and why. This is critical for learning.

---

## Full loop example

Let's say you want to add "Create Competition" feature:

1. **Grill it:** `/grill-me` → Interview about requirements, domain model, acceptance criteria.
2. **Spec it:** `/to-spec` → Agent creates `EPIC-001-CompetitionCrud.md`.
3. **Slice it:** `/to-tickets` → Agent creates:
   - `CMS-001-DefineCompetitionAggregate.md`
   - `CMS-002-BuildCompetitionRepository.md`
   - `CMS-003-BuildCompetitionService.md`
   - `CMS-004-AddCompetitionEndpoints.md`
4. **Implement 1:** `/implement CMS-001` → Domain model done, tests passing.
5. **Review 1:** `/code-review` → Feedback, no blockers. ✓
6. **Commit 1:** `git commit -m "[CMS-001] Define Competition aggregate..."`
7. **Implement 2:** `/implement CMS-002` → Repository done. (New session; full context for this ticket.)
8. **Review 2:** `/code-review` → ✓
9. **Commit 2:** `git commit -m "[CMS-002] Add CompetitionRepository..."`
10. **... repeat for CMS-003, CMS-004...**
11. **All tickets done:** Epic is complete!

---

## Tips for success

### Start small
- First ticket should be Domain-only (1–2 hours).
- Next tickets layer up: Application, then Infrastructure, then API.
- This teaches the architecture naturally.

### Use `/teach` actively
- Ask the `.NET/C# Specialist` questions: *"Explain the Repository pattern."*
- Ask the **Testing Specialist** about MSTest: *"How do I write better domain tests?"*
- Learning is the goal; take time to understand, not just build.

### Commit frequently
- One ticket = one commit.
- Commit messages should reference the ticket: `[CMS-NNN]`.
- This creates a readable history of the feature.

### Validate architecture
- After every `/implement`, the agent runs `python3 scripts/validate_boundaries.py`.
- If it fails, something is wrong; fix it before committing.
- This is how you learn the boundaries, not by memorizing them.

### Iterate the Epic
- If partway through tickets you realize the Domain needs to change, stop and run `/grill-me` again.
- Update the Epic if needed.
- This is normal; architecture evolves.

---

## Troubleshooting

**Q: A ticket is too big for one session.**
A: Split it in a new `/to-tickets` session. No shame in that; it's learning.

**Q: The architecture validator is failing.**
A: It means a file is importing from a forbidden layer. Check `architecture.json` and fix the import. This is how the guardrails work.

**Q: Tests are failing but I'm not sure why.**
A: Run `/teach` and ask the **Testing Specialist** for help. MSTest learning is part of the project.

**Q: I don't understand a design decision.**
A: Run `/teach` and ask the `.NET/C# Specialist` to explain. That's exactly what they're there for.

**Q: Can I skip `/code-review`?**
A: No. Code review teaches you to spot your own patterns and mistakes. This is a learning project, not a production rush.

**Q: Can the agent auto-commit?**
A: No. You commit manually so you understand every change. This is intentional.

---

## Key files

- **`AGENTS.md`** — Canonical conventions (layer boundaries, naming, code style, testing)
- **`architecture.json`** — Machine-readable layer structure and boundary rules
- **`scripts/validate_boundaries.py`** — Validator that checks boundaries
- **`.github/skills/`** — Slash command skills (`grill-me`, `to-spec`, etc.)
- **`.github/agents/`** — Specialist sub-agents (conductor, .NET specialist, testing specialist)
- **`tickets/`** — Epic and ticket tracking (local markdown)
- **`docs/agents/issue-tracker.md`** — Ticket structure and labels (this folder)
- **`docs/agents/workflow-guide.md`** — This file

---

## Next: Start your first feature

1. Think of a small feature (e.g., "Add competition creation").
2. Type `/grill-me` and describe it.
3. Follow the pipeline.
4. After the first ticket is done, you'll understand the rhythm.

Good luck, and enjoy learning .NET and C#! 🚀
