using SportsComplex.Logic.Exceptions;
using SportsComplex.Logic.Interfaces;
using SportsComplex.Logic.Models;
using SportsComplex.Logic.Repositories;
using SportsComplex.Logic.Validators;

namespace SportsComplex.Logic
{
    public class GuardianLogic : IGuardianLogic
    {
        private readonly IdValidator _idValidator;
        private readonly GuardianValidator _guardianValidator;
        private readonly IGuardianReadRepo _guardianReadRepo;
        private readonly IGuardianWriteRepo _guardianWriteRepo;

        public GuardianLogic(IdValidator idValidator,
            GuardianValidator guardianValidator,
            IGuardianReadRepo guardianReadRepo,
            IGuardianWriteRepo guardianWriteRepo)
        {
            _idValidator = idValidator;
            _guardianValidator = guardianValidator;
            _guardianReadRepo = guardianReadRepo;
            _guardianWriteRepo = guardianWriteRepo;
        }

        public async Task<List<Guardian>> GetGuardiansAsync(GuardianQuery filters)
        {
            return await _guardianReadRepo.GetGuardiansAsync(filters);
        }

        public async Task<Guardian> GetGuardianByIdAsync(int guardianId)
        {
            if (guardianId <= 0)
                throw new InvalidRequestException("'GuardianId' must be greater than 0.");

            return await _guardianReadRepo.GetGuardianByIdAsync(guardianId);
        }

        public async Task<Guardian> AddGuardianAsync(Guardian guardian)
        {
            await Validate(guardian);
            guardian.Id = await _guardianWriteRepo.InsertGuardianAsync(guardian);

            return guardian;
        }

        public async Task<Guardian> UpdateGuardianAsync(Guardian guardian)
        {
            await Validate(guardian, true);
            return await _guardianWriteRepo.UpdateGuardianAsync(guardian);
        }

        public async Task DeleteGuardianAsync(int guardianId)
        {
            await _guardianWriteRepo.DeleteGuardianAsync(guardianId);
        }

        private async Task Validate(Guardian guardian, bool checkId=false)
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