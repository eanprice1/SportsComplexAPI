using Microsoft.EntityFrameworkCore;
using SportsComplex.Logic.Exceptions;
using SportsComplex.Logic.Models;
using SportsComplex.Repository.Entities;
using System.Xml.Linq;
using SportsComplex.Logic.Repositories;
using static SportsComplex.Logic.Utilities.OrderByColumns.LocationColumns;

namespace SportsComplex.Repository.Read;

public class LocationReadRepo : ILocationReadRepo
{
    private readonly DbContextOptions<SportsComplexDbContext> _dbContextOptions;

    public LocationReadRepo(DbContextOptions<SportsComplexDbContext> dbContextOptions)
    {
        _dbContextOptions = dbContextOptions;
    }

    public async Task<List<Location>> GetLocationsAsync(LocationQuery filters)
    {
        await using var context = new SportsComplexDbContext(_dbContextOptions);
        var sqlQuery = context.Location.AsNoTracking();

        if (filters.Ids.Any())
            sqlQuery = sqlQuery.Where(x => filters.Ids.Contains(x.Id));

        if (filters.SportIds.Any())
            sqlQuery = sqlQuery.Where(x => filters.SportIds.Contains(x.SportId));

        if (filters.Name != null)
            sqlQuery = sqlQuery.Where(x => x.Name.Contains(filters.Name));

        if (filters.Address != null)
            sqlQuery = sqlQuery.Where(x => x.Address.Contains(filters.Address));

        sqlQuery = OrderBy(sqlQuery, filters.OrderBy, filters.Descending);

        if (filters.Count.HasValue)
            sqlQuery = sqlQuery.Take(filters.Count.Value);

        return await sqlQuery.Select(x => Map(x)).ToListAsync();
    }

    public async Task<Location> GetLocationByIdAsync(int id)
    {
        await using var context = new SportsComplexDbContext(_dbContextOptions);

        var entity = await context.Location.AsNoTracking()
            .Where(x => x.Id == id)
            .FirstOrDefaultAsync();

        if(entity == null)
            throw new EntityNotFoundException($"Could not find location with 'Id={id}' in database.");

        return Map(entity);
    }

    private static IQueryable<LocationDb> OrderBy(IQueryable<LocationDb> sqlQuery, string? orderBy, bool descending)
    {
        return orderBy?.ToLower() switch
        {
            SportId => descending
                ? sqlQuery.OrderByDescending(x => x.SportId)
                : sqlQuery.OrderBy(x => x.SportId),
            Name => descending
                ? sqlQuery.OrderByDescending(x => x.Name)
                : sqlQuery.OrderBy(x => x.Name),
            Address => descending
                ? sqlQuery.OrderByDescending(x => x.Address)
                : sqlQuery.OrderBy(x => x.Address),
            Description => descending
                ? sqlQuery.OrderByDescending(x => x.Description)
                : sqlQuery.OrderBy(x => x.Description),
            _ => descending
                ? sqlQuery.OrderByDescending(x => x.Id)
                : sqlQuery.OrderBy(x => x.Id)
        };
    }

    private static Location Map(LocationDb entity)
    {
        return new Location
        {
            Id = entity.Id,
            SportId = entity.SportId,
            Name = entity.Name,
            Address = entity.Address,
            Description = entity.Description
        };
    }
}