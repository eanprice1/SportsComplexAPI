using SportsComplex.Logic.Models;

namespace SportsComplex.Logic.Interfaces;

public interface IMatchLogic
{
    Task<List<Match>> GetMatchesAsync(MatchQuery filters);
    Task<Match> GetMatchById(int id);
    Task<Match> AddMatchAsync(Match match);
    Task<Match> UpdateMatchAsync(Match match);
    Task DeleteMatchAsync(int id);
}