using Microsoft.EntityFrameworkCore;
using SportsComplex.Logic.Exceptions;
using SportsComplex.Logic.Models;
using SportsComplex.Logic.Repositories;
using SportsComplex.Repository.Entities;
using static SportsComplex.Logic.Utilities.OrderByColumns.PracticeColumns;

namespace SportsComplex.Repository.Read;

public class PracticeReadRepo : IPracticeReadRepo
{
    private readonly DbContextOptions<SportsComplexDbContext> _dbContextOptions;

    public PracticeReadRepo(DbContextOptions<SportsComplexDbContext> dbContextOptions)
    {
        _dbContextOptions = dbContextOptions;
    }

    public async Task<List<Practice>> GetPracticesAsync(PracticeQuery filters)
    {
        await using var context = new SportsComplexDbContext(_dbContextOptions);
        var sqlQuery = context.Practice.AsNoTracking();

        if (filters.Ids.Any())
            sqlQuery = sqlQuery.Where(x => filters.Ids.Contains(x.Id));

        if (filters.TeamIds.Any())
            sqlQuery = sqlQuery.Where(x => filters.TeamIds.Contains(x.TeamId));

        if (filters.LocationIds.Any())
            sqlQuery = sqlQuery.Where(x => x.LocationId != null && filters.LocationIds.Contains((int)x.LocationId));

        if (filters.StartRange != null && filters.EndRange != null)
        {
            sqlQuery = sqlQuery.Where(x => filters.StartRange <= x.EndDateTime && x.StartDateTime <= filters.EndRange);
        }

        sqlQuery = OrderBy(sqlQuery, filters.OrderBy, filters.Descending);

        if (filters.Count.HasValue)
            sqlQuery = sqlQuery.Take(filters.Count.Value);

        return await sqlQuery.Select(x => Map(x)).ToListAsync();
    }

    public async Task<Practice> GetPracticeByIdAsync(int practiceId)
    {
        await using var context = new SportsComplexDbContext(_dbContextOptions);

        var entity = await context.Practice.AsNoTracking()
            .Where(x => x.Id == practiceId)
            .FirstOrDefaultAsync();

        if (entity == null)
            throw new EntityNotFoundException($"Could not find practice with 'Id={practiceId}' in database.");

        return Map(entity);
    }

    public async Task<List<Practice>> GetConflictingPracticesAsync(DateTime startRange, DateTime endRange)
    {
        await using var context = new SportsComplexDbContext(_dbContextOptions);

        return await context.Practice.AsNoTracking()
            .Where(x => startRange <= x.EndDateTime && x.StartDateTime <= endRange)
            .Select(x => Map(x))
            .ToListAsync();
    }

    private static IQueryable<PracticeDb> OrderBy(IQueryable<PracticeDb> sqlQuery, string? orderBy, bool descending)
    {
        return orderBy?.ToLower() switch
        {
            TeamId => descending
                ? sqlQuery.OrderByDescending(x => x.TeamId)
                : sqlQuery.OrderBy(x => x.TeamId),
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

    private static Practice Map(PracticeDb entity)
    {
        return new Practice()
        {
            Id = entity.Id,
            TeamId = entity.TeamId,
            LocationId = entity.LocationId,
            StartDateTime = entity.StartDateTime,
            EndDateTime = entity.EndDateTime
        };
    }
}