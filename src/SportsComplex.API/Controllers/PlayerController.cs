using Microsoft.AspNetCore.Mvc;
using Serilog;
using SportsComplex.API.Api.JSend;
using SportsComplex.API.Api.Requests;
using SportsComplex.Logic.Exceptions;
using SportsComplex.Logic.Interfaces;
using SportsComplex.Logic.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace SportsComplex.API.Controllers
{
    [ApiController]
    [Route("players")]
    [Produces("application/json")]
    public class PlayerController : ControllerBase
    {
        private readonly IPlayerLogic _logic;

        public PlayerController(IPlayerLogic logic)
        {
            _logic = logic;
        }

        [HttpGet]
        [SwaggerOperation(
            Summary = "Gets players from database")]
        public async Task<IActionResult> GetPlayersAsync([FromQuery] GetPlayerQuery query)
        {
            var filters = new PlayerQuery()
            {
                Ids = query.Ids,
                TeamIds = query.TeamIds,
                GuardianIds = query.GuardianIds,
                FirstName = query.FirstName,
                LastName = query.LastName,
                OnlyUnassignedPlayers = query.OnlyUnassignedPlayers,
                Count = query.Count,
                Descending = query.Descending,
                OrderBy = query.OrderBy
            };

            var data = await _logic.GetPlayersAsync(filters);
            return Ok(new JSendResponse(data));
        }

        [HttpGet("{id:int}")]
        [SwaggerOperation(
            Summary = "Gets existing player from database")]
        public async Task<IActionResult> GetPlayerByIdAsync([FromRoute] int id)
        {
            try
            {
                var data = await _logic.GetPlayerByIdAsync(id);
                return Ok(new JSendResponse(data));
            }
            catch (EntityNotFoundException ex)
            {
                Log.Error(ex, "Could not find player with 'Id={PlayerId}' in database. See inner exception for details.", id);

                return NotFound(new JSendResponse
                {
                    Status = JSendStatus.Fail,
                    Message = ex.Message
                });
            }
        }

        [HttpPost]
        [SwaggerOperation(
            Summary = "Inserts a new player into the database")]
        public async Task<IActionResult> AddPlayerAsync([FromBody] PlayerRequest request)
        {
            try
            {
                var player = Map(request);
                var data = await _logic.AddPlayerAsync(player);

                return Ok(new JSendResponse(data));
            }
            catch (ArgumentNullException ex)
            {
                Log.Warning(ex, "Player cannot be null. See exception for details.");

                return BadRequest(new JSendResponse
                {
                    Status = JSendStatus.Fail,
                    Message = ex.Message
                });
            }
        }

        [HttpPut("{id:int}")]
        [SwaggerOperation(
            Summary = "Updates an existing player in the database")]
        public async Task<IActionResult> UpdatePlayerAsync([FromRoute] int id, [FromBody] PlayerRequest request)
        {
            try
            {
                var player = Map(request, id);
                var data = await _logic.UpdatePlayerAsync(player);

                return Ok(new JSendResponse(data));
            }
            catch (ArgumentNullException ex)
            {
                Log.Warning(ex, "Player cannot be null. See exception for details.");

                return BadRequest(new JSendResponse
                {
                    Status = JSendStatus.Fail,
                    Message = ex.Message
                });
            }
        }

        [HttpDelete("{id:int}")]
        [SwaggerOperation(
            Summary = "Deletes an existing player in the database")]
        public async Task<IActionResult> DeletePlayerAsync([FromRoute] int id)
        {
            try
            {
                await _logic.DeletePlayerAsync(id);
                return Ok(new JSendResponse());
            }
            catch (EntityNotFoundException ex)
            {
                Log.Warning(ex, "Cannot delete a player that does not exist in the database. See exception for details.");

                return BadRequest(new JSendResponse
                {
                    Status = JSendStatus.Fail,
                    Message = ex.Message
                });
            }
        }

        private static Player Map(PlayerRequest request, int id = 0)
        {
            return new Player
            {
                Id = id,
                TeamId = request.TeamId == 0 ? null : request.TeamId,
                GuardianId = request.GuardianId,
                FirstName = request.FirstName,
                LastName = request.LastName,
                BirthDate = request.BirthDate,
            };
        }
    }
}
