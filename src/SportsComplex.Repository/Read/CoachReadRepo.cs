using Microsoft.EntityFrameworkCore;
using SportsComplex.Logic.Exceptions;
using SportsComplex.Logic.Models;
using SportsComplex.Repository.Entities;
using static SportsComplex.Logic.Utilities.OrderByColumns.CoachColumns;

namespace SportsComplex.Repository.Read;

public class CoachReadRepo
{
    private readonly DbContextOptions<SportsComplexDbContext> _dbContextOptions;

    public CoachReadRepo(DbContextOptions<SportsComplexDbContext> dbContextOptions)
    {
        _dbContextOptions = dbContextOptions;
    }

    public async Task<List<Coach>> GetCoachesAsync(CoachQuery filters)
    {
        await using var context = new SportsComplexDbContext(_dbContextOptions);
        var sqlQuery = context.Coach.AsNoTracking();

        if (filters.Ids.Any())
            sqlQuery = sqlQuery.Where(x => filters.Ids.Contains(x.Id));

        if (filters.TeamIds.Any() && !filters.OnlyUnassignedCoaches)
            sqlQuery = sqlQuery.Where(x => x.TeamId != null && filters.TeamIds.Contains((int)x.TeamId));

        if (filters.OnlyUnassignedCoaches)
            sqlQuery = sqlQuery.Where(x => x.TeamId == null);

        if (filters.OnlyHeadCoaches)
            sqlQuery = sqlQuery.Where(x => x.IsHeadCoach == true);

        sqlQuery = OrderBy(sqlQuery, filters.OrderBy, filters.Descending);

        if (filters.Count.HasValue)
            sqlQuery = sqlQuery.Take(filters.Count.Value);

        return await sqlQuery.Select(x => Map(x)).ToListAsync();
    }

    public async Task<Coach> GetCoachByIdAsync(int coachId)
    {
        await using var context = new SportsComplexDbContext(_dbContextOptions);

        var entity = await context.Coach.AsNoTracking()
            .Where(x => x.Id == coachId)
            .FirstOrDefaultAsync();

        if (entity == null)
            throw new EntityNotFoundException($"Could not find coach with 'Id={coachId}' in database.");

        return Map(entity);
    }

    private static IQueryable<CoachDb> OrderBy(IQueryable<CoachDb> sqlQuery, string? orderBy, bool descending)
    {
        return orderBy?.ToLower() switch
        {
            TeamId => descending
                ? sqlQuery.OrderByDescending(x => x.TeamId)
                : sqlQuery.OrderBy(x => x.TeamId),
            FirstName => descending
                ? sqlQuery.OrderByDescending(x => x.FirstName)
                : sqlQuery.OrderBy(x => x.FirstName),
            LastName => descending
                ? sqlQuery.OrderByDescending(x => x.LastName)
                : sqlQuery.OrderBy(x => x.LastName),
            BirthDate => descending
                ? sqlQuery.OrderByDescending(x => x.BirthDate)
                : sqlQuery.OrderBy(x => x.BirthDate),
            PhoneNumber => descending
                ? sqlQuery.OrderByDescending(x => x.PhoneNumber)
                : sqlQuery.OrderBy(x => x.PhoneNumber),
            Email => descending
                ? sqlQuery.OrderByDescending(x => x.Email)
                : sqlQuery.OrderBy(x => x.Email),
            Address => descending
                ? sqlQuery.OrderByDescending(x => x.Address)
                : sqlQuery.OrderBy(x => x.Address),
            IsHeadCoach => descending
                ? sqlQuery.OrderByDescending(x => x.IsHeadCoach)
                : sqlQuery.OrderBy(x => x.IsHeadCoach),
            _ => descending
                ? sqlQuery.OrderByDescending(x => x.Id)
                : sqlQuery.OrderBy(x => x.Id)
        };
    }

    private static Coach Map(CoachDb entity)
    {
        return new Coach
        {
            Id = entity.Id,
            TeamId = entity.TeamId,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            BirthDate = entity.BirthDate,
            PhoneNumber = entity.PhoneNumber,
            Email = entity.Email,
            Address = entity.Address,
            IsHeadCoach = entity.IsHeadCoach
        };
    }
}