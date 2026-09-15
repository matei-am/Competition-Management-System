using CompetitionManager.Domain.Entities;
using CompetitionManager.Domain.ValueObjects;

namespace CompetitionManager.Tests.Domain.Entities;

public class CompetitionTests
{
    [Fact]
    public void Constructor_WithValidInput_CreatesCompetition()
    {
        var competition = CreateCompetition();

        Assert.NotEqual(Guid.Empty, competition.Id);
        Assert.Equal("Open Championship", competition.Name);
        Assert.Equal(CompetitionStatus.Upcoming, competition.Status);
        Assert.Equal(4, competition.AthleteCount);
        Assert.Equal(4, competition.ClubCount);
        Assert.Equal(4, competition.RefereeCount);
        Assert.Single(competition.WeightCategories);
        Assert.Single(competition.AgeCategories);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithInvalidName_ThrowsArgumentException(string? name)
    {
        var action = () => new Competition(name!, DateTime.Today, "Arena", 4, 4, 4, "Rules");

        Assert.Throws<ArgumentException>(action);
    }

    [Theory]
    [InlineData(3, 4, 4, "athleteCount")]
    [InlineData(4, 3, 4, "clubCount")]
    [InlineData(4, 4, 3, "refereeCount")]
    public void Constructor_WithCountLessThanFour_ThrowsArgumentException(
        int athleteCount,
        int clubCount,
        int refereeCount,
        string parameterName)
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            new Competition("Open Championship", DateTime.Today, "Arena", athleteCount, clubCount, refereeCount, "Rules"));

        Assert.Equal(parameterName, exception.ParamName);
    }

    [Fact]
    public void TransitionStatus_FromUpcomingToActive_Succeeds()
    {
        var competition = CreateCompetition();

        competition.TransitionStatus(CompetitionStatus.Active);

        Assert.Equal(CompetitionStatus.Active, competition.Status);
    }

    [Fact]
    public void TransitionStatus_FromActiveToFinished_Succeeds()
    {
        var competition = CreateCompetition();
        competition.TransitionStatus(CompetitionStatus.Active);

        competition.TransitionStatus(CompetitionStatus.Finished);

        Assert.Equal(CompetitionStatus.Finished, competition.Status);
    }

    [Theory]
    [InlineData(CompetitionStatus.Finished)]
    [InlineData(CompetitionStatus.Upcoming)]
    public void TransitionStatus_FromUpcomingToInvalidStatus_ThrowsException(CompetitionStatus status)
    {
        var competition = CreateCompetition();

        Assert.Throws<InvalidOperationException>(() => competition.TransitionStatus(status));
    }

    [Fact]
    public void TransitionStatus_BackwardMove_ThrowsException()
    {
        var competition = CreateCompetition();
        competition.TransitionStatus(CompetitionStatus.Active);
        competition.TransitionStatus(CompetitionStatus.Finished);

        Assert.Throws<InvalidOperationException>(() => competition.TransitionStatus(CompetitionStatus.Active));
    }

    [Fact]
    public void CanUpdateName_InUpcomingStatus_ReturnsTrue()
    {
        Assert.True(CreateCompetition().CanUpdateName());
    }

    [Fact]
    public void CanUpdateName_InActiveStatus_ReturnsFalse()
    {
        var competition = CreateCompetition();
        competition.TransitionStatus(CompetitionStatus.Active);

        Assert.False(competition.CanUpdateName());
    }

    [Fact]
    public void CanUpdateName_InFinishedStatus_ReturnsFalse()
    {
        var competition = CreateCompetition();
        competition.TransitionStatus(CompetitionStatus.Active);
        competition.TransitionStatus(CompetitionStatus.Finished);

        Assert.False(competition.CanUpdateName());
    }

    [Fact]
    public void CanUpdateRules_InUpcomingAndActiveStatus_ReturnsTrue()
    {
        var competition = CreateCompetition();
        Assert.True(competition.CanUpdateRules());

        competition.TransitionStatus(CompetitionStatus.Active);

        Assert.True(competition.CanUpdateRules());
    }

    [Fact]
    public void CanUpdateRules_InFinishedStatus_ReturnsFalse()
    {
        var competition = CreateCompetition();
        competition.TransitionStatus(CompetitionStatus.Active);
        competition.TransitionStatus(CompetitionStatus.Finished);

        Assert.False(competition.CanUpdateRules());
    }

    [Fact]
    public void CanChangeCount_InUpcomingStatus_ReturnsTrue()
    {
        Assert.True(CreateCompetition().CanChangeCount());
    }

    [Fact]
    public void CanChangeCount_InActiveStatus_ReturnsFalse()
    {
        var competition = CreateCompetition();
        competition.TransitionStatus(CompetitionStatus.Active);

        Assert.False(competition.CanChangeCount());
    }

    [Fact]
    public void UpdateName_InUpcomingStatus_UpdatesSuccessfully()
    {
        var competition = CreateCompetition();

        competition.UpdateName("Regional Championship");

        Assert.Equal("Regional Championship", competition.Name);
    }

    [Fact]
    public void UpdateName_InActiveStatus_ThrowsException()
    {
        var competition = CreateCompetition();
        competition.TransitionStatus(CompetitionStatus.Active);

        Assert.Throws<InvalidOperationException>(() => competition.UpdateName("Regional Championship"));
    }

    [Fact]
    public void UpdateRules_InActiveStatus_UpdatesSuccessfully()
    {
        var competition = CreateCompetition();
        competition.TransitionStatus(CompetitionStatus.Active);

        competition.UpdateRules("Updated rules");

        Assert.Equal("Updated rules", competition.Rules);
    }

    [Fact]
    public void UpdateCounts_InUpcomingStatus_UpdatesSuccessfully()
    {
        var competition = CreateCompetition();

        competition.UpdateCounts(8, 6, 5);

        Assert.Equal(8, competition.AthleteCount);
        Assert.Equal(6, competition.ClubCount);
        Assert.Equal(5, competition.RefereeCount);
    }

    [Fact]
    public void UpdateCounts_WithInvalidCount_ThrowsArgumentException()
    {
        var competition = CreateCompetition();

        Assert.Throws<ArgumentException>(() => competition.UpdateCounts(3, 4, 4));
    }

    [Fact]
    public void CategoryCollections_CannotBeMutatedThroughAggregate()
    {
        var competition = CreateCompetition();

        Assert.Throws<NotSupportedException>(() =>
            ((IList<CategoryDefinition>)competition.WeightCategories)[0] = new CategoryDefinition("Heavy"));
    }

    private static Competition CreateCompetition()
    {
        return new Competition(
            "Open Championship",
            new DateTime(2026, 10, 1),
            "Arena",
            4,
            4,
            4,
            "Rules",
            [new CategoryDefinition("Heavy")],
            [new CategoryDefinition("Senior")]);
    }
}