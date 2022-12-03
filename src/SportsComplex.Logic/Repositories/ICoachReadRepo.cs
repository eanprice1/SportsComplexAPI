using SportsComplex.Logic.Models;

namespace SportsComplex.Logic.Repositories;

public interface ICoachReadRepo
{
    Task<List<Coach>> GetCoachesAsync(CoachQuery filters);
    Task<Coach> GetCoachByIdAsync(int id);
}