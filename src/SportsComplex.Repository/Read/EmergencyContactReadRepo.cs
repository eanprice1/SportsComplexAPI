using Microsoft.EntityFrameworkCore;
using SportsComplex.Logic.Exceptions;
using SportsComplex.Logic.Models;
using SportsComplex.Logic.Repositories;
using SportsComplex.Repository.Entities;
using static SportsComplex.Logic.Utilities.OrderByColumns.EmergencyContactColumns;

namespace SportsComplex.Repository.Read;

public class EmergencyContactReadRepo : IEmergencyContactReadRepo
{
    private readonly DbContextOptions<SportsComplexDbContext> _dbContextOptions;

    public EmergencyContactReadRepo(DbContextOptions<SportsComplexDbContext> dbContextOptions)
    {
        _dbContextOptions = dbContextOptions;
    }

    public async Task<List<EmergencyContact>> GetEmergencyContactsAsync(EmergencyContactQuery filters)
    {
        await using var context = new SportsComplexDbContext(_dbContextOptions);
        var sqlQuery = context.EmergencyContact.AsNoTracking();

        if (filters.Ids.Any())
            sqlQuery = sqlQuery.Where(x => filters.Ids.Contains(x.Id));

        if (filters.GuardianIds.Any())
            sqlQuery = sqlQuery.Where(x => filters.GuardianIds.Contains(x.GuardianId));

        if (filters.FirstName != null)
            sqlQuery = sqlQuery.Where(x => x.FirstName.Contains(filters.FirstName));

        if (filters.LastName != null)
            sqlQuery = sqlQuery.Where(x => x.LastName.Contains(filters.LastName));

        sqlQuery = OrderBy(sqlQuery, filters.OrderBy, filters.Descending);

        if (filters.Count.HasValue)
            sqlQuery = sqlQuery.Take(filters.Count.Value);

        return await sqlQuery.Select(x => Map(x)).ToListAsync();
    }

    public async Task<EmergencyContact> GetEmergencyContactByIdAsync(int id)
    {
        await using var context = new SportsComplexDbContext(_dbContextOptions);

        var entity = await context.EmergencyContact.AsNoTracking()
            .Where(x => x.Id == id)
            .FirstOrDefaultAsync();

        if (entity == null)
            throw new EntityNotFoundException($"Could not find emergency contact with 'Id={id}' in database.");

        return Map(entity);
    }

    private static IQueryable<EmergencyContactDb> OrderBy(IQueryable<EmergencyContactDb> sqlQuery, string? orderBy, bool descending)
    {
        return orderBy?.ToLower() switch
        {
            GuardianId => descending
                ? sqlQuery.OrderByDescending(x => x.GuardianId)
                : sqlQuery.OrderBy(x => x.GuardianId),
            FirstName => descending
                ? sqlQuery.OrderByDescending(x => x.FirstName)
                : sqlQuery.OrderBy(x => x.FirstName),
            LastName => descending
                ? sqlQuery.OrderByDescending(x => x.LastName)
                : sqlQuery.OrderBy(x => x.LastName),
            BirthDate => descending
                ? sqlQuery.OrderByDescending(x => x.BirthDate)
                : sqlQuery.OrderBy(x => x.BirthDate),
            PhoneNumber => descending
                ? sqlQuery.OrderByDescending(x => x.PhoneNumber)
                : sqlQuery.OrderBy(x => x.PhoneNumber),
            Email => descending
                ? sqlQuery.OrderByDescending(x => x.Email)
                : sqlQuery.OrderBy(x => x.Email),
            Address => descending
                ? sqlQuery.OrderByDescending(x => x.Address)
                : sqlQuery.OrderBy(x => x.Address),
            OtherAddress => descending
                ? sqlQuery.OrderByDescending(x => x.OtherAddress)
                : sqlQuery.OrderBy(x => x.OtherAddress),
            OtherPhoneNumber => descending
                ? sqlQuery.OrderByDescending(x => x.OtherPhoneNumber)
                : sqlQuery.OrderBy(x => x.OtherPhoneNumber),
            _ => descending
                ? sqlQuery.OrderByDescending(x => x.Id)
                : sqlQuery.OrderBy(x => x.Id)
        };
    }

    private static EmergencyContact Map(EmergencyContactDb entity)
    {
        return new EmergencyContact
        {
            Id = entity.Id,
            GuardianId = entity.GuardianId,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            BirthDate = entity.BirthDate,
            PhoneNumber = entity.PhoneNumber,
            Email = entity.Email,
            Address = entity.Address,
            OtherAddress = entity.OtherAddress,
            OtherPhoneNumber = entity.OtherPhoneNumber
        };
    }
}