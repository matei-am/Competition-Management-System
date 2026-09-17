using CompetitionManager.Domain.ValueObjects;

namespace CompetitionManager.Application.DTOs;

public sealed record UpdateCompetitionRequest
{
    public string? Name { get; init; }

    public string? Rules { get; init; }

    public CompetitionStatus? Status { get; init; }

    public int? AthleteCount { get; init; }

    public int? ClubCount { get; init; }

    public int? RefereeCount { get; init; }
}