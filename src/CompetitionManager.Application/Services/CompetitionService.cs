using CompetitionManager.Application.DTOs;
using CompetitionManager.Application.Exceptions;
using CompetitionManager.Domain.Entities;
using CompetitionManager.Domain.Repositories;

namespace CompetitionManager.Application.Services;

public sealed class CompetitionService
{
    private readonly ICompetitionRepository competitionRepository;

    public CompetitionService(ICompetitionRepository competitionRepository)
    {
        this.competitionRepository = competitionRepository
            ?? throw new ArgumentNullException(nameof(competitionRepository));
    }

    public async Task<CompetitionResponse> CreateCompetitionAsync(CreateCompetitionRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        ValidateRules(request.Rules);

        var competition = new Competition(
            request.Name,
            request.Date,
            request.Location,
            request.AthleteCount,
            request.ClubCount,
            request.RefereeCount,
            request.Rules,
            request.WeightCategories,
            request.AgeCategories);

        await competitionRepository.AddAsync(competition);
        return Map(competition);
    }

    public async Task<CompetitionResponse> GetCompetitionByIdAsync(Guid id)
    {
        var competition = await GetCompetitionAsync(id);
        return Map(competition);
    }

    public async Task<IReadOnlyList<CompetitionResponse>> GetAllCompetitionsAsync()
    {
        var competitions = await competitionRepository.GetAllAsync();
        return competitions.Select(Map).ToList();
    }

    public async Task<CompetitionResponse> UpdateCompetitionAsync(
        Guid id,
        UpdateCompetitionRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        ValidateUpdateRequest(request);

        var competition = await GetCompetitionAsync(id);

        if (request.Name is not null)
        {
            competition.UpdateName(request.Name);
        }

        if (request.Rules is not null)
        {
            ValidateRules(request.Rules);
            competition.UpdateRules(request.Rules);
        }

        if (HasCountChanges(request))
        {
            competition.UpdateCounts(
                request.AthleteCount ?? competition.AthleteCount,
                request.ClubCount ?? competition.ClubCount,
                request.RefereeCount ?? competition.RefereeCount);
        }

        if (request.Status is not null)
        {
            competition.TransitionStatus(request.Status.Value);
        }

        await competitionRepository.UpdateAsync(competition);
        return Map(competition);
    }

    public async Task DeleteCompetitionAsync(Guid id)
    {
        try
        {
            await competitionRepository.DeleteAsync(id);
        }
        catch (KeyNotFoundException exception)
        {
            throw CreateNotFoundException(id, exception);
        }
    }

    private async Task<Competition> GetCompetitionAsync(Guid id)
    {
        try
        {
            return await competitionRepository.GetByIdAsync(id);
        }
        catch (KeyNotFoundException exception)
        {
            throw CreateNotFoundException(id, exception);
        }
    }

    private static void ValidateUpdateRequest(UpdateCompetitionRequest request)
    {
        if (request.Name is null
            && request.Rules is null
            && request.Status is null
            && request.AthleteCount is null
            && request.ClubCount is null
            && request.RefereeCount is null)
        {
            throw new InvalidOperationException("At least one competition update must be supplied.");
        }
    }

    private static void ValidateRules(string rules)
    {
        if (string.IsNullOrWhiteSpace(rules))
        {
            throw new ArgumentException("Competition rules are required.", nameof(rules));
        }
    }

    private static bool HasCountChanges(UpdateCompetitionRequest request)
    {
        return request.AthleteCount is not null
            || request.ClubCount is not null
            || request.RefereeCount is not null;
    }

    private static NotFoundException CreateNotFoundException(Guid id, Exception innerException)
    {
        return new NotFoundException($"Competition with id '{id}' was not found.", innerException);
    }

    private static CompetitionResponse Map(Competition competition)
    {
        return new CompetitionResponse
        {
            Id = competition.Id,
            Name = competition.Name,
            Date = competition.Date,
            Location = competition.Location,
            Status = competition.Status,
            AthleteCount = competition.AthleteCount,
            ClubCount = competition.ClubCount,
            RefereeCount = competition.RefereeCount,
            Rules = competition.Rules,
            WeightCategories = competition.WeightCategories.ToList(),
            AgeCategories = competition.AgeCategories.ToList(),
            CreatedAt = competition.CreatedAt,
            UpdatedAt = competition.UpdatedAt
        };
    }
}