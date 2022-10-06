using SportsComplex.Logic.Models;

namespace SportsComplex.Logic.Repositories
{
    public interface IGuardianReadRepo
    {
        Task<Guardian> GetGuardianByIdAsync(int guardianId);
    }
}
