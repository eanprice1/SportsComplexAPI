using Microsoft.EntityFrameworkCore;
using SportsComplex.Logic.Exceptions;
using SportsComplex.Logic.Models;
using SportsComplex.Logic.Repositories;
using SportsComplex.Repository.Entities;

namespace SportsComplex.Repository.Read
{
    public class GuardianReadRepo : IGuardianReadRepo
    {
        private readonly DbContextOptions<SportsComplexDbContext> _dbContextOptions;

        public GuardianReadRepo(DbContextOptions<SportsComplexDbContext> dbContextOptions)
        {
            _dbContextOptions = dbContextOptions;
        }

        public async Task<Guardian> GetGuardianByIdAsync(int guardianId)
        {
            await using var context = new SportsComplexDbContext(_dbContextOptions);

            var entity = await context.Guardian.AsNoTracking()
                .Where(x => x.Id == guardianId)
                .FirstOrDefaultAsync();

            if (entity == null)
                throw new EntityNotFoundException($"Could not find guardian with Id {guardianId} in database.");

            return Map(entity);
        }

        private static Guardian Map(GuardianDb entity)
        {
            return new Guardian
            {
                Id = entity.Id,
                FirstName = entity.FirstName,
                LastName = entity.LastName,
                BirthDate = entity.BirthDate,
                PhoneNumber = entity.PhoneNumber,
                Email = entity.Email,
                Address = entity.Address,
                OtherAddress = entity.OtherAddress
            };
        }
    }
}
