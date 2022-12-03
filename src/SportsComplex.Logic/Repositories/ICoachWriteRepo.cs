using SportsComplex.Logic.Models;

namespace SportsComplex.Logic.Repositories;

public interface ICoachWriteRepo
{
    Task<int> InsertCoachAsync(Coach model);
    Task<Coach> UpdateCoachAsync(Coach model);
    Task DeleteCoachAsync(int id);
}