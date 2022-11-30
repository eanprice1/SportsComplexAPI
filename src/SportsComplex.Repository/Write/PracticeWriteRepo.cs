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

    public async Task<int> InsertPracticeAsync(Practice practice)
    {
        await using var context = new SportsComplexDbContext(_dbContextOptions);
        var practiceToInsert = Map(practice);

        await context.Practice.AddAsync(practiceToInsert);

        try
        {
            await context.SaveChangesAsync();
            return practiceToInsert.Id;
        }
        catch (DbUpdateException ex)
        {
            throw new DbWriteEntityException(
                "Could not insert practice into database. See inner exception for details.", ex);
        }
    }

    public async Task<Practice> UpdatePracticeAsync(Practice practice)
    {
        await using var context = new SportsComplexDbContext(_dbContextOptions);
        var practiceToUpdate = Map(practice);

        context.Practice.Update(practiceToUpdate);

        try
        {
            await context.SaveChangesAsync();
            return practice;
        }
        catch (DbUpdateException ex)
        {
            throw new DbWriteEntityException(
                "Could not update practice. Practice may not exist in database. See inner exception for details.", ex);
        }
    }

    public async Task DeletePracticeAsync(int practiceId)
    {
        await using var context = new SportsComplexDbContext(_dbContextOptions);

        var entity = await context.Practice.FindAsync(practiceId);

        if (entity == null)
            throw new EntityNotFoundException($"Practice with 'Id={practiceId}' does not exist.");

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