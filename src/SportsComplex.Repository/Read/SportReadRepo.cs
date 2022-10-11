using Microsoft.EntityFrameworkCore;
using SportsComplex.Logic.Exceptions;
using SportsComplex.Logic.Models;
using SportsComplex.Logic.Repositories;
using SportsComplex.Repository.Entities;
using static SportsComplex.Logic.Utilities.OrderByColumns.SportColumns;

namespace SportsComplex.Repository.Read;

public class SportReadRepo : ISportReadRepo
{
    private readonly DbContextOptions<SportsComplexDbContext> _dbContextOptions;

    public SportReadRepo(DbContextOptions<SportsComplexDbContext> dbContextOptions)
    {
        _dbContextOptions = dbContextOptions;
    }

    public async Task<List<Sport>> GetSportsAsync(SportQuery filters)
    {
        await using var context = new SportsComplexDbContext(_dbContextOptions);

        var sqlQuery = context.Sport.AsNoTracking();

        if (filters.Ids.Any())
        {
            sqlQuery = sqlQuery.Where(x => filters.Ids.Contains(x.Id));
        }

        if (filters.Name != null)
        {
            sqlQuery = sqlQuery.Where(x => x.Name == filters.Name);
        }

        if (filters.StartRange != null && filters.EndRange != null)
        {
            sqlQuery = filters.FilterByEndDateFlag 
                ? sqlQuery.Where(x => filters.StartRange <= x.EndDate && x.EndDate <= filters.EndRange) 
                : sqlQuery.Where(x => filters.StartRange <= x.StartDate && x.StartDate <= filters.EndRange);
        }

        sqlQuery = OrderBy(sqlQuery, filters.OrderBy, filters.Descending);

        if (filters.Count.HasValue)
        {
            sqlQuery = sqlQuery.Take(filters.Count.Value);
        }

        return await sqlQuery.Select(x => Map(x)).ToListAsync();
    }

    public async Task<Sport> GetSportByIdAsync(int sportId)
    {
        await using var context = new SportsComplexDbContext(_dbContextOptions);

        var entity = await context.Sport.AsNoTracking()
            .Where(x => x.Id == sportId)
            .FirstOrDefaultAsync();

        if (entity == null)
            throw new EntityNotFoundException($"Could not find sport with 'Id={sportId}' in database.");

        return Map(entity);
    }

    private static IQueryable<SportDb> OrderBy(IQueryable<SportDb> sqlQuery, string? orderBy, bool descending)
    {
        return orderBy?.ToLower() switch
        {
            Id => descending
                ? sqlQuery.OrderByDescending(x => x.Id)
                : sqlQuery.OrderBy(x => x.Id),
            Name => descending
                ? sqlQuery.OrderByDescending(x => x.Name)
                : sqlQuery.OrderBy(x => x.Name),
            Description => descending
                ? sqlQuery.OrderByDescending(x => x.Description)
                : sqlQuery.OrderBy(x => x.Description),
            MaxTeamSize => descending
                ? sqlQuery.OrderByDescending(x => x.MaxTeamSize)
                : sqlQuery.OrderBy(x => x.MaxTeamSize),
            StartDate => descending
                ? sqlQuery.OrderByDescending(x => x.StartDate)
                : sqlQuery.OrderBy(x => x.StartDate),
            EndDate => descending
                ? sqlQuery.OrderByDescending(x => x.EndDate)
                : sqlQuery.OrderBy(x => x.EndDate),
            _ => descending
                ? sqlQuery.OrderByDescending(x => x.Id)
                : sqlQuery.OrderBy(x => x.Id)
        };
    }

    private static Sport Map(SportDb entity)
    {
        return new Sport
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            MaxTeamSize = entity.MaxTeamSize,
            StartDate = entity.StartDate,
            EndDate = entity.EndDate
        };
    }
}