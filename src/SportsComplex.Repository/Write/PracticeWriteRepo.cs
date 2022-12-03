using Microsoft.EntityFrameworkCore;
using SportsComplex.Logic.Exceptions;
using SportsComplex.Logic.Models;
using SportsComplex.Logic.Repositories;
using SportsComplex.Repository.Entities;

namespace SportsComplex.Repository.Write;

public class PracticeWriteRepo : IPracticeWriteRepo
{
    private readonly DbContextOptions<SportsComplexDbContext> _dbContextOptions;

    public PracticeWriteRepo(DbContextOptions<SportsComplexDbContext> dbContextOptions)
    {
        _dbContextOptions = dbContextOptions;
    }

    public async Task<int> InsertPracticeAsync(Practice model)
    {
        await using var context = new SportsComplexDbContext(_dbContextOptions);
        var entity = Map(model);

        await context.Practice.AddAsync(entity);

        try
        {
            await context.SaveChangesAsync();
            return entity.Id;
        }
        catch (DbUpdateException ex)
        {
            throw new DbWriteEntityException(
                "Could not insert practice into database. See inner exception for details.", ex);
        }
    }

    public async Task<Practice> UpdatePracticeAsync(Practice model)
    {
        await using var context = new SportsComplexDbContext(_dbContextOptions);
        var entity = Map(model);

        context.Practice.Update(entity);

        try
        {
            await context.SaveChangesAsync();
            return model;
        }
        catch (DbUpdateException ex)
        {
            throw new DbWriteEntityException(
                "Could not update practice. Practice may not exist in database. See inner exception for details.", ex);
        }
    }

    public async Task DeletePracticeAsync(int id)
    {
        await using var context = new SportsComplexDbContext(_dbContextOptions);

        var entity = await context.Practice.FindAsync(id);

        if (entity == null)
            throw new EntityNotFoundException($"Practice with 'Id={id}' does not exist.");

        context.Practice.Remove(entity);

        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            throw new DbWriteEntityException(
                "Could not delete practice. See inner exception for details.", ex);
        }
    }

    private static PracticeDb Map(Practice model)
    {
        return new PracticeDb
        {
            Id = model.Id,
            TeamId = model.TeamId,
            LocationId = model.LocationId,
            StartDateTime = model.StartDateTime,
            EndDateTime = model.EndDateTime
        };
    }
}