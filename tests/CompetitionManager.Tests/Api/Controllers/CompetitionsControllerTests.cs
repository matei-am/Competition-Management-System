using CompetitionManager.Api.Controllers;
using CompetitionManager.Application.DTOs;
using CompetitionManager.Application.Exceptions;
using CompetitionManager.Application.Services;
using CompetitionManager.Domain.ValueObjects;
using Microsoft.AspNetCore.Mvc;

namespace CompetitionManager.Tests.Api.Controllers;

public class CompetitionsControllerTests
{
    [Fact]
    public async Task GetAllCompetitions_ReturnsOkWithList()
    {
        // Arrange
        var expected = new List<CompetitionResponse>
        {
            CreateResponse(Guid.NewGuid(), "Spring Open"),
            CreateResponse(Guid.NewGuid(), "Summer Clash")
        };
        var service = new FakeCompetitionService
        {
            GetAllCompetitionsAsyncResult = expected
        };
        var controller = new CompetitionsController(service);

        // Act
        var result = await controller.GetAllCompetitions();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var actual = Assert.IsAssignableFrom<IEnumerable<CompetitionResponse>>(okResult.Value);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task GetCompetitionById_WithValidId_ReturnsOk()
    {
        // Arrange
        var expected = CreateResponse(Guid.NewGuid(), "Spring Open");
        var service = new FakeCompetitionService
        {
            GetCompetitionByIdAsyncResult = expected
        };
        var controller = new CompetitionsController(service);

        // Act
        var result = await controller.GetCompetitionById(expected.Id);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var actual = Assert.IsType<CompetitionResponse>(okResult.Value);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task GetCompetitionById_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var service = new FakeCompetitionService
        {
            GetCompetitionByIdAsyncException = new NotFoundException("Missing")
        };
        var controller = new CompetitionsController(service);

        // Act
        var result = await controller.GetCompetitionById(Guid.NewGuid());

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task CreateCompetition_WithValidRequest_ReturnsCreatedWithLocation()
    {
        // Arrange
        var expected = CreateResponse(Guid.NewGuid(), "Spring Open");
        var service = new FakeCompetitionService
        {
            CreateCompetitionAsyncResult = expected
        };
        var controller = new CompetitionsController(service);
        var request = new CreateCompetitionRequest
        {
            Name = "Spring Open",
            Date = DateTime.UtcNow,
            Location = "Boston",
            AthleteCount = 4,
            ClubCount = 2,
            RefereeCount = 1,
            Rules = "Standard rules"
        };

        // Act
        var result = await controller.CreateCompetition(request);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(nameof(CompetitionsController.GetCompetitionById), createdResult.ActionName);
        Assert.Equal(expected.Id, createdResult.RouteValues!["id"]);
        Assert.Equal(expected, createdResult.Value);
    }

    [Fact]
    public async Task CreateCompetition_WhenServiceRejectsRequest_ReturnsBadRequest()
    {
        // Arrange
        var service = new FakeCompetitionService
        {
            CreateCompetitionAsyncException = new ArgumentException("Bad request")
        };
        var controller = new CompetitionsController(service);
        var request = new CreateCompetitionRequest
        {
            Name = "Spring Open",
            Date = DateTime.UtcNow,
            Location = "Boston",
            AthleteCount = 4,
            ClubCount = 2,
            RefereeCount = 1,
            Rules = "Standard rules"
        };

        // Act
        var result = await controller.CreateCompetition(request);

        // Assert
        Assert.IsType<BadRequestResult>(result.Result);
    }

    [Fact]
    public async Task UpdateCompetition_WithValidRequest_ReturnsOk()
    {
        // Arrange
        var expected = CreateResponse(Guid.NewGuid(), "Updated Open");
        var service = new FakeCompetitionService
        {
            UpdateCompetitionAsyncResult = expected
        };
        var controller = new CompetitionsController(service);
        var request = new UpdateCompetitionRequest { Name = "Updated Open" };

        // Act
        var result = await controller.UpdateCompetition(expected.Id, request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(expected, okResult.Value);
    }

    [Fact]
    public async Task UpdateCompetition_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var service = new FakeCompetitionService
        {
            UpdateCompetitionAsyncException = new NotFoundException("Missing")
        };
        var controller = new CompetitionsController(service);

        // Act
        var result = await controller.UpdateCompetition(Guid.NewGuid(), new UpdateCompetitionRequest { Name = "Test" });

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task UpdateCompetition_WhenServiceRejectsRequest_ReturnsBadRequest()
    {
        // Arrange
        var service = new FakeCompetitionService
        {
            UpdateCompetitionAsyncException = new InvalidOperationException("Invalid")
        };
        var controller = new CompetitionsController(service);

        // Act
        var result = await controller.UpdateCompetition(Guid.NewGuid(), new UpdateCompetitionRequest { Name = "Test" });

        // Assert
        Assert.IsType<BadRequestResult>(result.Result);
    }

    [Fact]
    public async Task DeleteCompetition_WithValidId_ReturnsNoContent()
    {
        // Arrange
        var service = new FakeCompetitionService();
        var controller = new CompetitionsController(service);

        // Act
        var result = await controller.DeleteCompetition(Guid.NewGuid());

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeleteCompetition_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var service = new FakeCompetitionService
        {
            DeleteCompetitionAsyncException = new NotFoundException("Missing")
        };
        var controller = new CompetitionsController(service);

        // Act
        var result = await controller.DeleteCompetition(Guid.NewGuid());

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    private static CompetitionResponse CreateResponse(Guid id, string name)
    {
        return new CompetitionResponse
        {
            Id = id,
            Name = name,
            Date = DateTime.UtcNow,
            Location = "Boston",
            Status = CompetitionStatus.Upcoming,
            AthleteCount = 4,
            ClubCount = 2,
            RefereeCount = 1,
            Rules = "Standard rules",
            WeightCategories = [],
            AgeCategories = [],
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    private sealed class FakeCompetitionService : ICompetitionService
    {
        public IReadOnlyList<CompetitionResponse> GetAllCompetitionsAsyncResult { get; set; } = [];

        public CompetitionResponse? GetCompetitionByIdAsyncResult { get; set; }

        public Exception? GetCompetitionByIdAsyncException { get; set; }

        public CompetitionResponse? CreateCompetitionAsyncResult { get; set; }

        public Exception? CreateCompetitionAsyncException { get; set; }

        public CompetitionResponse? UpdateCompetitionAsyncResult { get; set; }

        public Exception? UpdateCompetitionAsyncException { get; set; }

        public Exception? DeleteCompetitionAsyncException { get; set; }

        public Task<IReadOnlyList<CompetitionResponse>> GetAllCompetitionsAsync()
        {
            return Task.FromResult(GetAllCompetitionsAsyncResult);
        }

        public Task<CompetitionResponse> GetCompetitionByIdAsync(Guid id)
        {
            if (GetCompetitionByIdAsyncException is not null)
            {
                throw GetCompetitionByIdAsyncException;
            }

            return Task.FromResult(GetCompetitionByIdAsyncResult ?? throw new InvalidOperationException("No result configured."));
        }

        public Task<CompetitionResponse> CreateCompetitionAsync(CreateCompetitionRequest request)
        {
            if (CreateCompetitionAsyncException is not null)
            {
                throw CreateCompetitionAsyncException;
            }

            return Task.FromResult(CreateCompetitionAsyncResult ?? throw new InvalidOperationException("No result configured."));
        }

        public Task<CompetitionResponse> UpdateCompetitionAsync(Guid id, UpdateCompetitionRequest request)
        {
            if (UpdateCompetitionAsyncException is not null)
            {
                throw UpdateCompetitionAsyncException;
            }

            return Task.FromResult(UpdateCompetitionAsyncResult ?? throw new InvalidOperationException("No result configured."));
        }

        public Task DeleteCompetitionAsync(Guid id)
        {
            if (DeleteCompetitionAsyncException is not null)
            {
                throw DeleteCompetitionAsyncException;
            }

            return Task.CompletedTask;
        }
    }
}
