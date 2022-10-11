using Microsoft.EntityFrameworkCore;
using SportsComplex.Logic.Exceptions;
using SportsComplex.Logic.Models;
using SportsComplex.Logic.Repositories;
using SportsComplex.Repository.Entities;
using static SportsComplex.Logic.Utilities.OrderByColumns.TeamColumns;

namespace SportsComplex.Repository.Read;

public class TeamReadRepo : ITeamReadRepo
{
    private readonly DbContextOptions<SportsComplexDbContext> _dbContextOptions;

    public TeamReadRepo(DbContextOptions<SportsComplexDbContext> dbContextOptions)
    {
        _dbContextOptions = dbContextOptions;
    }

    public async Task<List<Team>> GetTeamsAsync(TeamQuery filters)
    {
        await using var context = new SportsComplexDbContext(_dbContextOptions);
        var sqlQuery = context.Team.AsNoTracking();

        if (filters.Ids.Any())
            sqlQuery = sqlQuery.Where(x => filters.Ids.Contains(x.Id));

        if (filters.SportIds.Any())
            sqlQuery = sqlQuery.Where(x => filters.SportIds.Contains(x.SportId));

        if (filters.Name != null)
            sqlQuery = sqlQuery.Where(x => x.Name == filters.Name);

        sqlQuery = OrderBy(sqlQuery, filters.OrderBy, filters.Descending);

        if (filters.Count.HasValue)
            sqlQuery = sqlQuery.Take(filters.Count.Value);

        return await sqlQuery.Select(x => Map(x)).ToListAsync();
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

    private static IQueryable<TeamDb> OrderBy(IQueryable<TeamDb> sqlQuery, string? orderBy, bool descending)
    {
        return orderBy?.ToLower() switch
        {
            Id => descending
                ? sqlQuery.OrderByDescending(x => x.Id)
                : sqlQuery.OrderBy(x => x.Id),
            SportId => descending
                ? sqlQuery.OrderByDescending(x => x.SportId)
                : sqlQuery.OrderBy(x => x.SportId),
            Name => descending
                ? sqlQuery.OrderByDescending(x => x.Name)
                : sqlQuery.OrderBy(x => x.Name),
            Motto => descending
                ? sqlQuery.OrderByDescending(x => x.Motto)
                : sqlQuery.OrderBy(x => x.Motto),
            _ => descending
                ? sqlQuery.OrderByDescending(x => x.Id)
                : sqlQuery.OrderBy(x => x.Id)
        };
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