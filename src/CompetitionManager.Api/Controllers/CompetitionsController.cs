using CompetitionManager.Application.DTOs;
using CompetitionManager.Application.Exceptions;
using CompetitionManager.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace CompetitionManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class CompetitionsController : ControllerBase
{
    private readonly ICompetitionService competitionService;

    public CompetitionsController(ICompetitionService competitionService)
    {
        this.competitionService = competitionService ?? throw new ArgumentNullException(nameof(competitionService));
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CompetitionResponse>>> GetAllCompetitions()
    {
        var competitions = await competitionService.GetAllCompetitionsAsync();
        return Ok(competitions);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CompetitionResponse>> GetCompetitionById(Guid id)
    {
        try
        {
            var competition = await competitionService.GetCompetitionByIdAsync(id);
            return Ok(competition);
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost]
    public async Task<ActionResult<CompetitionResponse>> CreateCompetition(CreateCompetitionRequest request)
    {
        try
        {
            var response = await competitionService.CreateCompetitionAsync(request);
            return CreatedAtAction(nameof(GetCompetitionById), new { id = response.Id }, response);
        }
        catch (ArgumentException)
        {
            return BadRequest();
        }
        catch (InvalidOperationException)
        {
            return BadRequest();
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<CompetitionResponse>> UpdateCompetition(Guid id, UpdateCompetitionRequest request)
    {
        try
        {
            var response = await competitionService.UpdateCompetitionAsync(id, request);
            return Ok(response);
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
        catch (ArgumentException)
        {
            return BadRequest();
        }
        catch (InvalidOperationException)
        {
            return BadRequest();
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteCompetition(Guid id)
    {
        try
        {
            await competitionService.DeleteCompetitionAsync(id);
            return NoContent();
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }
}
