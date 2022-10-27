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
[Route("locations")]
[Produces("application/json")]
public class LocationController : ControllerBase
{
    private readonly ILocationLogic _logic;

    public LocationController(ILocationLogic logic)
    {
        _logic = logic;
    }

    [HttpGet]
    [SwaggerOperation(
        Summary = "Gets locations from database")]
    public async Task<IActionResult> GetLocationsAsync([FromQuery] GetLocationQuery query)
    {
        var filters = new LocationQuery
        {
            Ids = query.Ids,
            SportIds = query.SportIds,
            Name = query.Name,
            Address = query.Address,
            Count = query.Count,
            Descending = query.Descending,
            OrderBy = query.OrderBy
        };

        var data = await _logic.GetLocationsAsync(filters);
        return Ok(new JSendResponse(data));
    }

    [HttpGet("{id:int}")]
    [SwaggerOperation(
        Summary = "Gets existing location from database")]
    public async Task<IActionResult> GetLocationByIdAsync([FromRoute] int id)
    {
        try
        {
            var data = await _logic.GetLocationByIdAsync(id);
            return Ok(new JSendResponse(data));
        }
        catch (EntityNotFoundException ex)
        {
            Log.Error(ex, "Could not find location with 'Id={LocationId}' in database. See inner exception for details.", id);

            return NotFound(new JSendResponse
            {
                Status = JSendStatus.Fail,
                Message = ex.Message
            });
        }
    }

    [HttpPost]
    [SwaggerOperation(
        Summary = "Inserts a new location into the database")]
    public async Task<IActionResult> AddLocationAsync([FromBody] LocationRequest request)
    {
        try
        {
            var location = Map(request);
            var data = await _logic.AddLocationAsync(location);

            return Ok(new JSendResponse(data));
        }
        catch (ArgumentNullException ex)
        {
            Log.Warning(ex, "Location cannot be null. See exception for details.");

            return BadRequest(new JSendResponse
            {
                Status = JSendStatus.Fail,
                Message = ex.Message
            });
        }
    }

    [HttpPut("{id:int}")]
    [SwaggerOperation(
        Summary = "Updates an existing location in the database")]
    public async Task<IActionResult> UpdateLocationAsync([FromRoute] int id, [FromBody] LocationRequest request)
    {
        try
        {
            var location = Map(request, id);
            var data = await _logic.UpdateLocationAsync(location);

            return Ok(new JSendResponse(data));
        }
        catch (ArgumentNullException ex)
        {
            Log.Warning(ex, "Location cannot be null. See exception for details.");

            return BadRequest(new JSendResponse
            {
                Status = JSendStatus.Fail,
                Message = ex.Message
            });
        }
    }

    [HttpDelete("{id:int}")]
    [SwaggerOperation(
        Summary = "Deletes an existing location in the database")]
    public async Task<IActionResult> DeleteLocationAsync([FromRoute] int id)
    {
        try
        {
            await _logic.DeleteLocationAsync(id);
            return Ok(new JSendResponse());
        }
        catch (EntityNotFoundException ex)
        {
            Log.Warning(ex, "Cannot delete a location that does not exist in the database. See exception for details.");

            return BadRequest(new JSendResponse
            {
                Status = JSendStatus.Fail,
                Message = ex.Message
            });
        }
    }

    private static Location Map(LocationRequest request, int id = 0)
    {
        return new Location
        {
            Id = id,
            SportId = request.SportId,
            Name = request.Name,
            Address = request.Address,
            Description = request.Description
        };
    }
}