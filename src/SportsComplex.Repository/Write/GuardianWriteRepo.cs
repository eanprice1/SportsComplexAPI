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

        public async Task<int> InsertGuardianAsync(Guardian model)
        {
            await using var context = new SportsComplexDbContext(_dbContextOptions);
            var entity = Map(model);

            await context.Guardian.AddAsync(entity);

            try
            {
                await context.SaveChangesAsync();
                return entity.Id;
            }
            catch (DbUpdateException ex)
            {
                throw new DbWriteEntityException(
                    "Could not insert guardian into database. See inner exception for details.", ex);
            }
        }

        public async Task<Guardian> UpdateGuardianAsync(Guardian model)
        {
            await using var context = new SportsComplexDbContext(_dbContextOptions);
            var entity = Map(model);

            context.Guardian.Update(entity);

            try
            {
                await context.SaveChangesAsync();
                return model;
            }
            catch (DbUpdateException ex)
            {
                throw new DbWriteEntityException(
                    "Could not update guardian. Guardian may not exist in database. See inner exception for details.", ex);
            }
        }

        public async Task DeleteGuardianAsync(int id)
        {
            await using var context = new SportsComplexDbContext(_dbContextOptions);

            var entity = await context.Guardian
                .Include(x => x.Players)
                .Include(x => x.EmergencyContacts)
                .Where(x => x.Id == id)
                .SingleOrDefaultAsync();

            if (entity == null)
                throw new EntityNotFoundException($"Guardian with 'Id={id}' does not exist.");

            if (entity.Players.Any(x => x.TeamId != null))
                throw new DbWriteEntityException("Cannot delete guardian if referenced by one or more players.");

            context.EmergencyContact.RemoveRange(entity.EmergencyContacts);
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
