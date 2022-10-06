using SportsComplex.Logic.Models;

namespace SportsComplex.Logic.Repositories
{
    public interface IGuardianWriteRepo
    {
        Task<int> InsertGuardianAsync(Guardian guardian);
        Task<Guardian> UpdateGuardianAsync(Guardian guardian);
        Task DeleteGuardianAsync(int guardianId);
    }
}
