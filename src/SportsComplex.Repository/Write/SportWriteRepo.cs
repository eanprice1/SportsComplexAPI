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

    public async Task<int> InsertSportAsync(Sport model)
    {
        await using var context = new SportsComplexDbContext(_dbContextOptions);
        var entity = Map(model);

        await context.Sport.AddAsync(entity);

        try
        {
            await context.SaveChangesAsync();
            return entity.Id;
        }
        catch (DbUpdateException ex)
        {
            throw new DbWriteEntityException(
                "Could not insert sport into database. See inner exception for details.", ex);
        }
    }

    public async Task<Sport> UpdateSportAsync(Sport model)
    {
        await using var context = new SportsComplexDbContext(_dbContextOptions);
        var entity = Map(model);

        var results = await context.Sport.AsNoTracking()
            .Include(x => x.Teams)
            .ThenInclude(x => x.Players)
            .Where(x => x.Id == entity.Id)
            .FirstOrDefaultAsync();

        if (results == null)
            throw new EntityNotFoundException($"Sport with 'Id={entity.Id}' does not exist in database.");


        if (results.Teams.Any(x => x.Players.Count > results.MaxTeamSize))
            throw new InvalidRequestException(
                "'MaxTeamSize' must be greater than or equal to the player count of all existing teams.");

        context.Sport.Update(entity);

        try
        {
            await context.SaveChangesAsync();
            return model;
        }
        catch (DbUpdateException ex)
        {
            throw new DbWriteEntityException(
                "Could not update sport. Sport may not exist in database. See inner exception for details.", ex);
        }
    }

    public async Task DeleteSportAsync(int id)
    {
        await using var context = new SportsComplexDbContext(_dbContextOptions);
        var trx = await context.Database.BeginTransactionAsync();

        var entity = await context.Sport
            .Include(x => x.Teams)
            .ThenInclude(x => x.HomeMatches)
            .Include(x => x.Teams)
            .ThenInclude(x => x.AwayMatches)
            .Include(x => x.Teams)
            .ThenInclude(x => x.Practices)
            .Include(x => x.Locations)
            .Where(x => x.Id == id)
            .SingleOrDefaultAsync();

        if (entity == null)
            throw new EntityNotFoundException($"Sport with 'Id={id}' does not exist.");


        foreach (var team in entity.Teams)
        {
            context.Match.RemoveRange(team.HomeMatches);
            context.Match.RemoveRange(team.AwayMatches);
            context.Practice.RemoveRange(team.Practices);
        }

        context.Team.RemoveRange(entity.Teams);
        context.Location.RemoveRange(entity.Locations);
        context.Sport.Remove(entity);

        try
        {
            await context.SaveChangesAsync();
            await trx.CommitAsync();
        }
        catch (DbUpdateException ex)
        {
            await trx.RollbackAsync();
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
            MaxTeamSize = model.MaxTeamSize,
            StartDate = model.StartDate,
            EndDate = model.EndDate
        };
    }
}