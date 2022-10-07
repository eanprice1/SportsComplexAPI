using Microsoft.EntityFrameworkCore;
using SportsComplex.Logic.Exceptions;
using SportsComplex.Logic.Models;
using SportsComplex.Logic.Repositories;
using SportsComplex.Repository.Entities;

namespace SportsComplex.Repository.Read
{
    public class PlayerReadRepo : IPlayerReadRepo
    {
        private readonly DbContextOptions<SportsComplexDbContext> _dbContextOptions;

        public PlayerReadRepo(DbContextOptions<SportsComplexDbContext> dbContextOptions)
        {
            _dbContextOptions = dbContextOptions;
        }

        public async Task<Player> GetPlayerByIdAsync(int playerId)
        {
            await using var context = new SportsComplexDbContext(_dbContextOptions);

            var entity = await context.Player.AsNoTracking()
                .Where(x => x.Id == playerId)
            .FirstOrDefaultAsync();

            if(entity == null)
                throw new EntityNotFoundException($"Could not find player with 'Id={playerId}' in database.");

            return Map(entity);
        }

        private static Player Map(PlayerDb entity)
        {
            return new Player
            {
                Id = entity.Id,
                TeamId = entity.TeamId,
                GuardianId = entity.GuardianId,
                FirstName = entity.FirstName,
                LastName = entity.LastName,
                BirthDate = entity.BirthDate,
                Age = entity.Age
            };
        }
    }
}
