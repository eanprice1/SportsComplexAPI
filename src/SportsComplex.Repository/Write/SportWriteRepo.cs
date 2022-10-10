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

        var sportDb = await context.Sport
            .Include(x => x.Teams)
            .ThenInclude(x => x.Players)
            .Where(x => x.Id == sportToUpdate.Id)
            .FirstOrDefaultAsync();

        var invalidPlayerCount = sportDb.Teams.Any(x =>
            x.Players.Count < sportDb.MinTeamSize || x.Players.Count > sportDb.MaxTeamSize);

        if (invalidPlayerCount)
            throw new InvalidRequestException(
                "'MinTeamSize' must be less than or equal to the player count of all existing teams. 'MaxTeamSize' must be greater than or equal to the player count of all existing teams.");

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
        var trx = await context.Database.BeginTransactionAsync();

        var entity = await context.Sport
            .Include(x => x.Teams)
            .ThenInclude(x => x.HomeMatches)
            .Include(x => x.Teams)
            .ThenInclude(x => x.AwayMatches)
            .Include(x => x.Teams)
            .ThenInclude(x => x.Practices)
            .Include(x => x.Locations)
            .Where(x => x.Id == sportId)
            .SingleOrDefaultAsync();

        if (entity == null)
            throw new EntityNotFoundException($"Sport with 'Id={sportId}' does not exist.");


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
            MinTeamSize = model.MinTeamSize,
            MaxTeamSize = model.MaxTeamSize,
            StartDate = model.StartDate,
            EndDate = model.EndDate
        };
    }
}