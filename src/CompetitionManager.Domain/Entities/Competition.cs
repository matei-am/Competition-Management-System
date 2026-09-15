using CompetitionManager.Domain.ValueObjects;

namespace CompetitionManager.Domain.Entities;

public sealed class Competition
{
    private const int MinimumParticipantCount = 4;
    private readonly List<CategoryDefinition> weightCategories;
    private readonly List<CategoryDefinition> ageCategories;

    public Competition(
        string name,
        DateTime date,
        string location,
        int athleteCount,
        int clubCount,
        int refereeCount,
        string rules,
        IEnumerable<CategoryDefinition>? weightCategories = null,
        IEnumerable<CategoryDefinition>? ageCategories = null)
    {
        ValidateName(name);
        ValidateLocation(location);
        ValidateRules(rules);
        ValidateCount(athleteCount, nameof(athleteCount));
        ValidateCount(clubCount, nameof(clubCount));
        ValidateCount(refereeCount, nameof(refereeCount));

        Id = Guid.NewGuid();
        Name = name.Trim();
        Date = date;
        Location = location.Trim();
        Status = CompetitionStatus.Upcoming;
        AthleteCount = athleteCount;
        ClubCount = clubCount;
        RefereeCount = refereeCount;
        Rules = rules;
        this.weightCategories = CopyCategories(weightCategories, nameof(weightCategories));
        this.ageCategories = CopyCategories(ageCategories, nameof(ageCategories));
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = CreatedAt;
    }

    public Guid Id { get; }

    public string Name { get; private set; }

    public DateTime Date { get; }

    public string Location { get; }

    public CompetitionStatus Status { get; private set; }

    public int AthleteCount { get; private set; }

    public int ClubCount { get; private set; }

    public int RefereeCount { get; private set; }

    public string Rules { get; private set; }

    public IReadOnlyList<CategoryDefinition> WeightCategories => weightCategories.AsReadOnly();

    public IReadOnlyList<CategoryDefinition> AgeCategories => ageCategories.AsReadOnly();

    public DateTime CreatedAt { get; }

    public DateTime UpdatedAt { get; private set; }

    public bool CanUpdateName() => Status == CompetitionStatus.Upcoming;

    public bool CanUpdateRules() => Status != CompetitionStatus.Finished;

    public bool CanChangeCount() => Status == CompetitionStatus.Upcoming;

    public void TransitionStatus(CompetitionStatus newStatus)
    {
        if (newStatus != Status + 1)
        {
            throw new InvalidOperationException(
                $"Competition status can only move forward one step from {Status}.");
        }

        Status = newStatus;
        Touch();
    }

    public void UpdateName(string name)
    {
        if (!CanUpdateName())
        {
            throw new InvalidOperationException("Competition name can only be updated while upcoming.");
        }

        ValidateName(name);
        Name = name.Trim();
        Touch();
    }

    public void UpdateRules(string rules)
    {
        if (!CanUpdateRules())
        {
            throw new InvalidOperationException("Competition rules cannot be updated after it is finished.");
        }

        ValidateRules(rules);
        Rules = rules;
        Touch();
    }

    public void UpdateCounts(int athleteCount, int clubCount, int refereeCount)
    {
        if (!CanChangeCount())
        {
            throw new InvalidOperationException("Competition counts can only be changed while upcoming.");
        }

        ValidateCount(athleteCount, nameof(athleteCount));
        ValidateCount(clubCount, nameof(clubCount));
        ValidateCount(refereeCount, nameof(refereeCount));

        AthleteCount = athleteCount;
        ClubCount = clubCount;
        RefereeCount = refereeCount;
        Touch();
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Competition name is required.", nameof(name));
        }
    }

    private static void ValidateLocation(string location)
    {
        if (string.IsNullOrWhiteSpace(location))
        {
            throw new ArgumentException("Competition location is required.", nameof(location));
        }
    }

    private static void ValidateRules(string rules)
    {
        ArgumentNullException.ThrowIfNull(rules);
    }

    private static void ValidateCount(int count, string parameterName)
    {
        if (count < MinimumParticipantCount)
        {
            throw new ArgumentException(
                $"{parameterName} must be at least {MinimumParticipantCount}.",
                parameterName);
        }
    }

    private static List<CategoryDefinition> CopyCategories(
        IEnumerable<CategoryDefinition>? categories,
        string parameterName)
    {
        if (categories is null)
        {
            return [];
        }

        var copiedCategories = categories.ToList();
        if (copiedCategories.Any(category => category is null))
        {
            throw new ArgumentException("Categories cannot contain null values.", parameterName);
        }

        return copiedCategories;
    }

    private void Touch()
    {
        UpdatedAt = DateTime.UtcNow;
    }
}