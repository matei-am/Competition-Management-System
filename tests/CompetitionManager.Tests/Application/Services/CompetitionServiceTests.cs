using CompetitionManager.Application.DTOs;
using CompetitionManager.Application.Exceptions;
using CompetitionManager.Application.Services;
using CompetitionManager.Domain.ValueObjects;
using CompetitionManager.Infrastructure.Repositories;

namespace CompetitionManager.Tests.Application.Services;

public class CompetitionServiceTests
{
    [Fact]
    public async Task CreateCompetition_WithValidRequest_ReturnsUpcomingCompetition()
    {
        var service = CreateService();
        var request = CreateRequest();

        var result = await service.CreateCompetitionAsync(request);

        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal("Spring Cup", result.Name);
        Assert.Equal(CompetitionStatus.Upcoming, result.Status);
    }

    [Fact]
    public async Task CreateCompetition_WithInvalidCount_ThrowsException()
    {
        var service = CreateService();
        var request = CreateRequest() with { AthleteCount = 3 };

        await Assert.ThrowsAsync<ArgumentException>(() => service.CreateCompetitionAsync(request));
    }

    [Fact]
    public async Task CreateCompetition_WithDuplicateName_ThrowsException()
    {
        var service = CreateService();
        await service.CreateCompetitionAsync(CreateRequest());

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.CreateCompetitionAsync(CreateRequest()));

        Assert.Contains("already exists", exception.Message);
    }

    [Fact]
    public async Task GetCompetitionById_WithValidId_ReturnsCompetition()
    {
        var service = CreateService();
        var created = await service.CreateCompetitionAsync(CreateRequest());

        var result = await service.GetCompetitionByIdAsync(created.Id);

        Assert.Equal(created.Id, result.Id);
    }

    [Fact]
    public async Task GetCompetitionById_WithInvalidId_ThrowsNotFoundException()
    {
        var service = CreateService();

        await Assert.ThrowsAsync<NotFoundException>(() =>
            service.GetCompetitionByIdAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task GetAllCompetitions_WithMultiple_ReturnsAll()
    {
        var service = CreateService();
        await service.CreateCompetitionAsync(CreateRequest());
        await service.CreateCompetitionAsync(CreateRequest() with { Name = "Summer Cup" });

        var result = await service.GetAllCompetitionsAsync();

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task UpdateCompetition_ChangeName_InUpcomingStatus_Succeeds()
    {
        var service = CreateService();
        var created = await service.CreateCompetitionAsync(CreateRequest());

        var result = await service.UpdateCompetitionAsync(
            created.Id,
            new UpdateCompetitionRequest { Name = "Regional Cup" });

        Assert.Equal("Regional Cup", result.Name);
    }

    [Fact]
    public async Task UpdateCompetition_ChangeName_InActiveStatus_ThrowsException()
    {
        var service = CreateService();
        var created = await service.CreateCompetitionAsync(CreateRequest());
        await service.UpdateCompetitionAsync(
            created.Id,
            new UpdateCompetitionRequest { Status = CompetitionStatus.Active });

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.UpdateCompetitionAsync(
            created.Id,
            new UpdateCompetitionRequest { Name = "Regional Cup" }));
    }

    [Fact]
    public async Task UpdateCompetition_ChangeRules_InActiveStatus_Succeeds()
    {
        var service = CreateService();
        var created = await service.CreateCompetitionAsync(CreateRequest());
        await service.UpdateCompetitionAsync(
            created.Id,
            new UpdateCompetitionRequest { Status = CompetitionStatus.Active });

        var result = await service.UpdateCompetitionAsync(
            created.Id,
            new UpdateCompetitionRequest { Rules = "Updated rules" });

        Assert.Equal("Updated rules", result.Rules);
    }

    [Fact]
    public async Task UpdateCompetition_ChangeRules_InFinishedStatus_ThrowsException()
    {
        var service = CreateService();
        var created = await service.CreateCompetitionAsync(CreateRequest());
        await service.UpdateCompetitionAsync(
            created.Id,
            new UpdateCompetitionRequest { Status = CompetitionStatus.Active });
        await service.UpdateCompetitionAsync(
            created.Id,
            new UpdateCompetitionRequest { Status = CompetitionStatus.Finished });

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.UpdateCompetitionAsync(
            created.Id,
            new UpdateCompetitionRequest { Rules = "Updated rules" }));
    }

    [Fact]
    public async Task UpdateCompetition_ChangeEachCountIndependently_Succeeds()
    {
        var service = CreateService();
        var created = await service.CreateCompetitionAsync(CreateRequest());

        await service.UpdateCompetitionAsync(
            created.Id,
            new UpdateCompetitionRequest { AthleteCount = 8 });
        await service.UpdateCompetitionAsync(
            created.Id,
            new UpdateCompetitionRequest { ClubCount = 6 });
        var result = await service.UpdateCompetitionAsync(
            created.Id,
            new UpdateCompetitionRequest { RefereeCount = 5 });

        Assert.Equal(8, result.AthleteCount);
        Assert.Equal(6, result.ClubCount);
        Assert.Equal(5, result.RefereeCount);
    }

    [Theory]
    [InlineData(" ", null)]
    [InlineData(null, " ")]
    public async Task UpdateCompetition_WithBlankNameOrRules_ThrowsException(string? name, string? rules)
    {
        var service = CreateService();
        var created = await service.CreateCompetitionAsync(CreateRequest());

        await Assert.ThrowsAsync<ArgumentException>(() => service.UpdateCompetitionAsync(
            created.Id,
            new UpdateCompetitionRequest { Name = name, Rules = rules }));
    }

    [Fact]
    public async Task UpdateCompetition_WithNoChanges_ThrowsException()
    {
        var service = CreateService();
        var created = await service.CreateCompetitionAsync(CreateRequest());

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.UpdateCompetitionAsync(
            created.Id,
            new UpdateCompetitionRequest()));
    }

    [Fact]
    public async Task UpdateCompetition_TransitionStatusForward_Succeeds()
    {
        var service = CreateService();
        var created = await service.CreateCompetitionAsync(CreateRequest());

        var result = await service.UpdateCompetitionAsync(
            created.Id,
            new UpdateCompetitionRequest { Status = CompetitionStatus.Active });

        Assert.Equal(CompetitionStatus.Active, result.Status);
    }

    [Fact]
    public async Task UpdateCompetition_TransitionStatusBackward_ThrowsException()
    {
        var service = CreateService();
        var created = await service.CreateCompetitionAsync(CreateRequest());
        await service.UpdateCompetitionAsync(
            created.Id,
            new UpdateCompetitionRequest { Status = CompetitionStatus.Active });
        await service.UpdateCompetitionAsync(
            created.Id,
            new UpdateCompetitionRequest { Status = CompetitionStatus.Finished });

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.UpdateCompetitionAsync(
            created.Id,
            new UpdateCompetitionRequest { Status = CompetitionStatus.Active }));
    }

    [Fact]
    public async Task DeleteCompetition_WithValidId_RemovesSuccessfully()
    {
        var service = CreateService();
        var created = await service.CreateCompetitionAsync(CreateRequest());

        await service.DeleteCompetitionAsync(created.Id);

        await Assert.ThrowsAsync<NotFoundException>(() => service.GetCompetitionByIdAsync(created.Id));
    }

    [Fact]
    public async Task DeleteCompetition_WithInvalidId_ThrowsNotFoundException()
    {
        var service = CreateService();

        await Assert.ThrowsAsync<NotFoundException>(() => service.DeleteCompetitionAsync(Guid.NewGuid()));
    }

    private static CompetitionService CreateService()
    {
        return new CompetitionService(new CompetitionRepository());
    }

    private static CreateCompetitionRequest CreateRequest()
    {
        return new CreateCompetitionRequest
        {
            Name = "Spring Cup",
            Date = new DateTime(2026, 5, 14),
            Location = "Lisbon",
            AthleteCount = 4,
            ClubCount = 4,
            RefereeCount = 4,
            Rules = "Standard competition rules"
        };
    }
}