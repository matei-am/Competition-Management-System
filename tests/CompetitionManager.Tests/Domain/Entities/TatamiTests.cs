using CompetitionManager.Domain.Entities;

namespace CompetitionManager.Tests.Domain.Entities;

public class TatamiTests
{
    [Fact]
    public void Constructor_WithValidInput_CreatesTatami()
    {
        var competitionId = Guid.NewGuid();

        var tatami = new Tatami(competitionId, 1);

        Assert.NotEqual(Guid.Empty, tatami.Id);
        Assert.Equal(competitionId, tatami.CompetitionId);
        Assert.Equal(1, tatami.Number);
    }

    [Fact]
    public void Constructor_WithEmptyCompetitionId_ThrowsArgumentException()
    {
        var action = () => new Tatami(Guid.Empty, 1);

        Assert.Throws<ArgumentException>(action);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Constructor_WithNonPositiveNumber_ThrowsArgumentException(int number)
    {
        var action = () => new Tatami(Guid.NewGuid(), number);

        Assert.Throws<ArgumentException>(action);
    }

    [Fact]
    public void Renumber_WithPositiveNumber_UpdatesNumber()
    {
        var tatami = new Tatami(Guid.NewGuid(), 1);

        tatami.Renumber(2);

        Assert.Equal(2, tatami.Number);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Renumber_WithNonPositiveNumber_ThrowsArgumentException(int number)
    {
        var tatami = new Tatami(Guid.NewGuid(), 1);

        Assert.Throws<ArgumentException>(() => tatami.Renumber(number));
    }
}
