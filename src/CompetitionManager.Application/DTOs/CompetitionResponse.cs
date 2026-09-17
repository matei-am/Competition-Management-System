using CompetitionManager.Domain.ValueObjects;

namespace CompetitionManager.Application.DTOs;

public sealed record CompetitionResponse
{
    public Guid Id { get; init; }

    public required string Name { get; init; }

    public DateTime Date { get; init; }

    public required string Location { get; init; }

    public CompetitionStatus Status { get; init; }

    public int AthleteCount { get; init; }

    public int ClubCount { get; init; }

    public int RefereeCount { get; init; }

    public required string Rules { get; init; }

    public IReadOnlyList<CategoryDefinition> WeightCategories { get; init; } = [];

    public IReadOnlyList<CategoryDefinition> AgeCategories { get; init; } = [];

    public DateTime CreatedAt { get; init; }

    public DateTime UpdatedAt { get; init; }
}