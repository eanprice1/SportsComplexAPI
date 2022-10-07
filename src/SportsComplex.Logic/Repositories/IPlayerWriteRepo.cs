using SportsComplex.Logic.Models;

namespace SportsComplex.Logic.Repositories;

public interface IPlayerWriteRepo
{
    Task<int> InsertPlayerAsync(Player player);
    Task<Player> UpdatePlayerAsync(Player player);
    Task DeletePlayerAsync(int playerId);
}