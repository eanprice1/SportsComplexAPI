using Microsoft.AspNetCore.Mvc;
using Serilog;
using SportsComplex.API.Api.JSend;
using SportsComplex.API.Api.Requests;
using SportsComplex.Logic.Exceptions;
using SportsComplex.Logic.Interfaces;
using SportsComplex.Logic.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace SportsComplex.API.Controllers;

[ApiController]
[Route("teams")]
[Produces("application/json")]
public class TeamController : ControllerBase
{
    private readonly ITeamLogic _logic;

    public TeamController(ITeamLogic logic)
    {
        _logic = logic;
    }

    [HttpGet]
    [SwaggerOperation(
        Summary = "Gets teams from database")]
    public async Task<IActionResult> GetTeamsAync([FromQuery] GetTeamQuery query)
    {
        var filters = new TeamQuery
        {
            Ids = query.Ids,
            SportIds = query.SportIds,
            Name = query.Name,
            Count = query.Count,
            Descending = query.Descending,
            OrderBy = query.OrderBy
        };

        var data = await _logic.GetTeamsAsync(filters);
        return Ok(new JSendResponse(data));
    }

    [HttpGet("{id:int}")]
    [SwaggerOperation(
        Summary = "Gets existing team from database")]
    public async Task<IActionResult> GetTeamByIdAsync([FromRoute] int id)
    {
        try
        {
            var data = await _logic.GetTeamByIdAsync(id);
            return Ok(new JSendResponse(data));
        }
        catch (EntityNotFoundException ex)
        {
            Log.Error(ex, "Could not find team with 'Id={TeamId}' in database. See inner exception for details.", id);

            return NotFound(new JSendResponse
            {
                Status = JSendStatus.Fail,
                Message = ex.Message
            });
        }
    }

    [HttpPost]
    [SwaggerOperation(
        Summary = "Inserts a new team into the database")]
    public async Task<IActionResult> AddPlayerAsync([FromBody] TeamRequest request)
    {
        try
        {
            var team = Map(request);
            var data = await _logic.AddTeamAsync(team);

            return Ok(new JSendResponse(data));
        }
        catch (ArgumentNullException ex)
        {
            Log.Warning(ex, "Team cannot be null. See exception for details.");

            return BadRequest(new JSendResponse
            {
                Status = JSendStatus.Fail,
                Message = ex.Message
            });
        }
    }

    [HttpPut("{id:int}")]
    [SwaggerOperation(
        Summary = "Updates an existing team in the database")]
    public async Task<IActionResult> UpdatePlayerAsync([FromRoute] int id, [FromBody] TeamRequest request)
    {
        try
        {
            var team = Map(request, id);
            var data = await _logic.UpdateTeamAsync(team);

            return Ok(new JSendResponse(data));
        }
        catch (ArgumentNullException ex)
        {
            Log.Warning(ex, "Team cannot be null. See exception for details.");

            return BadRequest(new JSendResponse
            {
                Status = JSendStatus.Fail,
                Message = ex.Message
            });
        }
    }

    [HttpDelete("{id:int}")]
    [SwaggerOperation(
        Summary = "Deletes an existing team in the database")]
    public async Task<IActionResult> DeleteTeamAsync([FromRoute] int id)
    {
        try
        {
            await _logic.DeleteTeamAsync(id);
            return Ok(new JSendResponse());
        }
        catch (EntityNotFoundException ex)
        {
            Log.Warning(ex, "Cannot delete a team that does not exist in the database. See exception for details.");

            return BadRequest(new JSendResponse
            {
                Status = JSendStatus.Fail,
                Message = ex.Message
            });
        }
    }

    private static Team Map(TeamRequest request, int id = 0)
    {
        return new Team
        {
            Id = id,
            SportId = request.SportId,
            Name = request.Name,
            Motto = request.Motto
        };
    }
}