using SportsComplex.Logic.Models;

namespace SportsComplex.Logic.Repositories;

public interface ICoachWriteRepo
{
    Task<int> InsertCoachAsync(Coach coach);
    Task<Coach> UpdateCoachAsync(Coach coach);
    Task DeleteCoachAsync(int coachId);
}