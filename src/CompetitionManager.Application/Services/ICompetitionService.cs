using CompetitionManager.Application.DTOs;

namespace CompetitionManager.Application.Services;

public interface ICompetitionService
{
    Task<CompetitionResponse> CreateCompetitionAsync(CreateCompetitionRequest request);

    Task<CompetitionResponse> GetCompetitionByIdAsync(Guid id);

    Task<IReadOnlyList<CompetitionResponse>> GetAllCompetitionsAsync();

    Task<CompetitionResponse> UpdateCompetitionAsync(Guid id, UpdateCompetitionRequest request);

    Task DeleteCompetitionAsync(Guid id);
}
