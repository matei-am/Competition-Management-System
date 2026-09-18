namespace CompetitionManager.Domain.Entities;

public sealed class Tatami
{
    public Tatami(Guid competitionId, int number)
    {
        ValidateCompetitionId(competitionId);
        ValidateNumber(number);

        Id = Guid.NewGuid();
        CompetitionId = competitionId;
        Number = number;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = CreatedAt;
    }

    public Guid Id { get; }

    public Guid CompetitionId { get; }

    public int Number { get; private set; }

    public DateTime CreatedAt { get; }

    public DateTime UpdatedAt { get; private set; }

    public void Renumber(int newNumber)
    {
        ValidateNumber(newNumber);
        Number = newNumber;
        Touch();
    }

    private static void ValidateCompetitionId(Guid competitionId)
    {
        if (competitionId == Guid.Empty)
        {
            throw new ArgumentException("Competition id is required.", nameof(competitionId));
        }
    }

    private static void ValidateNumber(int number)
    {
        if (number < 1)
        {
            throw new ArgumentException("Tatami number must be a positive integer.", nameof(number));
        }
    }

    private void Touch()
    {
        UpdatedAt = DateTime.UtcNow;
    }
}
