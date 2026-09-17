using CompetitionManager.Domain.ValueObjects;

namespace CompetitionManager.Application.DTOs;

public sealed record CreateCompetitionRequest
{
    public required string Name { get; init; }

    public DateTime Date { get; init; }

    public required string Location { get; init; }

    public int AthleteCount { get; init; }

    public int ClubCount { get; init; }

    public int RefereeCount { get; init; }

    public required string Rules { get; init; }

    public IEnumerable<CategoryDefinition>? WeightCategories { get; init; }

    public IEnumerable<CategoryDefinition>? AgeCategories { get; init; }
}