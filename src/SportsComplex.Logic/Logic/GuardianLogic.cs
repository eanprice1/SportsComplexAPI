using SportsComplex.Logic.Exceptions;
using SportsComplex.Logic.Interfaces;
using SportsComplex.Logic.Models;
using SportsComplex.Logic.Repositories;
using SportsComplex.Logic.Validators;

namespace SportsComplex.Logic.Logic
{
    public class GuardianLogic : IGuardianLogic
    {
        private readonly IdValidator _idValidator;
        private readonly GuardianValidator _guardianValidator;
        private readonly IGuardianReadRepo _readRepo;
        private readonly IGuardianWriteRepo _writeRepo;

        public GuardianLogic(IdValidator idValidator,
            GuardianValidator guardianValidator,
            IGuardianReadRepo readRepo,
            IGuardianWriteRepo writeRepo)
        {
            _idValidator = idValidator;
            _guardianValidator = guardianValidator;
            _readRepo = readRepo;
            _writeRepo = writeRepo;
        }

        public async Task<List<Guardian>> GetGuardiansAsync(GuardianQuery filters)
        {
            return await _readRepo.GetGuardiansAsync(filters);
        }

        public async Task<Guardian> GetGuardianByIdAsync(int guardianId)
        {
            if (guardianId <= 0)
                throw new InvalidRequestException("'GuardianId' must be greater than 0.");

            return await _readRepo.GetGuardianByIdAsync(guardianId);
        }

        public async Task<Guardian> AddGuardianAsync(Guardian guardian)
        {
            await ValidateAsync(guardian);
            guardian.Id = await _writeRepo.InsertGuardianAsync(guardian);

            return guardian;
        }

        public async Task<Guardian> UpdateGuardianAsync(Guardian guardian)
        {
            await ValidateAsync(guardian, true);
            return await _writeRepo.UpdateGuardianAsync(guardian);
        }

        public async Task DeleteGuardianAsync(int guardianId)
        {
            await _writeRepo.DeleteGuardianAsync(guardianId);
        }

        private async Task ValidateAsync(Guardian guardian, bool checkId = false)
        {
            if (guardian == null)
                throw new ArgumentNullException(nameof(guardian));

            var result = await _guardianValidator.ValidateAsync(guardian);

            if (!result.IsValid)
                throw new InvalidRequestException(result.ToString());

            if (checkId)
            {
                result = await _idValidator.ValidateAsync(guardian);

                if (!result.IsValid)
                    throw new InvalidRequestException(result.ToString());
            }
        }
    }
}