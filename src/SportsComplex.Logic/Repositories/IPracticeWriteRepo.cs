using SportsComplex.Logic.Models;

namespace SportsComplex.Logic.Repositories;

public interface IPracticeWriteRepo
{
    Task<int> InsertPracticeAsync(Practice practice);
    Task<Practice> UpdatePracticeAsync(Practice practice);
    Task DeletePracticeAsync(int practiceId);
}