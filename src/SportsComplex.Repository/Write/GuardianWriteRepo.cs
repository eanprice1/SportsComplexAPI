using Microsoft.EntityFrameworkCore;
using SportsComplex.Logic.Exceptions;
using SportsComplex.Logic.Models;
using SportsComplex.Logic.Repositories;
using SportsComplex.Repository.Entities;

namespace SportsComplex.Repository.Write
{
    public class GuardianWriteRepo : IGuardianWriteRepo
    {
        private readonly DbContextOptions<SportsComplexDbContext> _dbContextOptions;

        public GuardianWriteRepo(DbContextOptions<SportsComplexDbContext> dbContextOptions)
        {
            _dbContextOptions = dbContextOptions;
        }

        public async Task<int> InsertGuardianAsync(Guardian guardian)
        {
            await using var context = new SportsComplexDbContext(_dbContextOptions);
            var guardianToInsert = Map(guardian);

            await context.Guardian.AddAsync(guardianToInsert);

            try
            {
                await context.SaveChangesAsync();
                return guardianToInsert.Id;
            }
            catch (DbUpdateException ex)
            {
                throw new DbWriteEntityException(
                    "Could not insert guardian into database. See inner exception for details.", ex);
            }
        }

        public async Task<Guardian> UpdateGuardianAsync(Guardian guardian)
        {
            await using var context = new SportsComplexDbContext(_dbContextOptions);
            var guardianToUpdate = Map(guardian);

            context.Guardian.Update(guardianToUpdate);

            try
            {
                await context.SaveChangesAsync();
                return guardian;
            }
            catch (DbUpdateException ex)
            {
                throw new DbWriteEntityException(
                    "Could not update guardian. Guardian may not exist in database. See inner exception for details.", ex);
            }
        }

        public async Task DeleteGuardianAsync(int guardianId)
        {
            await using var context = new SportsComplexDbContext(_dbContextOptions);

            var entity = await context.Guardian
                .Include(x => x.Players)
                .Include(x => x.EmergencyContacts)
                .Where(x => x.Id == guardianId)
                .SingleOrDefaultAsync();

            if (entity == null)
                throw new EntityNotFoundException($"Guardian with 'Id={guardianId}' does not exist.");

            if (entity.Players.Any(x => x.TeamId != null))
                throw new DbWriteEntityException("Cannot delete guardian if referenced by one or more players.");

            context.Guardian.Remove(entity);

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new DbWriteEntityException(
                    "Could not delete guardian. See inner exception for details.", ex);
            }
        }

        private static GuardianDb Map(Guardian model)
        {
            return new GuardianDb
            {
                Id = model.Id,
                FirstName = model.FirstName,
                LastName = model.LastName,
                BirthDate = model.BirthDate,
                PhoneNumber = model.PhoneNumber,
                Email = model.Email,
                Address = model.Address,
                OtherAddress = model.OtherAddress
            };
        }
    }
}
