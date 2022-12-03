using Microsoft.EntityFrameworkCore;
using SportsComplex.Logic.Exceptions;
using SportsComplex.Logic.Models;
using SportsComplex.Logic.Repositories;
using SportsComplex.Repository.Entities;

namespace SportsComplex.Repository.Write;

public class PlayerWriteRepo : IPlayerWriteRepo
{
    private readonly DbContextOptions<SportsComplexDbContext> _dbContextOptions;

    public PlayerWriteRepo(DbContextOptions<SportsComplexDbContext> dbContextOptions)
    {
        _dbContextOptions = dbContextOptions;
    }

    public async Task<int> InsertPlayerAsync(Player model)
    {
        await using var context = new SportsComplexDbContext(_dbContextOptions);
        var entity = Map(model);

        await context.Player.AddAsync(entity);

        try
        {
            await context.SaveChangesAsync();
            return entity.Id;
        }
        catch (DbUpdateException ex)
        {
            throw new DbWriteEntityException(
                "Could not insert player into database. See inner exception for details.", ex);
        }
    }

    public async Task<Player> UpdatePlayerAsync(Player model)
    {
        await using var context = new SportsComplexDbContext(_dbContextOptions);
        var entity = Map(model);

        context.Player.Update(entity);

        try
        {
            await context.SaveChangesAsync();
            return model;
        }
        catch (DbUpdateException ex)
        {
            throw new DbWriteEntityException(
                "Could not update player. Player may not exist in database. See inner exception for details.", ex);
        }
    }

    public async Task DeletePlayerAsync(int id)
    {
        await using var context = new SportsComplexDbContext(_dbContextOptions);

        var entity = await context.Player.FindAsync(id);

        if(entity == null)
            throw new EntityNotFoundException($"Player with 'Id={id}' does not exist.");

        context.Player.Remove(entity);

        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            throw new DbWriteEntityException(
                "Could not delete player. See inner exception for details.", ex);
        }
    }

    private static PlayerDb Map(Player model)
    {
        return new PlayerDb
        {
            Id = model.Id,
            TeamId = model.TeamId, 
            GuardianId = model.GuardianId,
            FirstName = model.FirstName,
            LastName = model.LastName,
            BirthDate = model.BirthDate,
            Age = model.Age
        };
    }
}