using Microsoft.AspNetCore.Mvc;
using Serilog;
using SportsComplex.API.Api.JSend;
using SportsComplex.API.Api.Requests;
using SportsComplex.Logic.Exceptions;
using SportsComplex.Logic.Models;
using SportsComplex.Logic.Repositories;
using Swashbuckle.AspNetCore.Annotations;

namespace SportsComplex.API.Controllers;

[ApiController]
[Route("matches")]
[Produces("application/json")]
public class MatchController : ControllerBase
{
    private readonly IMatchLogic _logic;

    public MatchController(IMatchLogic logic)
    {
        _logic = logic;
    }

    [HttpGet]
    [SwaggerOperation(
        Summary = "Gets matches from database")]
    public async Task<IActionResult> GetMatchesAsync([FromQuery] GetMatchQuery query)
    {
        var filters = new MatchQuery()
        {
            Ids = query.Ids,
            TeamIds = query.TeamIds,
            LocationIds = query.LocationIds,
            StartRange = query.StartRange,
            EndRange = query.EndRange,
            Count = query.Count,
            OrderBy = query.OrderBy,
            Descending = query.Descending
        };

        var data = await _logic.GetMatchesAsync(filters);
        return Ok(new JSendResponse(data));
    }

    [HttpGet("{id:int}")]
    [SwaggerOperation(
        Summary = "Gets existing match from database")]
    public async Task<IActionResult> GetMatchByIdAsync([FromRoute] int id)
    {
        try
        {
            var data = await _logic.GetMatchById(id);
            return Ok(new JSendResponse(data));
        }
        catch (EntityNotFoundException ex)
        {
            Log.Error(ex, "Could not find match with 'Id={MatchId}' in database. See inner exception for details.", id);

            return NotFound(new JSendResponse
            {
                Status = JSendStatus.Fail,
                Message = ex.Message
            });
        }
    }

    [HttpPost]
    [SwaggerOperation(
        Summary = "Inserts a new match into the database")]
    public async Task<IActionResult> AddMatchAsync([FromBody] MatchRequest request)
    {
        try
        {
            var match = Map(request);
            var data = await _logic.AddMatchAsync(match);

            return Ok(new JSendResponse(data));
        }
        catch (ArgumentNullException ex)
        {
            Log.Warning(ex, "Match cannot be null. See exception for details.");

            return BadRequest(new JSendResponse
            {
                Status = JSendStatus.Fail,
                Message = ex.Message
            });
        }
    }

    [HttpPut("{id:int}")]
    [SwaggerOperation(
        Summary = "Updates an existing match in the database")]
    public async Task<IActionResult> UpdateMatchAsync([FromRoute] int id, [FromBody] MatchRequest request)
    {
        try
        {
            var match = Map(request, id);
            var data = await _logic.UpdateMatchAsync(match);

            return Ok(new JSendResponse(data));
        }
        catch (ArgumentNullException ex)
        {
            Log.Warning(ex, "Match cannot be null. See exception for details.");

            return BadRequest(new JSendResponse
            {
                Status = JSendStatus.Fail,
                Message = ex.Message
            });
        }
    }

    [HttpDelete("{id:int}")]
    [SwaggerOperation(
        Summary = "Deletes an existing match in the database")]
    public async Task<IActionResult> DeleteMatchAsync([FromRoute] int id)
    {
        try
        {
            await _logic.DeleteMatchAsync(id);
            return Ok(new JSendResponse());
        }
        catch (EntityNotFoundException ex)
        {
            Log.Warning(ex, "Cannot delete a match that does not exist in the database. See exception for details.");

            return BadRequest(new JSendResponse
            {
                Status = JSendStatus.Fail,
                Message = ex.Message
            });
        }
    }

    private static Match Map(MatchRequest request, int id = 0)
    {
        return new Match
        {
            Id = id,
            HomeTeamId = request.HomeTeamId,
            AwayTeamId = request.AwayTeamId,
            LocationId = request.LocationId,
            StartDateTime = request.StartDateTime,
            EndDateTime = request.EndDateTime
        };
    }
}