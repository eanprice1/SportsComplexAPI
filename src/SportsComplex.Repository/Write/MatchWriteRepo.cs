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

    public async Task<int> InsertMatchAsync(Match model)
    {
        await using var context = new SportsComplexDbContext(_dbContextOptions);
        var entity = Map(model);

        await context.Match.AddAsync(entity);

        try
        {
            await context.SaveChangesAsync();
            return entity.Id;
        }
        catch (DbUpdateException ex)
        {
            throw new DbWriteEntityException(
                "Could not insert match into database. See inner exception for details.", ex);
        }
    }

    public async Task<Match> UpdateMatchAsync(Match model)
    {
        await using var context = new SportsComplexDbContext(_dbContextOptions);
        var entity = Map(model);

        context.Match.Update(entity);

        try
        {
            await context.SaveChangesAsync();
            return model;
        }
        catch (DbUpdateException ex)
        {
            throw new DbWriteEntityException(
                "Could not update match. Match may not exist in database. See inner exception for details.", ex);
        }
    }

    public async Task DeleteMatchAsync(int id)
    {
        await using var context = new SportsComplexDbContext(_dbContextOptions);

        var entity = await context.Match.FindAsync(id);

        if (entity == null)
            throw new EntityNotFoundException($"Match with 'Id={id}' does not exist.");

        context.Match.Remove(entity);

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