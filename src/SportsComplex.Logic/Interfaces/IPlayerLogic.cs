using SportsComplex.Logic.Models;

namespace SportsComplex.Logic.Interfaces;

public interface IPlayerLogic
{
    Task<Player> GetPlayerByIdAsync(int playerId);
    Task<Player> AddPlayerAsync(Player player);
    Task<Player> UpdatePlayerAsync(Player player);
    Task DeletePlayerAsync(int playerId);
}