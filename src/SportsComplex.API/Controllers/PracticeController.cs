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
[Route("practices")]
[Produces("application/json")]
public class PracticeController : ControllerBase
{
    private readonly IPracticeLogic _logic;

    public PracticeController(IPracticeLogic logic)
    {
        _logic = logic;
    }

    [HttpGet]
    [SwaggerOperation(
        Summary = "Gets practices from database")]
    public async Task<IActionResult> GetPracticesAsync([FromQuery] GetPracticeQuery query)
    {
        var filters = new PracticeQuery()
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

        var data = await _logic.GetPracticesAsync(filters);
        return Ok(new JSendResponse(data));
    }

    [HttpGet("{id:int}")]
    [SwaggerOperation(
        Summary = "Gets existing practice from database")]
    public async Task<IActionResult> GetPracticeByIdAsync([FromRoute] int id)
    {
        try
        {
            var data = await _logic.GetPracticeById(id);
            return Ok(new JSendResponse(data));
        }
        catch (EntityNotFoundException ex)
        {
            Log.Error(ex, "Could not find practice with 'Id={PracticeId}' in database. See inner exception for details.", id);

            return NotFound(new JSendResponse
            {
                Status = JSendStatus.Fail,
                Message = ex.Message
            });
        }
    }

    [HttpPost]
    [SwaggerOperation(
        Summary = "Inserts a new practice into the database")]
    public async Task<IActionResult> AddPracticeAsync([FromBody] PracticeRequest request)
    {
        try
        {
            var practice = Map(request);
            var data = await _logic.AddPracticeAsync(practice);

            return Ok(new JSendResponse(data));
        }
        catch (ArgumentNullException ex)
        {
            Log.Warning(ex, "Practice cannot be null. See exception for details.");

            return BadRequest(new JSendResponse
            {
                Status = JSendStatus.Fail,
                Message = ex.Message
            });
        }
    }

    [HttpPut("{id:int}")]
    [SwaggerOperation(
        Summary = "Updates an existing practice in the database")]
    public async Task<IActionResult> UpdatePracticeAsync([FromRoute] int id, [FromBody] PracticeRequest request)
    {
        try
        {
            var practice = Map(request, id);
            var data = await _logic.UpdatePracticeAsync(practice);

            return Ok(new JSendResponse(data));
        }
        catch (ArgumentNullException ex)
        {
            Log.Warning(ex, "Practice cannot be null. See exception for details.");

            return BadRequest(new JSendResponse
            {
                Status = JSendStatus.Fail,
                Message = ex.Message
            });
        }
    }

    [HttpDelete("{id:int}")]
    [SwaggerOperation(
        Summary = "Deletes an existing practice in the database")]
    public async Task<IActionResult> DeletePracticeAsync([FromRoute] int id)
    {
        try
        {
            await _logic.DeletePracticeAsync(id);
            return Ok(new JSendResponse());
        }
        catch (EntityNotFoundException ex)
        {
            Log.Warning(ex, "Cannot delete a practice that does not exist in the database. See exception for details.");

            return BadRequest(new JSendResponse
            {
                Status = JSendStatus.Fail,
                Message = ex.Message
            });
        }
    }

    private static Practice Map(PracticeRequest request, int id = 0)
    {
        return new Practice
        {
            Id = id,
            TeamId = request.TeamId,
            LocationId = request.LocationId,
            StartDateTime = request.StartDateTime,
            EndDateTime = request.EndDateTime
        };
    }
}