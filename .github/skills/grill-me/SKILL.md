---
name: grill-me
description: "Grill the user relentlessly about a plan, decision, or idea until we reach shared understanding. Use when the user wants to stress-test their thinking, or uses 'grill' trigger phrases."
disable-model-invocation: true
---

Interview the user relentlessly about every aspect of a plan, decision, or feature idea until we reach a shared understanding.

Walk down each branch of the decision tree, resolving dependencies between decisions one-by-one. For each question:
- Provide your recommended answer or next step.
- Ask the questions one at a time. Multiple questions at once are bewildering.
- If a *fact* can be found by exploring the environment (filesystem, codebase, tests), look it up rather than asking.
- Use `architecture.json` and `AGENTS.md` for facts about layers, boundaries, naming, and constraints.

For this .NET/C# project:
- Reference the layer structure in `architecture.json`.
- Ask about domain concepts, not implementation details (we'll decide those later).
- When drilling into a feature, separate *what* (domain logic) from *how* (which layer, which pattern).

Do not act on it until the user confirms we have reached a shared understanding and you summarize it back.

## Summary template

Once we've grilled it, summarize in this format:

```
## Shared Understanding

**Feature/Decision:** <one sentence>

**Domain model changes:**
- New entities: <list>
- Modified entities: <list>
- Domain rules: <list>

**Layers impacted:**
- API: <describe changes>
- Application: <describe services/use cases>
- Domain: <describe model>
- Infrastructure: <describe data/external services>

**Open questions:** <list any remaining decisions>

**Next step:** Run `/to-spec` to formalize this into a spec.
```
