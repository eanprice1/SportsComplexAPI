using SportsComplex.Logic.Models;

namespace SportsComplex.Logic.Repositories;

public interface IPracticeWriteRepo
{
    Task<int> InsertPracticeAsync(Practice model);
    Task<Practice> UpdatePracticeAsync(Practice model);
    Task DeletePracticeAsync(int id);
}