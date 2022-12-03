using SportsComplex.Logic.Models;

namespace SportsComplex.Logic.Repositories;

public interface ITeamReadRepo
{
    Task<List<Team>> GetTeamsAsync(TeamQuery filters);
    Task<Team> GetTeamByIdAsync(int id);
}