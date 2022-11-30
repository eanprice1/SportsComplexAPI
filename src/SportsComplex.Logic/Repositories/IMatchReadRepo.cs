using SportsComplex.Logic.Models;

namespace SportsComplex.Logic.Repositories
{
    public interface IMatchReadRepo
    {
        Task<List<Match>> GetMatchesAsync(MatchQuery filters);
        Task<Match> GetMatchByIdAsync(int matchId);
        Task<List<Match>> GetConflictingMatchesAsync(DateTime startRange, DateTime endRange);
    }
}
