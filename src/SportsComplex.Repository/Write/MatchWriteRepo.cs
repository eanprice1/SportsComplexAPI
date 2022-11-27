using Microsoft.EntityFrameworkCore;
using SportsComplex.Logic.Exceptions;
using SportsComplex.Logic.Models;
using SportsComplex.Logic.Repositories;
using SportsComplex.Repository.Entities;

namespace SportsComplex.Repository.Write;

public class MatchWriteRepo : IMatchWriteRepo
{
    private readonly DbContextOptions<SportsComplexDbContext> _dbContextOptions;

    public MatchWriteRepo(DbContextOptions<SportsComplexDbContext> dbContextOptions)
    {
        _dbContextOptions = dbContextOptions;
    }

    public async Task<int> InsertMatchAsync(Match match)
    {
        await using var context = new SportsComplexDbContext(_dbContextOptions);
        var matchToInsert = Map(match);

        await context.Match.AddAsync(matchToInsert);

        try
        {
            await context.SaveChangesAsync();
            return matchToInsert.Id;
        }
        catch (DbUpdateException ex)
        {
            throw new DbWriteEntityException(
                "Could not insert match into database. See inner exception for details.", ex);
        }
    }

    public async Task<Match> UpdateMatchAsync(Match match)
    {
        await using var context = new SportsComplexDbContext(_dbContextOptions);
        var matchToUpdate = Map(match);

        context.Match.Update(matchToUpdate);

        try
        {
            await context.SaveChangesAsync();
            return match;
        }
        catch (DbUpdateException ex)
        {
            throw new DbWriteEntityException(
                "Could not update match. Match may not exist in database. See inner exception for details.", ex);
        }
    }

    public async Task DeleteMatchAsync(int matchId)
    {
        await using var context = new SportsComplexDbContext(_dbContextOptions);

        var entity = await context.Match.FindAsync(matchId);

        if (entity == null)
            throw new EntityNotFoundException($"Match with 'Id={matchId}' does not exist.");

        context.Remove(entity);

        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            throw new DbWriteEntityException(
                "Could not delete match. See inner exception for details.", ex);
        }
    }

    private static MatchDb Map(Match model)
    {
        return new MatchDb
        {
            Id = model.Id,
            HomeTeamId = model.HomeTeamId,
            AwayTeamId = model.AwayTeamId,
            LocationId = model.LocationId,
            StartDateTime = model.StartDateTime,
            EndDateTime = model.EndDateTime
        };
    }
}