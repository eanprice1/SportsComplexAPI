using Microsoft.EntityFrameworkCore;
using SportsComplex.Logic.Exceptions;
using SportsComplex.Logic.Models;
using SportsComplex.Logic.Repositories;
using SportsComplex.Repository.Entities;

namespace SportsComplex.Repository.Write;

public class LocationWriteRepo : ILocationWriteRepo
{
    private readonly DbContextOptions<SportsComplexDbContext> _dbContextOptions;

    public LocationWriteRepo(DbContextOptions<SportsComplexDbContext> dbContextOptions)
    {
        _dbContextOptions = dbContextOptions;
    }

    public async Task<int> InsertLocationAsync(Location location)
    {
        await using var context = new SportsComplexDbContext(_dbContextOptions);
        var locationToInsert = Map(location);

        await context.Location.AddAsync(locationToInsert);

        try
        {
            await context.SaveChangesAsync();
            return locationToInsert.Id;
        }
        catch (DbUpdateException ex)
        {
            throw new DbWriteEntityException(
                "Could not insert location into database. See inner exception for details.", ex);
        }
    }

    public async Task<Location> UpdateLocationAsync(Location location)
    {
        await using var context = new SportsComplexDbContext(_dbContextOptions);
        var locationToUpdate = Map(location);

        context.Location.Update(locationToUpdate);

        try
        {
            await context.SaveChangesAsync();
            return location;
        }
        catch (DbUpdateException ex)
        {
            throw new DbWriteEntityException(
                "Could not update location. Location may not exist in database. See inner exception for details.", ex);
        }
    }

    public async Task DeleteLocationAsync(int locationId)
    {
        await using var context = new SportsComplexDbContext(_dbContextOptions);

        var entity = await context.Location.FindAsync(locationId);

        if (entity == null)
            throw new EntityNotFoundException($"Location with 'Id={locationId}' does not exist.");

        context.Location.Remove(entity);

        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            throw new DbWriteEntityException(
                "Could not delete location. See inner exception for details.", ex);
        }
    }

    private static LocationDb Map(Location model)
    {
        return new LocationDb
        {
            Id = model.Id,
            SportId = model.SportId,
            Name = model.Name,
            Address = model.Address,
            Description = model.Description
        };
    }
}