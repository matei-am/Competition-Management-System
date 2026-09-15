---
name: teach
description: "Learn .NET and C# concepts interactively. Explain design patterns, architecture decisions, and idioms as they apply to the current code."
disable-model-invocation: true
---

Learn .NET and C# as you build. Use this skill when you want to understand *why* a design choice was made, learn a pattern, or understand an idiom.

This skill is **integrated with `/implement`**. As you implement a ticket, the `.NET/C# Specialist` sub-agent will:
- Explain architectural decisions in real-time
- Teach design patterns (Dependency Injection, Repository, Service, etc.)
- Show idiomatic C# (async/await, LINQ, null-coalescing, etc.)
- Articulate SOLID principles as they're applied
- Clarify why Domain is pure, why Application orchestrates, why API is thin

## How to use it

**Option 1: Passive learning during `/implement`**
- Run `/implement` normally. The `.NET/C# Specialist` will interleave teaching moments.
- Look for sections marked **"💡 Learning moment:"** in the output.

**Option 2: Active learning with `/teach`**
- Ask the specialist directly about a concept:
  - *"Explain the Repository pattern and why we're using it."*
  - *"What's the difference between a value object and an entity?"*
  - *"Show me idiomatic C# for async/await in this context."*
  - *"Why is the Application layer orchestrating here instead of the Domain?"*

**Option 3: Deep dive into a pattern**
- Ask the specialist to teach a specific pattern before you implement it:
  - *"Teach me about dependency injection in .NET."*
  - *"Explain aggregate roots and bounded contexts."*
  - *"What are SOLID principles and how do we apply them in this repo?"*

## Topics the specialist can teach

### Architecture & Design
- **4-layer architecture:** Why each layer exists, what belongs where, dependency directions.
- **Domain-Driven Design (DDD):** Entities, value objects, aggregates, repositories, bounded contexts.
- **SOLID principles:** Single Responsibility, Open/Closed, Liskov, Interface Segregation, Dependency Inversion—and how they shape the codebase.
- **Design patterns:** Dependency Injection, Repository, Service, Specification, Factory, etc.

### .NET & C# Idioms
- **Async/await:** How to write async code, avoid blocking, handle exceptions.
- **Dependency Injection:** .NET's built-in DI container, service lifetimes (transient, scoped, singleton).
- **LINQ:** Queries, projections, deferred execution.
- **Null handling:** Null-coalescing (`??`, `??=`), nullable reference types (C# 8+).
- **Collections:** Immutable collections, readonly lists, when to use each.
- **Exception handling:** When to throw, when to catch, custom exceptions for domain logic.

### Testing
- **MSTest basics:** Arrange-Act-Assert, test naming, assertions.
- **Unit testing:** Testing entities, value objects, application services in isolation.
- **Integration testing:** Testing repositories and external service integrations.
- **Test data builders:** Fixtures and builders for complex test setup.

### Practices
- **Naming conventions:** When to use suffixes (Service, Repository, Validator), why naming matters.
- **Code style:** Formatting, indentation, line breaks, readability.
- **Commits:** Clear commit messages, atomic commits, using tickets for traceability.
- **Code review:** What to look for, how to give and receive feedback.

## Learning loop

The best way to learn is **do → review → reflect → do again**:

1. **Implement** a ticket with guidance from the `.NET/C# Specialist`.
2. **Review** the code with `/code-review` and learn from feedback.
3. **Reflect** on what you learned: "Why was the pattern useful? When would I use it again?"
4. **Implement** the next ticket, applying what you learned.

## Teaching principles

The specialist follows these principles:

- **Explain the *why*, not just the *how*.** Not "use `async/await`" but "async/await lets us scale to many concurrent requests without blocking threads."
- **Connect to the architecture.** Explain design patterns in the context of the 4-layer model.
- **Use real code examples.** Teach by showing code in this repo, not abstract examples.
- **Encourage questions.** Learning is iterative; ask clarifying questions.
- **Reference authoritative sources.** Point to Microsoft docs, SOLID papers, DDD books when they apply.

## Quick-start topics

If you're new to .NET, ask the specialist about these first:

1. **"Explain the 4-layer architecture and why it matters."**
2. **"What is dependency injection and how does .NET use it?"**
3. **"What's the difference between entities, value objects, and DTOs?"**
4. **"Teach me about the Repository pattern."**
5. **"Show me idiomatic async/await in C#."**

Then implement your first ticket and learn by doing.
