---
name: implement
description: "Implement one ticket end-to-end: test-first, cross-layer, with architecture validation. Do NOT auto-commit or push."
disable-model-invocation: true
---

Implement **one ticket** from `tickets/CMS-NNN.md`. This is a fresh session; the ticket contains everything you need.

**Critical constraint:** Write code, run tests, validate boundaries—but **do NOT commit or push**. The user will commit manually to see exactly what changed.

## Process

1. **Read the ticket.** Understand the objective, acceptance criteria, layer breakdown, and dependencies.
2. **Verify dependencies.** Check "blocked by" — ensure all prerequisites are done.
3. **Test-first approach:** 
   - Write tests first in `tests/CompetitionManager.Tests/` that express the acceptance criteria.
   - Run `dotnet test` to see them fail.
   - Write code to make them pass.
4. **Layer-aware implementation:**
   - If Domain is touched: define entities, value objects, aggregate roots, repository interfaces. Keep Domain pure.
   - If Application is touched: build services that orchestrate Domain and call repositories.
   - If Infrastructure is touched: implement repositories and external integrations.
   - If API is touched: add thin controllers that inject services and map DTOs.
5. **Validate boundaries.** Run:
   ```
   python3 scripts/validate_boundaries.py architecture.json --repo-root .
   ```
   Fix any violations before finishing.
6. **Build and test.** Run:
   ```
   dotnet build
   dotnet test
   ```
7. **Do NOT commit.** Stop here. The user will review and commit manually.
8. **Hand off.** Report what was implemented, any architecture decisions, and name the next stage: **"Ready for manual commit. After you commit, run `/code-review` for feedback."**

## Integration with `/teach` skill

As you implement, use the `.NET/C# Specialist` sub-agent. It will:
- Explain design patterns and SOLID principles as you apply them.
- Articulate why Domain is pure, why Application orchestrates, why API is thin.
- Teach idiomatic C# and async patterns.
- Make architecture decisions explicit.

Refer to the specialist by asking clarifying questions in the session; it's there to help you learn.

## Implementation checklist

- [ ] Ticket read and understood
- [ ] Tests written first (acceptance criteria → test code)
- [ ] Tests failing
- [ ] Code written to make tests pass
- [ ] All tests passing: `dotnet test`
- [ ] Boundary validation passes: `python3 scripts/validate_boundaries.py architecture.json --repo-root .`
- [ ] Build successful: `dotnet build`
- [ ] No compiler warnings (treat warnings as errors where possible)
- [ ] Naming conventions match `AGENTS.md`
- [ ] Code style follows C# conventions (use `dotnet format` if available)
- [ ] Tickets marked as "In Progress" in `tickets/` folder
- [ ] **NOT committed or pushed** (user will do this)

## Report template

After implementation, summarize:

```
## Implementation Complete: CMS-NNN

**What was done:**
- <bullet list of changes>

**Tests added:**
- <list of test classes/methods>

**Architecture decisions:**
- <explain why you chose this pattern/approach>

**Learning moment:**
- <one thing the .NET Specialist taught you>

**Next step:** Review the changes (`git diff`), then run `/code-review` for feedback. 
After you commit, the next ticket is ready.
```
