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
[Route("sports")]
[Produces("application/json")]
public class SportController : ControllerBase
{
    private readonly ISportLogic _logic;

    public SportController(ISportLogic logic)
    {
        _logic = logic;
    }

    [HttpGet]
    [SwaggerOperation(
        Summary = "Gets sports from database")]
    public async Task<IActionResult> GetSportsAsync([FromQuery] GetSportQuery query)
    {
        var filters = new SportQuery()
        {
            Ids = query.Ids,
            Name = query.Name,
            StartRange = query.StartRange,
            EndRange = query.EndRange,
            Count = query.Count,
            Descending = query.Descending,
            OrderBy = query.OrderBy
        };

        var data = await _logic.GetSportsAsync(filters);
        return Ok(new JSendResponse(data));
    }

    [HttpGet("{id:int}")]
    [SwaggerOperation(
        Summary = "Gets existing sport from database")]
    public async Task<IActionResult> GetSportByIdAsync([FromRoute] int id)
    {
        try
        {
            var data = await _logic.GetSportByIdAsync(id);
            return Ok(new JSendResponse(data));
        }
        catch (EntityNotFoundException ex)
        {
            Log.Error(ex, "Could not find sport with 'Id={SportId}' in database. See inner exception for details.", id);

            return NotFound(new JSendResponse
            {
                Status = JSendStatus.Fail,
                Message = ex.Message
            });
        }
    }

    [HttpPost]
    [SwaggerOperation(
        Summary = "Inserts a new sport into the database")]
    public async Task<IActionResult> AddSportAsync([FromBody] SportRequest request)
    {
        try
        {
            var sport = Map(request);
            var data = await _logic.AddSportAsync(sport);

            return Ok(new JSendResponse(data));
        }
        catch (ArgumentNullException ex)
        {
            Log.Warning(ex, "Sport cannot be null. See exception for details.");

            return BadRequest(new JSendResponse
            {
                Status = JSendStatus.Fail,
                Message = ex.Message
            });
        }
    }

    [HttpPut("{id:int}")]
    [SwaggerOperation(
        Summary = "Updates an existing sport in the database")]
    public async Task<IActionResult> UpdateSportAsync([FromRoute] int id, [FromBody] SportRequest request)
    {
        try
        {
            var sport = Map(request, id);
            var data = await _logic.UpdateSportAsync(sport);

            return Ok(new JSendResponse(data));
        }
        catch (ArgumentNullException ex)
        {
            Log.Warning(ex, "Sport cannot be null. See exception for details.");

            return BadRequest(new JSendResponse
            {
                Status = JSendStatus.Fail,
                Message = ex.Message
            });
        }
    }

    [HttpDelete("{id:int}")]
    [SwaggerOperation(
        Summary = "Deletes an existing sport in the database")]
    public async Task<IActionResult> DeleteSportAsync([FromRoute] int id)
    {
        try
        {
            await _logic.DeleteSportAsync(id);
            return Ok(new JSendResponse());
        }
        catch (EntityNotFoundException ex)
        {
            Log.Warning(ex, "Cannot delete a sport that does not exist in the database. See exception for details.");

            return BadRequest(new JSendResponse
            {
                Status = JSendStatus.Fail,
                Message = ex.Message
            });
        }
    }

    private static Sport Map(SportRequest request, int id = 0)
    {
        return new Sport
        {
            Id = id,
            Name = request.Name,
            Description = request.Description,
            MaxTeamSize = request.MaxTeamSize,
            StartDate = request.StartDate,
            EndDate = request.EndDate
        };
    }
}