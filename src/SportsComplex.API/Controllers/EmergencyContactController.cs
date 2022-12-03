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
[Route("emergencyContacts")]
[Produces("application/json")]
public class EmergencyContactController : ControllerBase
{
    private readonly IEmergencyContactLogic _logic;

    public EmergencyContactController(IEmergencyContactLogic logic)
    {
        _logic = logic;
    }

    [HttpGet]
    [SwaggerOperation(
        Summary = "Gets emergency contacts from database")]
    public async Task<IActionResult> GetEmergencyContactsAsync([FromQuery] GetEmergencyContactQuery query)
    {
        var filters = new EmergencyContactQuery()
        {
            Ids = query.Ids,
            GuardianIds = query.GuardianIds,
            FirstName = query.FirstName,
            LastName = query.LastName,
            Count = query.Count,
            Descending = query.Descending,
            OrderBy = query.OrderBy
        };

        var data = await _logic.GetEmergencyContactsAsync(filters);
        return Ok(new JSendResponse(data));
    }

    [HttpGet("{id:int}")]
    [SwaggerOperation(
        Summary = "Gets existing emergency contact from database")]
    public async Task<IActionResult> GetEmergencyContactByIdAsync([FromRoute] int id)
    {
        try
        {
            var data = await _logic.GetEmergencyContactByIdAsync(id);
            return Ok(new JSendResponse(data));
        }
        catch (EntityNotFoundException ex)
        {
            Log.Error(ex, "Could not find emergency contact with 'Id={EmergencyContactId}' in database. See inner exception for details.", id);

            return NotFound(new JSendResponse
            {
                Status = JSendStatus.Fail,
                Message = ex.Message
            });
        }
    }

    [HttpPost]
    [SwaggerOperation(
        Summary = "Inserts a new emergency contact into the database")]
    public async Task<IActionResult> AddEmergencyContactAsync([FromBody] EmergencyContactRequest request)
    {
        try
        {
            var emergencyContact = Map(request);
            var data = await _logic.AddEmergencyContactAsync(emergencyContact);
            return Ok(new JSendResponse(data));
        }
        catch (ArgumentNullException ex)
        {
            Log.Warning(ex, "Emergency contact cannot be null. See exception for details.");

            return BadRequest(new JSendResponse
            {
                Status = JSendStatus.Fail,
                Message = ex.Message
            });
        }
    }

    [HttpPut("{id:int}")]
    [SwaggerOperation(
        Summary = "Updates an existing emergency contact in the database")]
    public async Task<IActionResult> UpdateEmergencyContactAsync([FromRoute] int id, [FromBody] EmergencyContactRequest request)
    {
        try
        {
            var emergencyContact = Map(request, id);
            var data = await _logic.UpdateEmergencyContactAsync(emergencyContact);

            return Ok(new JSendResponse(data));
        }
        catch (ArgumentNullException ex)
        {
            Log.Warning(ex, "Emergency contact cannot be null. See exception for details.");

            return BadRequest(new JSendResponse
            {
                Status = JSendStatus.Fail,
                Message = ex.Message
            });
        }
    }

    [HttpDelete("{id:int}")]
    [SwaggerOperation(
        Summary = "Deletes an existing emergency contact in the database")]
    public async Task<IActionResult> DeleteEmergencyContactAsync([FromRoute] int id)
    {
        try
        {
            await _logic.DeleteEmergencyContactAsync(id);
            return Ok(new JSendResponse());
        }
        catch (EntityNotFoundException ex)
        {
            Log.Warning(ex, "Cannot delete an emergency contact that does not exist in the database. See exception for details.");

            return BadRequest(new JSendResponse
            {
                Status = JSendStatus.Fail,
                Message = ex.Message
            });
        }
    }

    private static EmergencyContact Map(EmergencyContactRequest request, int id = 0)
    {
        return new EmergencyContact
        {
            Id = id,
            GuardianId = request.GuardianId,
            FirstName = request.FirstName,
            LastName = request.LastName,
            BirthDate = request.BirthDate,
            PhoneNumber = request.PhoneNumber,
            Email = request.Email,
            Address = request.Address,
            OtherAddress = request.OtherAddress,
            OtherPhoneNumber = request.OtherPhoneNumber
        };
    }
}