using SportsComplex.Logic.Models;

namespace SportsComplex.Logic.Repositories;

public interface ITeamWriteRepo
{
    Task<int> InsertTeamAsync(Team team);
    Task<Team> UpdateTeamAsync(Team team);
    Task DeleteTeamAsync(int teamId);
}