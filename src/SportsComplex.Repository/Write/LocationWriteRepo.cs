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

    public async Task<int> InsertLocationAsync(Location model)
    {
        await using var context = new SportsComplexDbContext(_dbContextOptions);
        var entity = Map(model);

        await context.Location.AddAsync(entity);

        try
        {
            await context.SaveChangesAsync();
            return entity.Id;
        }
        catch (DbUpdateException ex)
        {
            throw new DbWriteEntityException(
                "Could not insert location into database. See inner exception for details.", ex);
        }
    }

    public async Task<Location> UpdateLocationAsync(Location model)
    {
        await using var context = new SportsComplexDbContext(_dbContextOptions);
        var entity = Map(model);

        context.Location.Update(entity);

        try
        {
            await context.SaveChangesAsync();
            return model;
        }
        catch (DbUpdateException ex)
        {
            throw new DbWriteEntityException(
                "Could not update location. Location may not exist in database. See inner exception for details.", ex);
        }
    }

    public async Task DeleteLocationAsync(int id)
    {
        await using var context = new SportsComplexDbContext(_dbContextOptions);

        var entity = await context.Location.FindAsync(id);

        if (entity == null)
            throw new EntityNotFoundException($"Location with 'Id={id}' does not exist.");

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