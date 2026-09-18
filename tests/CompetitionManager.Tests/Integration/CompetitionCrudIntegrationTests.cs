using System.Net;
using System.Net.Http.Json;
using CompetitionManager.Application.DTOs;
using CompetitionManager.Domain.ValueObjects;
using Microsoft.AspNetCore.Mvc.Testing;

namespace CompetitionManager.Tests.Integration;

public sealed class CompetitionCrudIntegrationTests
{
    [Fact]
    public async Task CreateAndRetrieve_HappyPath_Succeeds()
    {
        using var client = CreateClient();

        var createResponse = await client.PostAsJsonAsync("/api/competitions", CreateRequest());
        var created = await ReadResponseAsync(createResponse);

        var getResponse = await client.GetAsync($"/api/competitions/{created.Id}");
        var retrieved = await ReadResponseAsync(getResponse);

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        Assert.Equal(created.Id, retrieved.Id);
        Assert.Equal("Spring Cup", retrieved.Name);
        Assert.Equal(CompetitionStatus.Upcoming, retrieved.Status);
    }

    [Fact]
    public async Task CreateUpdateDelete_Workflow_Succeeds()
    {
        using var client = CreateClient();

        var created = await CreateCompetitionAsync(client);
        var updateResponse = await client.PutAsJsonAsync(
            $"/api/competitions/{created.Id}",
            new UpdateCompetitionRequest
            {
                Name = "Regional Cup",
                Rules = "Updated rules",
                AthleteCount = 8,
                ClubCount = 6,
                RefereeCount = 5
            });
        var updated = await ReadResponseAsync(updateResponse);

        var deleteResponse = await client.DeleteAsync($"/api/competitions/{created.Id}");
        var getResponse = await client.GetAsync($"/api/competitions/{created.Id}");

        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
        Assert.Equal("Regional Cup", updated.Name);
        Assert.Equal("Updated rules", updated.Rules);
        Assert.Equal(8, updated.AthleteCount);
        Assert.Equal(6, updated.ClubCount);
        Assert.Equal(5, updated.RefereeCount);
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task ActivateAndFinish_StatusTransitions_Succeed()
    {
        using var client = CreateClient();
        var created = await CreateCompetitionAsync(client);

        var activateResponse = await client.PutAsJsonAsync(
            $"/api/competitions/{created.Id}",
            new UpdateCompetitionRequest { Status = CompetitionStatus.Active });
        var finishResponse = await client.PutAsJsonAsync(
            $"/api/competitions/{created.Id}",
            new UpdateCompetitionRequest { Status = CompetitionStatus.Finished });
        var finished = await ReadResponseAsync(finishResponse);

        Assert.Equal(HttpStatusCode.OK, activateResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, finishResponse.StatusCode);
        Assert.Equal(CompetitionStatus.Finished, finished.Status);
    }

    [Fact]
    public async Task UpdateNameInUpcoming_Succeeds_UpdateNameInActive_Fails()
    {
        using var client = CreateClient();
        var created = await CreateCompetitionAsync(client);

        var upcomingResponse = await client.PutAsJsonAsync(
            $"/api/competitions/{created.Id}",
            new UpdateCompetitionRequest { Name = "Updated Cup" });
        await client.PutAsJsonAsync(
            $"/api/competitions/{created.Id}",
            new UpdateCompetitionRequest { Status = CompetitionStatus.Active });
        var activeResponse = await client.PutAsJsonAsync(
            $"/api/competitions/{created.Id}",
            new UpdateCompetitionRequest { Name = "Another Cup" });

        Assert.Equal(HttpStatusCode.OK, upcomingResponse.StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, activeResponse.StatusCode);
    }

    [Fact]
    public async Task UpdateRulesInUpcomingAndActive_SucceedsInFinished_Fails()
    {
        using var client = CreateClient();
        var created = await CreateCompetitionAsync(client);

        var upcomingResponse = await UpdateRulesAsync(client, created.Id, "Upcoming rules");
        await TransitionStatusAsync(client, created.Id, CompetitionStatus.Active);
        var activeResponse = await UpdateRulesAsync(client, created.Id, "Active rules");
        await TransitionStatusAsync(client, created.Id, CompetitionStatus.Finished);
        var finishedResponse = await UpdateRulesAsync(client, created.Id, "Finished rules");

        Assert.Equal(HttpStatusCode.OK, upcomingResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, activeResponse.StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, finishedResponse.StatusCode);
    }

    [Fact]
    public async Task CreateWithDuplicateName_Fails()
    {
        using var client = CreateClient();
        await CreateCompetitionAsync(client);

        var duplicateResponse = await client.PostAsJsonAsync("/api/competitions", CreateRequest());

        Assert.Equal(HttpStatusCode.BadRequest, duplicateResponse.StatusCode);
    }

    [Fact]
    public async Task CreateWithInvalidCounts_Fails()
    {
        using var client = CreateClient();
        var request = CreateRequest() with { AthleteCount = 3 };

        var response = await client.PostAsJsonAsync("/api/competitions", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task TransitionStatusBackward_Fails()
    {
        using var client = CreateClient();
        var created = await CreateCompetitionAsync(client);
        await TransitionStatusAsync(client, created.Id, CompetitionStatus.Active);
        await TransitionStatusAsync(client, created.Id, CompetitionStatus.Finished);

        var response = await client.PutAsJsonAsync(
            $"/api/competitions/{created.Id}",
            new UpdateCompetitionRequest { Status = CompetitionStatus.Active });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task ListAllCompetitions_Returns_All()
    {
        using var client = CreateClient();
        await CreateCompetitionAsync(client, "Spring Cup");
        await CreateCompetitionAsync(client, "Summer Cup");

        var response = await client.GetAsync("/api/competitions");
        var competitions = await response.Content.ReadFromJsonAsync<List<CompetitionResponse>>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(competitions);
        Assert.Equal(2, competitions.Count);
    }

    [Fact]
    public async Task DeleteInAnyStatus_Succeeds()
    {
        using var client = CreateClient();
        var created = await CreateCompetitionAsync(client);
        await TransitionStatusAsync(client, created.Id, CompetitionStatus.Active);

        var response = await client.DeleteAsync($"/api/competitions/{created.Id}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task CreateWithCategories_CategoriesRoundTrip()
    {
        using var client = CreateClient();
        var request = CreateRequest() with
        {
            WeightCategories = [new CategoryDefinition("Lightweight", "Under 60kg")],
            AgeCategories = [new CategoryDefinition("Junior")]
        };

        var response = await client.PostAsJsonAsync("/api/competitions", request);
        var created = await ReadResponseAsync(response);

        Assert.Single(created.WeightCategories);
        Assert.Equal("Lightweight", created.WeightCategories[0].Name);
        Assert.Single(created.AgeCategories);
        Assert.Equal("Junior", created.AgeCategories[0].Name);
    }

    [Fact]
    public async Task CreateWithUnlimitedCounts_Succeeds()
    {
        using var client = CreateClient();
        var request = CreateRequest() with { AthleteCount = 1000, ClubCount = 500, RefereeCount = 100 };

        var response = await client.PostAsJsonAsync("/api/competitions", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    private static HttpClient CreateClient()
    {
        var factory = new WebApplicationFactory<Program>();
        return factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
    }

    private static async Task<CompetitionResponse> CreateCompetitionAsync(HttpClient client, string? name = null)
    {
        var response = await client.PostAsJsonAsync("/api/competitions", CreateRequest(name));
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        return await ReadResponseAsync(response);
    }

    private static async Task<HttpResponseMessage> UpdateRulesAsync(HttpClient client, Guid id, string rules)
    {
        return await client.PutAsJsonAsync(
            $"/api/competitions/{id}",
            new UpdateCompetitionRequest { Rules = rules });
    }

    private static async Task TransitionStatusAsync(HttpClient client, Guid id, CompetitionStatus status)
    {
        var response = await client.PutAsJsonAsync(
            $"/api/competitions/{id}",
            new UpdateCompetitionRequest { Status = status });
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    private static async Task<CompetitionResponse> ReadResponseAsync(HttpResponseMessage response)
    {
        return await response.Content.ReadFromJsonAsync<CompetitionResponse>()
            ?? throw new InvalidOperationException("Expected a competition response.");
    }

    private static CreateCompetitionRequest CreateRequest(string? name = null)
    {
        return new CreateCompetitionRequest
        {
            Name = name ?? "Spring Cup",
            Date = new DateTime(2026, 5, 14),
            Location = "Lisbon",
            AthleteCount = 4,
            ClubCount = 4,
            RefereeCount = 4,
            Rules = "Standard competition rules"
        };
    }
}