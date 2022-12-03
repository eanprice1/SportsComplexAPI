using SportsComplex.Logic.Models;

namespace SportsComplex.Logic.Repositories;

public interface ITeamWriteRepo
{
    Task<int> InsertTeamAsync(Team model);
    Task<Team> UpdateTeamAsync(Team model);
    Task DeleteTeamAsync(int id);
}