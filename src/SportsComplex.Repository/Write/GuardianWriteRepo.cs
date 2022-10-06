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
                    "Could not inset guardian into database. See inner exception for details.", ex);
            }
        }

        private static GuardianDb Map(Guardian model)
        {
            return new GuardianDb
            {
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
