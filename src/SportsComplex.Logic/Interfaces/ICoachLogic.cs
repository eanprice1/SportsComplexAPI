using SportsComplex.Logic.Models;

namespace SportsComplex.Logic.Interfaces;

public interface ICoachLogic
{
    Task<List<Coach>> GetCoachesAsync(CoachQuery filters);
    Task<Coach> GetCoachById(int coachId);
    Task<Coach> AddCoachAsync(Coach coach);
    Task<Coach> UpdateCoachAsync(Coach coach);
    Task DeleteCoachAsync(int coachId);
}