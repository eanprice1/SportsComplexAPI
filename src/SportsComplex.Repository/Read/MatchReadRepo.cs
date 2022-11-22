using System.Data;
using Microsoft.EntityFrameworkCore;
using SportsComplex.Logic.Exceptions;
using SportsComplex.Logic.Models;
using SportsComplex.Logic.Repositories;
using SportsComplex.Repository.Entities;
using static SportsComplex.Logic.Utilities.OrderByColumns.MatchColumns;


namespace SportsComplex.Repository.Read;

public class MatchReadRepo : IMatchReadRepo
{
    private readonly DbContextOptions<SportsComplexDbContext> _dbContextOptions;

    public MatchReadRepo(DbContextOptions<SportsComplexDbContext> dbContextOptions)
    {
        _dbContextOptions = dbContextOptions;
    }

    public async Task<List<Match>> GetMatchesAsync(MatchQuery filters)
    {
        await using var context = new SportsComplexDbContext(_dbContextOptions);
        var sqlQuery = context.Match.AsNoTracking();

        if (filters.Ids.Any())
            sqlQuery = sqlQuery.Where(x => filters.Ids.Contains(x.Id));

        if (filters.TeamIds.Any())
            sqlQuery = sqlQuery.Where(x =>
                filters.TeamIds.Contains(x.AwayTeamId) || filters.TeamIds.Contains(x.HomeTeamId));

        if (filters.LocationIds.Any())
            sqlQuery = sqlQuery.Where(x => x.LocationId != null && filters.LocationIds.Contains((int)x.LocationId));

        if (filters.StartRange != null && filters.EndRange != null)
        {
            sqlQuery = sqlQuery.Where(x =>
                (filters.StartRange <= x.EndDateTime && x.EndDateTime <= filters.EndRange) ||
                (filters.StartRange <= x.StartDateTime && x.StartDateTime <= filters.EndRange));
        }

        sqlQuery = OrderBy(sqlQuery, filters.OrderBy, filters.Descending);

        if (filters.Count.HasValue)
            sqlQuery = sqlQuery.Take(filters.Count.Value);

        return await sqlQuery.Select(x => Map(x)).ToListAsync();
    }

    public async Task<Match> GetMatchByIdAsync(int matchId)
    {
        await using var context = new SportsComplexDbContext(_dbContextOptions);

        var entity = await context.Match.AsNoTracking()
            .Where(x => x.Id == matchId)
            .FirstOrDefaultAsync();

        if (entity == null)
            throw new EntityNotFoundException($"Could not find match with 'Id={matchId}' in database.");

        return Map(entity);
    }

    private static IQueryable<MatchDb> OrderBy(IQueryable<MatchDb> sqlQuery, string? orderBy, bool descending)
    {
        return orderBy?.ToLower() switch
        {
            HomeTeamId => descending
                ? sqlQuery.OrderByDescending(x => x.HomeTeamId)
                : sqlQuery.OrderBy(x => x.HomeTeamId),
            AwayTeamId => descending
                ? sqlQuery.OrderByDescending(x => x.AwayTeamId)
                : sqlQuery.OrderBy(x => x.AwayTeamId),
            LocationId => descending
                ? sqlQuery.OrderByDescending(x => x.LocationId)
                : sqlQuery.OrderBy(x => x.LocationId),
            StartDateTime => descending
                ? sqlQuery.OrderByDescending(x => x.StartDateTime)
                : sqlQuery.OrderBy(x => x.StartDateTime),
            EndDateTime => descending
                ? sqlQuery.OrderByDescending(x => x.EndDateTime)
                : sqlQuery.OrderBy(x => x.EndDateTime),
            _ => descending
                ? sqlQuery.OrderByDescending(x => x.Id)
                : sqlQuery.OrderBy(x => x.Id)
        };
    }

    private static Match Map(MatchDb entity)
    {
        return new Match
        {
            Id = entity.Id,
            HomeTeamId = entity.HomeTeamId,
            AwayTeamId = entity.AwayTeamId,
            LocationId = entity.LocationId,
            StartDateTime = entity.StartDateTime,
            EndDateTime = entity.EndDateTime
        };
    }
}