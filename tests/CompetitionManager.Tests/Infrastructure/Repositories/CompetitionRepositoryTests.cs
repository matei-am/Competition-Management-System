using CompetitionManager.Domain.Entities;
using CompetitionManager.Domain.ValueObjects;
using CompetitionManager.Infrastructure.Repositories;

namespace CompetitionManager.Tests.Infrastructure.Repositories;

public class CompetitionRepositoryTests
{
    [Fact]
    public async Task AddAsync_WithValidInput_StoresSuccessfully()
    {
        // Arrange
        var repository = new CompetitionRepository();
        var competition = CreateCompetition("Spring Cup");

        // Act
        await repository.AddAsync(competition);

        // Assert
        var storedCompetition = await repository.GetByIdAsync(competition.Id);
        Assert.Equal(competition.Id, storedCompetition.Id);
        Assert.Equal("Spring Cup", storedCompetition.Name);
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsCompetition()
    {
        // Arrange
        var repository = new CompetitionRepository();
        var competition = CreateCompetition("Summer Cup");
        await repository.AddAsync(competition);

        // Act
        var result = await repository.GetByIdAsync(competition.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(competition.Id, result.Id);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ThrowsKeyNotFoundException()
    {
        // Arrange
        var repository = new CompetitionRepository();

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => repository.GetByIdAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task GetAllAsync_WithNoCompetitions_ReturnsEmptyList()
    {
        // Arrange
        var repository = new CompetitionRepository();

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllAsync_WithMultipleCompetitions_ReturnsAll()
    {
        // Arrange
        var repository = new CompetitionRepository();
        var first = CreateCompetition("Spring Cup");
        var second = CreateCompetition("Summer Cup");
        await repository.AddAsync(first);
        await repository.AddAsync(second);

        // Act
        var result = (await repository.GetAllAsync()).ToList();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains(result, c => c.Id == first.Id);
        Assert.Contains(result, c => c.Id == second.Id);
    }

    [Fact]
    public async Task UpdateAsync_WithValidCompetition_UpdatesSuccessfully()
    {
        // Arrange
        var repository = new CompetitionRepository();
        var competition = CreateCompetition("Autumn Cup");
        await repository.AddAsync(competition);
        competition.UpdateName("Winter Cup");

        // Act
        await repository.UpdateAsync(competition);

        // Assert
        var updatedCompetition = await repository.GetByIdAsync(competition.Id);
        Assert.Equal("Winter Cup", updatedCompetition.Name);
    }

    [Fact]
    public async Task UpdateAsync_WithNonExistentId_ThrowsKeyNotFoundException()
    {
        // Arrange
        var repository = new CompetitionRepository();
        var competition = CreateCompetition("Missing Cup");

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => repository.UpdateAsync(competition));
    }

    [Fact]
    public async Task DeleteAsync_WithValidId_RemovesCompetition()
    {
        // Arrange
        var repository = new CompetitionRepository();
        var competition = CreateCompetition("Deleted Cup");
        await repository.AddAsync(competition);

        // Act
        await repository.DeleteAsync(competition.Id);

        // Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => repository.GetByIdAsync(competition.Id));
    }

    [Fact]
    public async Task DeleteAsync_WithInvalidId_ThrowsKeyNotFoundException()
    {
        // Arrange
        var repository = new CompetitionRepository();

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => repository.DeleteAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task ExistsByNameAsync_WithExistingName_ReturnsTrue()
    {
        // Arrange
        var repository = new CompetitionRepository();
        var competition = CreateCompetition("Existing Cup");
        await repository.AddAsync(competition);

        // Act
        var result = await repository.ExistsByNameAsync("Existing Cup");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task ExistsByNameAsync_WithNonExistentName_ReturnsFalse()
    {
        // Arrange
        var repository = new CompetitionRepository();

        // Act
        var result = await repository.ExistsByNameAsync("Missing Cup");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task AddAsync_WithDuplicateName_ThrowsInvalidOperationException()
    {
        // Arrange
        var repository = new CompetitionRepository();
        var first = CreateCompetition("Duplicate Cup");
        var second = CreateCompetition("Duplicate Cup");
        await repository.AddAsync(first);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => repository.AddAsync(second));
    }

    [Fact]
    public async Task UpdateAsync_WithDuplicateName_ThrowsInvalidOperationException()
    {
        // Arrange
        var repository = new CompetitionRepository();
        var existing = CreateCompetition("Original Cup");
        var updated = CreateCompetition("Updated Cup");
        await repository.AddAsync(existing);
        await repository.AddAsync(updated);

        updated.UpdateName("Original Cup");

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => repository.UpdateAsync(updated));
    }

    private static Competition CreateCompetition(string name)
    {
        return new Competition(
            name: name,
            date: new DateTime(2026, 5, 14),
            location: "Lisbon",
            athleteCount: 4,
            clubCount: 4,
            refereeCount: 4,
            rules: "Standard competition rules",
            weightCategories: [new CategoryDefinition("Lightweight")],
            ageCategories: [new CategoryDefinition("Senior")]);
    }
}
