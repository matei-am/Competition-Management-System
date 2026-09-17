using CompetitionManager.Domain.Entities;
using CompetitionManager.Domain.Repositories;

namespace CompetitionManager.Infrastructure.Repositories;

public sealed class CompetitionRepository : ICompetitionRepository
{
    private readonly Dictionary<Guid, Competition> _competitions = new();
    private readonly Dictionary<string, Guid> _nameIndex = new(StringComparer.OrdinalIgnoreCase);

    public Task<Competition> GetByIdAsync(Guid id)
    {
        if (_competitions.TryGetValue(id, out var competition))
        {
            return Task.FromResult(competition);
        }

        throw new KeyNotFoundException($"Competition with id '{id}' was not found.");
    }

    public Task<IEnumerable<Competition>> GetAllAsync()
    {
        return Task.FromResult<IEnumerable<Competition>>(_competitions.Values.ToList());
    }

    public Task AddAsync(Competition competition)
    {
        ArgumentNullException.ThrowIfNull(competition);

        if (_nameIndex.ContainsKey(competition.Name))
        {
            throw new InvalidOperationException($"Competition with name '{competition.Name}' already exists.");
        }

        _competitions[competition.Id] = competition;
        _nameIndex[competition.Name] = competition.Id;

        return Task.CompletedTask;
    }

    public Task UpdateAsync(Competition competition)
    {
        ArgumentNullException.ThrowIfNull(competition);

        if (!_competitions.ContainsKey(competition.Id))
        {
            throw new KeyNotFoundException($"Competition with id '{competition.Id}' was not found.");
        }

        var existingByName = _nameIndex.TryGetValue(competition.Name, out var existingId)
            && existingId != competition.Id;

        if (existingByName)
        {
            throw new InvalidOperationException($"Competition with name '{competition.Name}' already exists.");
        }

        _competitions[competition.Id] = competition;
        _nameIndex[competition.Name] = competition.Id;

        foreach (var (name, id) in _nameIndex.Where(item => item.Value == competition.Id).ToList())
        {
            if (!string.Equals(name, competition.Name, StringComparison.OrdinalIgnoreCase))
            {
                _nameIndex.Remove(name);
            }
        }

        return Task.CompletedTask;
    }

    public Task DeleteAsync(Guid id)
    {
        if (!_competitions.TryGetValue(id, out var competition))
        {
            throw new KeyNotFoundException($"Competition with id '{id}' was not found.");
        }

        _competitions.Remove(id);
        _nameIndex.Remove(competition.Name);

        return Task.CompletedTask;
    }

    public Task<bool> ExistsByNameAsync(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Task.FromResult(false);
        }

        return Task.FromResult(_nameIndex.ContainsKey(name.Trim()));
    }
}
