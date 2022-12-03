using SportsComplex.Logic.Models;

namespace SportsComplex.Logic.Repositories;

public interface IPlayerWriteRepo
{
    Task<int> InsertPlayerAsync(Player model);
    Task<Player> UpdatePlayerAsync(Player model);
    Task DeletePlayerAsync(int id);
}