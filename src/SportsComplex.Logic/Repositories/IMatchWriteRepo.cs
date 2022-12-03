using SportsComplex.Logic.Models;

namespace SportsComplex.Logic.Repositories;

public interface IMatchWriteRepo
{
    Task<int> InsertMatchAsync(Match model);
    Task<Match> UpdateMatchAsync(Match model);
    Task DeleteMatchAsync(int id);
}