using SportsComplex.Logic.Models;

namespace SportsComplex.Logic.Repositories;

public interface IMatchWriteRepo
{
    Task<int> InsertMatchAsync(Match match);
    Task<Match> UpdateMatchAsync(Match match);
    Task DeleteMatchAsync(int matchId);
}