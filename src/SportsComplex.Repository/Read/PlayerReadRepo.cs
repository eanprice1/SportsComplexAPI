using Microsoft.EntityFrameworkCore;
using SportsComplex.Logic.Exceptions;
using SportsComplex.Logic.Models;
using SportsComplex.Logic.Repositories;
using SportsComplex.Repository.Entities;
using static SportsComplex.Logic.Utilities.OrderByColumns.PlayerColumns;

namespace SportsComplex.Repository.Read
{
    public class PlayerReadRepo : IPlayerReadRepo
    {
        private readonly DbContextOptions<SportsComplexDbContext> _dbContextOptions;

        public PlayerReadRepo(DbContextOptions<SportsComplexDbContext> dbContextOptions)
        {
            _dbContextOptions = dbContextOptions;
        }

        public async Task<List<Player>> GetPlayersAsync(PlayerQuery filters)
        {
            await using var context = new SportsComplexDbContext(_dbContextOptions);

            var sqlQuery = context.Player.AsNoTracking();

            if (filters.Ids.Any())
            {
                sqlQuery = sqlQuery.Where(x => filters.Ids.Contains(x.Id));
            }

            if (filters.TeamIds.Any())
            {
                sqlQuery = sqlQuery.Where(x => x.TeamId != null && filters.TeamIds.Contains((int)x.TeamId));
            }

            if (filters.GuardianIds.Any())
            {
                sqlQuery = sqlQuery.Where(x => filters.GuardianIds.Contains(x.GuardianId));
            }

            sqlQuery = OrderBy(sqlQuery, filters.OrderBy, filters.Descending);

            if (filters.Count.HasValue)
            {
                sqlQuery = sqlQuery.Take(filters.Count.Value);
            }

            return await sqlQuery.Select(x => Map(x)).ToListAsync();
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

        private static IQueryable<PlayerDb> OrderBy(IQueryable<PlayerDb> sqlQuery, string? orderBy, bool descending)
        {
            return orderBy?.ToLower() switch
            {
                Id => descending
                    ? sqlQuery.OrderByDescending(x => x.Id)
                    : sqlQuery.OrderBy(x => x.Id),
                TeamId => descending 
                    ? sqlQuery.OrderByDescending(x => x.TeamId)
                    : sqlQuery.OrderBy(x => x.TeamId),
                GuardianId => descending
                    ? sqlQuery.OrderByDescending(x => x.GuardianId)
                    : sqlQuery.OrderBy(x => x.GuardianId),
                FirstName => descending
                    ? sqlQuery.OrderByDescending(x => x.FirstName)
                    : sqlQuery.OrderBy(x => x.FirstName),
                LastName => descending
                    ? sqlQuery.OrderByDescending(x => x.LastName)
                    : sqlQuery.OrderBy(x => x.LastName),
                BirthDate => descending
                    ? sqlQuery.OrderByDescending(x => x.BirthDate)
                    : sqlQuery.OrderBy(x => x.BirthDate),
                Age => descending
                    ? sqlQuery.OrderByDescending(x => x.Age)
                    : sqlQuery.OrderBy(x => x.Age),
                _ => descending
                    ? sqlQuery.OrderByDescending(x => x.Id)
                    : sqlQuery.OrderBy(x => x.Id)
            };
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
