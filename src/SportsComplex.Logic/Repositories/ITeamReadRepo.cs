using SportsComplex.Logic.Models;

namespace SportsComplex.Logic.Repositories;

public interface ITeamReadRepo
{
    Task<Team> GetTeamByIdAsync(int teamId);
}