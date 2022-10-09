using Microsoft.EntityFrameworkCore;
using SportsComplex.Logic.Exceptions;
using SportsComplex.Logic.Models;
using SportsComplex.Repository.Entities;
using SportsComplex.Logic.Repositories;

namespace SportsComplex.Repository.Write;

public class SportWriteRepo : ISportWriteRepo
{
    private readonly DbContextOptions<SportsComplexDbContext> _dbContextOptions;

    public SportWriteRepo(DbContextOptions<SportsComplexDbContext> dbContextOptions)
    {
        _dbContextOptions = dbContextOptions;
    }

    public async Task<int> InsertSportAsync(Sport sport)
    {
        await using var context = new SportsComplexDbContext(_dbContextOptions);
        var sportToInsert = Map(sport);

        await context.Sport.AddAsync(sportToInsert);

        try
        {
            await context.SaveChangesAsync();
            return sportToInsert.Id;
        }
        catch (DbUpdateException ex)
        {
            throw new DbWriteEntityException(
                "Could not insert sport into database. See inner exception for details.", ex);
        }
    }

    public async Task<Sport> UpdateSportAsync(Sport sport)
    {
        await using var context = new SportsComplexDbContext(_dbContextOptions);
        var sportToUpdate = Map(sport);

        context.Sport.Update(sportToUpdate);

        try
        {
            await context.SaveChangesAsync();
            return sport;
        }
        catch (DbUpdateException ex)
        {
            throw new DbWriteEntityException(
                "Could not update sport. Sport may not exist in database. See inner exception for details.", ex);
        }
    }

    public async Task DeleteSportAsync(int sportId)
    {
        await using var context = new SportsComplexDbContext(_dbContextOptions);

        var entity = await context.Sport
            .Include(x => x.Teams)
            .Include(x => x.Locations)
            .Where(x => x.Id == sportId)
            .SingleOrDefaultAsync();

        if (entity == null)
            throw new EntityNotFoundException($"Sport with 'Id={sportId}' does not exist.");

        if(entity.Teams.Any())
            throw new DbWriteEntityException("Cannot delete sport if referenced by one or more teams.");

        if(entity.Locations.Any())
            throw new DbWriteEntityException("Cannot delete sport if referenced by one or more locations.");

        context.Sport.Remove(entity);

        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            throw new DbWriteEntityException(
                "Could not delete sport. See inner exception for details.", ex);
        }
    }

    private static SportDb Map(Sport model)
    {
        return new SportDb
        {
            Id = model.Id,
            Name = model.Name,
            Description = model.Description,
            MinTeamSize = model.MinTeamSize,
            MaxTeamSize = model.MaxTeamSize,
            StartDate = model.StartDate,
            EndDate = model.EndDate
        };
    }
}