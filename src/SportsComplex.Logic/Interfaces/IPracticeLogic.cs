using SportsComplex.Logic.Models;

namespace SportsComplex.Logic.Interfaces;

public interface IPracticeLogic
{
    Task<List<Practice>> GetPracticesAsync(PracticeQuery filters);
    Task<Practice> GetPracticeById(int practiceId);
    Task<Practice> AddPracticeAsync(Practice practice);
    Task<Practice> UpdatePracticeAsync(Practice practice);
    Task DeletePracticeAsync(int practiceId);
}