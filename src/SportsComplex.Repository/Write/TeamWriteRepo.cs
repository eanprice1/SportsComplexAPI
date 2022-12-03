using Microsoft.EntityFrameworkCore;
using SportsComplex.Logic.Exceptions;
using SportsComplex.Logic.Models;
using SportsComplex.Logic.Repositories;
using SportsComplex.Repository.Entities;

namespace SportsComplex.Repository.Write;

public class TeamWriteRepo : ITeamWriteRepo
{
    private readonly DbContextOptions<SportsComplexDbContext> _dbContextOptions;

    public TeamWriteRepo(DbContextOptions<SportsComplexDbContext> dbContextOptions)
    {
        _dbContextOptions = dbContextOptions;
    }

    public async Task<int> InsertTeamAsync(Team model)
    {
        await using var context = new SportsComplexDbContext(_dbContextOptions);
        var entity = Map(model);

        await context.Team.AddAsync(entity);

        try
        {
            await context.SaveChangesAsync();
            return entity.Id;
        }
        catch (DbUpdateException ex)
        {
            throw new DbWriteEntityException(
                "Could not insert team into database. See inner exception for details.", ex);
        }
    }

    public async Task<Team> UpdateTeamAsync(Team model)
    {
        await using var context = new SportsComplexDbContext(_dbContextOptions);
        var entity = Map(model);

        context.Team.Update(entity);

        try
        {
            await context.SaveChangesAsync();
            return model;
        }
        catch (DbUpdateException ex)
        {
            throw new DbWriteEntityException(
                "Could not update team. Team may not exist in database. See inner exception for details.", ex);
        }
    }

    public async Task DeleteTeamAsync(int id)
    {
        await using var context = new SportsComplexDbContext(_dbContextOptions);
        var trx = await context.Database.BeginTransactionAsync();

        var entity = await context.Team
            .Include(x => x.HomeMatches)
            .Include(x => x.AwayMatches)
            .Include(x => x.Practices)
            .Where(x => x.Id == id)
            .SingleOrDefaultAsync();

        if (entity == null)
            throw new EntityNotFoundException($"Team with 'Id={id}' does not exist.");

        context.Match.RemoveRange(entity.AwayMatches);
        context.Match.RemoveRange(entity.HomeMatches);
        context.Practice.RemoveRange(entity.Practices);
        context.Team.Remove(entity);

        try
        {
            await context.SaveChangesAsync();
            await trx.CommitAsync();
        }
        catch (DbUpdateException ex)
        {
            await trx.RollbackAsync();
            throw new DbWriteEntityException(
                "Could not delete team. See inner exception for details.", ex);
        }
    }

    private static TeamDb Map(Team model)
    {
        return new TeamDb
        {
            Id = model.Id,
            SportId = model.SportId,
            Name = model.Name,
            Motto = model.Motto
        };
    }
}