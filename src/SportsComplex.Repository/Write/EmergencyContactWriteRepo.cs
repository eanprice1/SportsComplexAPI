using Microsoft.EntityFrameworkCore;
using SportsComplex.Logic.Exceptions;
using SportsComplex.Logic.Models;
using SportsComplex.Logic.Repositories;
using SportsComplex.Repository.Entities;

namespace SportsComplex.Repository.Write;

public class EmergencyContactWriteRepo : IEmergencyContactWriteRepo
{
    private readonly DbContextOptions<SportsComplexDbContext> _dbContextOptions;

    public EmergencyContactWriteRepo(DbContextOptions<SportsComplexDbContext> dbContextOptions)
    {
        _dbContextOptions = dbContextOptions;
    }

    public async Task<int> InsertEmergencyContactAsync(EmergencyContact model)
    {
        await using var context = new SportsComplexDbContext(_dbContextOptions);
        var entity = Map(model);

        await context.EmergencyContact.AddAsync(entity);

        try
        {
            await context.SaveChangesAsync();
            return entity.Id;
        }
        catch (DbUpdateException ex)
        {
            throw new DbWriteEntityException(
                "Could not insert emergency contact into database. See inner exception for details.", ex);
        }
    }

    public async Task<EmergencyContact> UpdateEmergencyContactAsync(EmergencyContact model)
    {
        await using var context = new SportsComplexDbContext(_dbContextOptions);
        var entity = Map(model);

        context.EmergencyContact.Update(entity);

        try
        {
            await context.SaveChangesAsync();
            return model;
        }
        catch (DbUpdateException ex)
        {
            throw new DbWriteEntityException(
                "Could not update emergency contact. Emergency contact may not exist in database. See inner exception for details.", ex);
        }
    }

    public async Task DeleteEmergencyContactAsync(int id)
    {
        await using var context = new SportsComplexDbContext(_dbContextOptions);

        var entity = await context.EmergencyContact.FindAsync(id);

        if (entity == null)
            throw new EntityNotFoundException($"Emergency contact with 'Id={id}' does not exist.");

        context.EmergencyContact.Remove(entity);

        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            throw new DbWriteEntityException(
                "Could not delete emergency contact. See inner exception for details.", ex);
        }
    }

    private static EmergencyContactDb Map(EmergencyContact model)
    {
        return new EmergencyContactDb
        {
            Id = model.Id,
            GuardianId = model.GuardianId,
            FirstName = model.FirstName,
            LastName = model.LastName,
            BirthDate = model.BirthDate,
            PhoneNumber = model.PhoneNumber,
            Email = model.Email,
            Address = model.Address,
            OtherAddress = model.OtherAddress,
            OtherPhoneNumber = model.OtherPhoneNumber
        };
    }
}