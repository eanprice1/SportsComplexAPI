using Microsoft.EntityFrameworkCore;
using SportsComplex.Logic.Exceptions;
using SportsComplex.Logic.Models;
using SportsComplex.Logic.Repositories;
using SportsComplex.Repository.Entities;

namespace SportsComplex.Repository.Read;

public class TeamReadRepo : ITeamReadRepo
{
    private readonly DbContextOptions<SportsComplexDbContext> _dbContextOptions;

    public TeamReadRepo(DbContextOptions<SportsComplexDbContext> dbContextOptions)
    {
        _dbContextOptions = dbContextOptions;
    }

    public async Task<Team> GetTeamByIdAsync(int teamId)
    {
        await using var context = new SportsComplexDbContext(_dbContextOptions);

        var entity = await context.Team.AsNoTracking()
            .Where(x => x.Id == teamId)
            .FirstOrDefaultAsync();

        if(entity == null)
            throw new EntityNotFoundException($"Could not find team with 'Id={teamId}' in database.");

        return Map(entity);
    }

    private static Team Map(TeamDb entity)
    {
        return new Team
        {
            Id = entity.Id,
            SportId = entity.SportId,
            Name = entity.Name,
            Motto = entity.Motto
        };
    }
}