using Microsoft.EntityFrameworkCore;
using SportsComplex.Logic.Exceptions;
using SportsComplex.Logic.Models;
using SportsComplex.Logic.Repositories;
using SportsComplex.Logic.Utilities.OrderByColumns;
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

        public async Task<List<Guardian>> GetGuardiansAsync(GuardianQuery filters)
        {
            await using var context = new SportsComplexDbContext(_dbContextOptions);

            var sqlQuery = context.Guardian.AsNoTracking();

            sqlQuery = OrderBy(sqlQuery, filters.OrderBy, filters.Descending);

            if(filters.Count.HasValue)
            {
                sqlQuery = sqlQuery.Take(filters.Count.Value);
            }

            return await sqlQuery.Select(x => Map(x)).ToListAsync();
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

        private static IQueryable<GuardianDb> OrderBy(IQueryable<GuardianDb> sqlQuery, string? orderBy, bool descending)
        {
            return orderBy?.ToLower() switch
            {
                GuardianColumns.Id => descending
                    ? sqlQuery.OrderByDescending(x => x.Id)
                    : sqlQuery.OrderBy(x => x.Id),
                GuardianColumns.FirstName => descending
                    ? sqlQuery.OrderByDescending(x => x.FirstName)
                    : sqlQuery.OrderBy(x => x.FirstName),
                GuardianColumns.LastName => descending
                    ? sqlQuery.OrderByDescending(x => x.LastName)
                    : sqlQuery.OrderBy(x => x.LastName),
                GuardianColumns.BirthDate => descending
                    ? sqlQuery.OrderByDescending(x => x.BirthDate)
                    : sqlQuery.OrderBy(x => x.BirthDate),
                GuardianColumns.PhoneNumber => descending
                    ? sqlQuery.OrderByDescending(x => x.PhoneNumber)
                    : sqlQuery.OrderBy(x => x.PhoneNumber),
                GuardianColumns.Email => descending
                    ? sqlQuery.OrderByDescending(x => x.Email)
                    : sqlQuery.OrderBy(x => x.Email),
                GuardianColumns.Address => descending
                    ? sqlQuery.OrderByDescending(x => x.Address)
                    : sqlQuery.OrderBy(x => x.Address),
                GuardianColumns.OtherAddress => descending
                    ? sqlQuery.OrderByDescending(x => x.OtherAddress)
                    : sqlQuery.OrderBy(x => x.OtherAddress),
                _ => descending
                    ? sqlQuery.OrderByDescending(x => x.Id)
                    : sqlQuery.OrderBy(x => x.Id)
            };
                
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
