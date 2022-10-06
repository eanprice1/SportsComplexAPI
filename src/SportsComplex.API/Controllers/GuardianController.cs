using Microsoft.AspNetCore.Mvc;
using Serilog;
using SportsComplex.API.Api.JSend;
using SportsComplex.Logic.Exceptions;
using SportsComplex.Logic.Interfaces;
using SportsComplex.Logic.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace SportsComplex.API.Controllers
{
    [ApiController]
    [Route("guardians")]
    [Produces("application/json")]
    public class GuardianController : ControllerBase
    {
        private readonly IGuardianLogic _guardianLogic;

        public GuardianController(IGuardianLogic guardianLogic)
        {
            _guardianLogic = guardianLogic;
        }

        [HttpGet("{guardianId:int}")]
        [SwaggerOperation(
            Summary = "Gets guardians from database")]
        [SwaggerResponse(StatusCodes.Status200OK, "Successfully returns guardians information.", typeof(JSendResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad request with description of error.", typeof(JSendResponse))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "The guardian was not found.", typeof(JSendResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Unhandled exception was thrown.", typeof(JSendResponse))]
        public async Task<IActionResult> GetGuardianAsync([FromRoute] int guardianId)
        {
            try
            {
                var data = await _guardianLogic.GetGuardianByIdAsync(guardianId);
                return Ok(new JSendResponse(data));
            }
            catch (EntityNotFoundException ex)
            {
                Log.Error(ex, "Could not find guardian with Id {GuardianId} in database. See inner exception for details.", guardianId);

                return NotFound(new JSendResponse
                {
                    Status = JSendStatus.Fail,
                    Message = ex.Message
                });
            }
        }

        [HttpPost]
        [SwaggerOperation(
            Summary = "Inserts a new guardian into the database")]
        [SwaggerResponse(StatusCodes.Status200OK, "Successfully inserts new guardian into database.", typeof(JSendResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad request with description of error.", typeof(JSendResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Unhandled exception was thrown", typeof(JSendResponse))]
        public async Task<IActionResult> AddGuardianAsync([FromBody] Guardian guardian)
        {
            try
            {
                var data = await _guardianLogic.AddGuardianAsync(guardian);
                return Ok(new JSendResponse(data));
            }
            catch (ArgumentNullException ex)
            {
                Log.Warning(ex, "Guardian cannot be null. See exception for details.");

                return BadRequest(new JSendResponse
                {
                    Status = JSendStatus.Fail,
                    Message = ex.Message
                });
            }
        }
    }
}