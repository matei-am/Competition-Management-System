using CompetitionManager.Domain.Entities;

namespace CompetitionManager.Domain.Repositories;

public interface ICompetitionRepository
{
    Task<Competition> GetByIdAsync(Guid id);

    Task<IEnumerable<Competition>> GetAllAsync();

    Task AddAsync(Competition competition);

    Task UpdateAsync(Competition competition);

    Task DeleteAsync(Guid id);

    Task<bool> ExistsByNameAsync(string name);
}
