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
            FilterByEndDateFlag = query.FilterByEndDateFlag,
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
}