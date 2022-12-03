using SportsComplex.Logic.Models;

namespace SportsComplex.Logic.Repositories
{
    public interface IMatchReadRepo
    {
        Task<List<Match>> GetMatchesAsync(MatchQuery filters);
        Task<Match> GetMatchByIdAsync(int id);
        Task<List<Match>> GetConflictingMatchesAsync(DateTime startRange, DateTime endRange);
    }
}
