using SportsComplex.Logic.Exceptions;
using SportsComplex.Logic.Interfaces;
using SportsComplex.Logic.Models;
using SportsComplex.Logic.Repositories;
using SportsComplex.Logic.Validators;
using System.Reflection;

namespace SportsComplex.Logic
{
    public class PlayerLogic : IPlayerLogic
    {
        private readonly IdValidator _idValidator;
        private readonly PlayerValidator _playerValidator;
        private readonly IPlayerReadRepo _playerReadRepo;
        private readonly IPlayerWriteRepo _writeRepo;
        private readonly IGuardianReadRepo _guardianReadRepo;
        private readonly ITeamReadRepo _teamReadRepo;

        public PlayerLogic(IdValidator idValidator,
            PlayerValidator playerValidator,
            IPlayerReadRepo playerReadRepo,
            IPlayerWriteRepo writeRepo,
            IGuardianReadRepo guardianReadRepo,
            ITeamReadRepo teamReadRepo)
        {
            _idValidator = idValidator;
            _playerValidator = playerValidator;
            _playerReadRepo = playerReadRepo;
            _writeRepo = writeRepo;
            _guardianReadRepo = guardianReadRepo;
            _teamReadRepo = teamReadRepo;
        }

        public async Task<List<Player>> GetPlayersAsync(PlayerQuery filters)
        {
            return await _playerReadRepo.GetPlayersAsync(filters);
        }

        public async Task<Player> GetPlayerByIdAsync(int playerId)
        {
            if(playerId <= 0)
                throw new InvalidRequestException("'PlayerId' must be greater than 0.");

            return await _playerReadRepo.GetPlayerByIdAsync(playerId);
        }

        public async Task<Player> AddPlayerAsync(Player player)
        {
            await ValidateAsync(player);
            player.Age = CalculateAge(player.BirthDate);
            player.Id = await _writeRepo.InsertPlayerAsync(player);

            return player;
        }

        public async Task<Player> UpdatePlayerAsync(Player player)
        {
            await ValidateAsync(player, true);
            player.Age = CalculateAge(player.BirthDate);

            return await _writeRepo.UpdatePlayerAsync(player);
        }

        public async Task DeletePlayerAsync(int playerId)
        {
            await _writeRepo.DeletePlayerAsync(playerId);
        }

        private async Task ValidateAsync(Player player, bool checkId=false)
        {
            if (player == null)
                throw new ArgumentNullException(nameof(player));

            var result = await _playerValidator.ValidateAsync(player);

            if (!result.IsValid)
                throw new InvalidRequestException(result.ToString());

            if (checkId)
            {
                result = await _idValidator.ValidateAsync(player);

                if (!result.IsValid)
                    throw new InvalidRequestException(result.ToString());
            }

            try
            {
                await _guardianReadRepo.GetGuardianByIdAsync(player.GuardianId);

                if (player.TeamId != null)
                    await _teamReadRepo.GetTeamByIdAsync((int) player.TeamId);
            }
            catch (EntityNotFoundException ex)
            {
                throw new InvalidRequestException(
                    "'GuardianId' and 'TeamId' (if provided) must exist in the database when adding a new player. See inner exception for details.", ex);
            }
        }

        private static int CalculateAge(DateTime birthDate)
        {
            return (int)Math.Floor((DateTime.Now - birthDate).TotalDays / 365.242199);
        }
    }
}
