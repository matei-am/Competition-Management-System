# AGENTS.md — Competition Management System

Behavioural instructions for AI coding agents working in this repository.

## Before making changes
1. Read `architecture.json` to understand the layer hierarchy and boundary rules.
2. Identify which layer the target file belongs to.
3. Check boundary rules — verify your change introduces no forbidden dependency.
4. Match existing patterns — read 2–3 neighbouring files before writing new code.

## Layer boundaries (hard rules — never violate)

| Layer | Directory | May depend on | Must NOT depend on |
|-------|-----------|---------------|-------------------|
| **API** | `src/CompetitionManager.Api` | Application, Domain | Infrastructure (except DI config), other layer internals |
| **Application** | `src/CompetitionManager.Application` | Domain | API, Infrastructure (except DI interfaces) |
| **Domain** | `src/CompetitionManager.Domain` | (nothing above) | API, Application, Infrastructure |
| **Infrastructure** | `src/CompetitionManager.Infrastructure` | Domain (interfaces only) | API, Application business logic |

**Key principle:** Dependency flow is strictly downward: API → Application → Domain ← Infrastructure. Domain never knows about layers above it. Infrastructure provides implementations *of* Domain interfaces.

## Creating new files

- **New entity or value object:** add to `Domain/`. Update domain model docs if adding a new aggregate root.
- **New use case / application service:** add to `Application/Services/`. Implement with `IApplicationService` pattern. Register in DI.
- **New API endpoint:** add to `Api/Controllers/`. Inject application services only. Map DTOs to domain objects in the application layer, not in controllers.
- **New data access:** add to `Infrastructure/Data/` or `Infrastructure/Repositories/`. Implement domain repository interfaces.
- **New external service integration:** add to `Infrastructure/ExternalServices/`. Wrap in a domain-level interface.

## Naming conventions

- **Files:** PascalCase (e.g., `CompetitionService.cs`, `CompetitionRepository.cs`).
- **Classes:** PascalCase. Suffixes: `Service` (application), `Repository` (data), `Validator` (domain rules), `Specification` (queries).
- **Interfaces:** PascalCase prefixed with `I` (e.g., `ICompetitionRepository`, `ICompetitionService`).
- **Methods:** PascalCase, verb-first for commands (e.g., `CreateCompetition`, `UpdateResult`).
- **Properties:** PascalCase. Use auto-properties where possible.
- **Private fields:** camelCase prefixed with `_` (e.g., `_logger`, `_repository`).
- **Constants:** UPPER_SNAKE_CASE in a static `Constants` class or top of file.

## Code style

- **Formatting:** Follow [C# Coding Conventions](https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions).
- **Indentation:** 4 spaces, no tabs.
- **Line length:** Aim for ≤120 characters.
- **One responsibility per class.** Use SOLID principles: Single responsibility, Open/closed, Liskov, Interface segregation, Dependency inversion.
- **Error handling:** Use exceptions for exceptional cases, not control flow. Log with the injected `ILogger`.
- **Async/await:** Use `async/await` for I/O operations. Avoid `Task.Result` and `Task.Wait()`.
- **Null handling:** Use null-coalescing operators (`??`, `??=`). C# 8+: consider nullable reference types.

## Testing

- **Framework:** MSTest.
- **Test locations:** `tests/CompetitionManager.Tests/` (mirror the layer structure).
- **Naming:** `<TargetClass>Tests.cs`. Test methods: `<Method>_<Scenario>_<Expected>` (e.g., `CreateCompetition_WithValidInput_ReturnsCompetition`).
- **Coverage:** Add tests for every public API in the Application and Domain layers. Controllers require only happy-path integration tests.
- **Arrange-Act-Assert:** Always use AAA pattern.
- **Prepare the host:** Before running tests, run `dotnet build` and `dotnet test`.

## Safety-critical code

No paths are designated as safety-critical at this stage. As the system evolves and handles sensitive data (e.g., competition results, user authentication), authentication and result-validation code will be marked for review.

## Commits

- **Prefix:** `[CMS-NNN]` (Competition Management System ticket number from `tickets/` folder).
- **Format:** `[CMS-NNN] Brief description of change`.
- **Branch:** `feature/CMS-NNN` for features, `fix/CMS-NNN` for fixes.
- **Message body:** Explain *why*, not just *what*. Reference architecture decisions if applicable.
- **Example:** `[CMS-001] Add domain model for competitions. Creates Competition aggregate root with Specifications for querying. Domain remains persistence-agnostic.`

## Agent skills & docs

- **Issue tracker:** Read `docs/agents/issue-tracker.md` for ticket structure and labeling conventions.
- **Architecture is the single source of truth:** `architecture.json` defines layers, boundary rules, and constraints. All architectural decisions must be reflected there.
- **Skill invocation discipline:** User-invoked skills fire ONLY when the user types the command (`/grill-me`, `/to-spec`, `/to-tickets`, `/implement`, `/code-review`). Never auto-run the next pipeline stage; end each stage by naming it.
- **Teaching during implementation:** The `/teach` skill is integrated with `/implement`. When `/implement` is run, the .NET/C# Specialist will interleave learning moments: explaining design patterns, architecture decisions, and idiomatic C# as they are applied.
- **No auto-commit:** The `/implement` skill writes code, runs tests, and validates boundaries but **does not** commit or push. You manually commit to understand exactly what changed.

## Key learning goals for this project

This is a learning project for .NET and C#. As you work:

1. **Understand the 4-layer architecture:** Why Domain is pure, why Application orchestrates, why API is thin.
2. **SOLID principles:** Each phase will reinforce Single Responsibility and Dependency Inversion.
3. **Async patterns:** .NET's async/await is pervasive; learn its idioms early.
4. **Entity Framework or raw SQL:** Infrastructure layer choices and their tradeoffs.
5. **Testing discipline:** Test-first development with MSTest; write tests before code.

The `/teach` skill and specialist agent are there to make these explicit as you build.
