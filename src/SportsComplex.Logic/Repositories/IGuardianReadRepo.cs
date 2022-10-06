using SportsComplex.Logic.Models;

namespace SportsComplex.Logic.Repositories
{
    public interface IGuardianReadRepo
    {
        Task<List<Guardian>> GetGuardiansAsync(GuardianQuery filters);
        Task<Guardian> GetGuardianByIdAsync(int guardianId);
    }
}
