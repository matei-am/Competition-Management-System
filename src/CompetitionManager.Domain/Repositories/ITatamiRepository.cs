using CompetitionManager.Domain.Entities;

namespace CompetitionManager.Domain.Repositories;

public interface ITatamiRepository
{
    Task<Tatami> AddAsync(Tatami tatami);

    Task<Tatami?> GetByIdAsync(Guid id);

    Task<IReadOnlyList<Tatami>> GetByCompetitionIdAsync(Guid competitionId);

    Task RemoveAsync(Guid id);

    Task<int> CountByCompetitionIdAsync(Guid competitionId);
}
