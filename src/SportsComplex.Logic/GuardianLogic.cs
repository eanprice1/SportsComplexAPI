using SportsComplex.Logic.Exceptions;
using SportsComplex.Logic.Interfaces;
using SportsComplex.Logic.Models;
using SportsComplex.Logic.Repositories;
using SportsComplex.Logic.Validators;

namespace SportsComplex.Logic
{
    public class GuardianLogic : IGuardianLogic
    {
        private readonly IGuardianReadRepo _guardianReadRepo;
        private readonly IGuardianWriteRepo _guardianWriteRepo;
        private readonly GuardianValidator _guardianValidator;

        public GuardianLogic(IGuardianReadRepo guardianReadRepo, IGuardianWriteRepo guardianWriteRepo, GuardianValidator guardianValidator)
        {
            _guardianReadRepo = guardianReadRepo;
            _guardianWriteRepo = guardianWriteRepo;
            _guardianValidator = guardianValidator;
        }

        public async Task<Guardian> GetGuardianByIdAsync(int guardianId)
        {
            if (guardianId <= 0)
            {
                throw new InvalidRequestException("'GuardianId' must be greater than 0.");
            }

            return await _guardianReadRepo.GetGuardianByIdAsync(guardianId);
        }

        public async Task<int> AddGuardianAsync(Guardian guardian)
        {
            await Validate(guardian);

            return await _guardianWriteRepo.InsertGuardianAsync(guardian);
        }

        private async Task Validate(Guardian guardian)
        {
            if (guardian == null)
                throw new ArgumentNullException(nameof(guardian));

            var result = await _guardianValidator.ValidateAsync(guardian);

            if (!result.IsValid)
                throw new InvalidRequestException(result.ToString());
        }
    }
}