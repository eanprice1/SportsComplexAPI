using SportsComplex.Logic.Models;

namespace SportsComplex.Logic.Repositories
{
    public interface IGuardianWriteRepo
    {
        Task<int> InsertGuardianAsync(Guardian model);
        Task<Guardian> UpdateGuardianAsync(Guardian model);
        Task DeleteGuardianAsync(int id);
    }
}
