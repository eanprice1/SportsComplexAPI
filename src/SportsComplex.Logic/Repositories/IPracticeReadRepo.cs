using SportsComplex.Logic.Models;

namespace SportsComplex.Logic.Repositories;

public interface IPracticeReadRepo
{
    Task<List<Practice>> GetPracticesAsync(PracticeQuery filters);
    Task<Practice> GetPracticeByIdAsync(int practiceId);
    Task<List<Practice>> GetConflictingPracticesAsync(DateTime startRange, DateTime endRange);
}