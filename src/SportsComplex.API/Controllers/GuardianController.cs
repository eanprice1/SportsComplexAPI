using Microsoft.AspNetCore.Mvc;
using Serilog;
using SportsComplex.API.Api.JSend;
using SportsComplex.API.Api.Requests;
using SportsComplex.Logic.Exceptions;
using SportsComplex.Logic.Interfaces;
using SportsComplex.Logic.Models;
using SportsComplex.Repository.Entities;
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

        [HttpGet]
        [SwaggerOperation(
            Summary = "Gets guardians from database")]
        public async Task<IActionResult> GetGuardiansAsync([FromQuery] GetGuardianQuery query)
        {
            var filters = new GuardianQuery
            {
                Count = query.Count,
                Descending = query.Descending,
                OrderBy = query.OrderBy
            };

            var data = await _guardianLogic.GetGuardiansAsync(filters);
            return Ok(new JSendResponse(data));
        }

        [HttpGet("{guardianId:int}")]
        [SwaggerOperation(
            Summary = "Gets guardians from database")]
        public async Task<IActionResult> GetGuardianByIdAsync([FromRoute] int guardianId)
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
        public async Task<IActionResult> AddGuardianAsync([FromBody] GuardianRequest request)
        {
            try
            {
                var guardian = Map(request);
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

        [HttpPut("{guardianId:int}")]
        [SwaggerOperation(
            Summary = "Updates an existing guardian in the database")]
        public async Task<IActionResult> UpdateGuardianAsync([FromRoute] int guardianId, [FromBody] GuardianRequest request)
        {
            try
            {
                var guardian = Map(request, guardianId);
                var data = await _guardianLogic.UpdateGuardianAsync(guardian);

                return Ok(new JSendResponse(data));
            }catch(ArgumentNullException ex)
            {
                Log.Warning(ex, "Guardian cannot be null. See exception for details.");

                return BadRequest(new JSendResponse
                {
                    Status = JSendStatus.Fail,
                    Message = ex.Message
                });
            }
        }

        [HttpDelete("{guardianId:int}")]
        [SwaggerOperation(
            Summary = "Deletes an existing guardian in the database")]
        public async Task<IActionResult> DeleteGuardianAsync([FromRoute] int guardianId)
        {
            try
            {
                await _guardianLogic.DeleteGuardianAsync(guardianId);

                return Ok(new JSendResponse());
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

        private static Guardian Map(GuardianRequest request, int guardianId=0)
        {
            return new Guardian
            {
                Id = guardianId,
                FirstName = request.FirstName,
                LastName = request.LastName,
                BirthDate = request.BirthDate,
                PhoneNumber = request.PhoneNumber,
                Email = request.Email,
                Address = request.Address,
                OtherAddress = request.OtherAddress
            };
        }
    }
}