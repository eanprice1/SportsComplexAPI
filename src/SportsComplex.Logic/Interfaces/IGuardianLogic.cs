using SportsComplex.Logic.Models;

namespace SportsComplex.Logic.Interfaces
{
    public interface IGuardianLogic
    {
        Task<Guardian> GetGuardianByIdAsync(int guardianId);
        Task<int> AddGuardianAsync(Guardian guardian);
        Task<Guardian> UpdateGuardianAsync(Guardian guardian);
        Task DeleteGuardianAsync(int guardianId);
    }
}
