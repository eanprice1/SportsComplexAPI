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
[Route("coaches")]
[Produces("application/json")]
public class CoachController : ControllerBase
{
    private readonly ICoachLogic _logic;

    public CoachController(ICoachLogic logic)
    {
        _logic = logic;
    }

    [HttpGet]
    [SwaggerOperation(
        Summary = "Gets coaches from database")]
    public async Task<IActionResult> GetCoachesAsync([FromQuery] GetCoachQuery query)
    {
        var filters = new CoachQuery()
        {
            Ids = query.Ids,
            TeamIds = query.TeamIds,
            FirstName = query.FirstName,
            LastName = query.LastName,
            IsHeadCoach = query.IsHeadCoach,
            OnlyUnassignedCoaches = query.OnlyUnassignedCoaches,
            Count = query.Count,
            Descending = query.Descending,
            OrderBy = query.OrderBy
        };

        var data = await _logic.GetCoachesAsync(filters);
        return Ok(new JSendResponse(data));
    }

    [HttpGet("{id:int}")]
    [SwaggerOperation(
        Summary = "Gets existing coach from database")]
    public async Task<IActionResult> GetCoachByIdAsync([FromRoute] int id)
    {
        try
        {
            var data = await _logic.GetCoachById(id);
            return Ok(new JSendResponse(data));
        }
        catch (EntityNotFoundException ex)
        {
            Log.Error(ex, "Could not find coach with 'Id={CoachId}' in database. See inner exception for details.", id);

            return NotFound(new JSendResponse
            {
                Status = JSendStatus.Fail,
                Message = ex.Message
            });
        }
    }

    [HttpPost]
    [SwaggerOperation(
        Summary = "Inserts a new coach into the database")]
    public async Task<IActionResult> AddCoachAsync([FromBody] CoachRequest request)
    {
        try
        {
            var coach = Map(request);
            var data = await _logic.AddCoachAsync(coach);

            return Ok(new JSendResponse(data));
        }
        catch (ArgumentNullException ex)
        {
            Log.Warning(ex, "Coach cannot be null. See exception for details.");

            return BadRequest(new JSendResponse
            {
                Status = JSendStatus.Fail,
                Message = ex.Message
            });
        }
    }

    [HttpPut("{id:int}")]
    [SwaggerOperation(
        Summary = "Updates an existing coach in the database")]
    public async Task<IActionResult> UpdateCoachAsync([FromRoute] int id, [FromBody] CoachRequest request)
    {
        try
        {
            var coach = Map(request, id);
            var data = await _logic.UpdateCoachAsync(coach);

            return Ok(new JSendResponse(data));
        }
        catch (ArgumentNullException ex)
        {
            Log.Warning(ex, "Coach cannot be null. See exception for details.");

            return BadRequest(new JSendResponse
            {
                Status = JSendStatus.Fail,
                Message = ex.Message
            });
        }
    }

    [HttpDelete("{id:int}")]
    [SwaggerOperation(
        Summary = "Deletes an existing coach in the database")]
    public async Task<IActionResult> DeleteCoachAsync([FromRoute] int id)
    {
        try
        {
            await _logic.DeleteCoachAsync(id);
            return Ok(new JSendResponse());
        }
        catch (EntityNotFoundException ex)
        {
            Log.Warning(ex, "Cannot delete a coach that does not exist in the database. See exception for details.");

            return BadRequest(new JSendResponse
            {
                Status = JSendStatus.Fail,
                Message = ex.Message
            });
        }
    }

    private static Coach Map(CoachRequest request, int id = 0)
    {
        return new Coach
        {
            Id = id,
            TeamId = request.TeamId,
            FirstName = request.FirstName,
            LastName = request.LastName,
            BirthDate = request.BirthDate,
            PhoneNumber = request.PhoneNumber,
            Email = request.Email,
            Address = request.Address,
            IsHeadCoach = request.IsHeadCoach
        };
    }
}