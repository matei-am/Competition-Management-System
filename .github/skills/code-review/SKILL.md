---
name: code-review
description: "Review a ticket's implementation on two axes: house standards (AGENTS.md, architecture) and spec conformance."
disable-model-invocation: true
---

Review the implementation of **one ticket**. Review on two axes:

1. **House standards:** Does it match `AGENTS.md`, `architecture.json`, naming conventions, code style, and testing discipline?
2. **Spec conformance:** Does it satisfy the acceptance criteria in `tickets/CMS-NNN.md` and the original Epic?

**Scope:** Review the diff (staged changes, not committed yet). Provide actionable feedback; suggest fixes where appropriate.

## Process

1. **Get the ticket and spec.** Read `tickets/CMS-NNN.md` and the original Epic to understand acceptance criteria.
2. **Review the diff.** Examine the code changes:
   - Architecture: are layer boundaries respected?
   - Naming: do files, classes, methods follow `AGENTS.md`?
   - Tests: do tests cover the acceptance criteria? Are they well-named?
   - Style: does code follow C# conventions? (Formatting, naming, structure)
   - Patterns: are SOLID principles applied? Is Domain pure? Is API thin?
3. **Spec conformance:** 
   - Does each acceptance criterion have a corresponding test?
   - Does the implementation fully deliver the ticket objective?
4. **Report findings.** Use this structure:
   - ✓ What's working well (celebrate good decisions)
   - ⚠ Suggestions for improvement (with specific line/file references)
   - ❌ Blockers (must fix before commit)
5. **Hand off.** Name the next step: **"Address any blockers, then commit. After commit, you're ready for the next ticket or a new feature grilling session."**

## Review template

```
# Code Review: CMS-NNN

## Summary
<One sentence: does this ticket meet the spec?>

## ✓ Strengths
- <Positive observation with reasoning>
- <Positive observation with reasoning>

## ⚠ Suggestions
- **[filename#L123]** <suggestion with context>
- **[filename#L456]** <suggestion with context>

## ❌ Blockers
- <Issue that must be fixed before commit>
- <Issue that must be fixed before commit>

## Spec Conformance Checklist
- [ ] Acceptance criterion 1: met
- [ ] Acceptance criterion 2: met
- [ ] Architecture boundaries: respected
- [ ] Tests: complete and passing
- [ ] Naming: consistent with `AGENTS.md`

## Verdict
**Ready to commit** / **Needs fixes** (specify which blockers)

## Next step
<If blockers, describe fixes. If clear, name the next ticket or session.>
```

## Review checklist

**Architecture & boundaries:**
- [ ] No forbidden cross-layer dependencies (verify with `python3 scripts/validate_boundaries.py`)
- [ ] Domain is pure (no I/O, no dependencies on Application/API/Infrastructure)
- [ ] Application services orchestrate Domain (not reimplementing business logic)
- [ ] API is thin (controllers only validate shape and call services)
- [ ] Infrastructure implements Domain interfaces (not the reverse)

**Naming & style (AGENTS.md):**
- [ ] Files: PascalCase with appropriate suffixes (Service, Repository, etc.)
- [ ] Classes: PascalCase, interfaces prefixed with `I`
- [ ] Methods: PascalCase, verb-first for commands
- [ ] Private fields: camelCase with `_` prefix
- [ ] Constants: UPPER_SNAKE_CASE

**Testing:**
- [ ] Test file naming: `<TargetClass>Tests.cs`
- [ ] Test method naming: `<Method>_<Scenario>_<Expected>`
- [ ] AAA pattern: Arrange, Act, Assert
- [ ] Tests are isolated (no inter-test dependencies)
- [ ] Happy path and edge cases covered

**Code quality:**
- [ ] No compilation warnings
- [ ] Consistent indentation (4 spaces)
- [ ] Line length reasonable (≤120 chars where practical)
- [ ] Comments explain *why*, not *what*
- [ ] No dead code or commented-out lines

## Tone

Be encouraging. This is a learning project. If something is wrong, explain *why* it's wrong and how to fix it—teaching as you review.
