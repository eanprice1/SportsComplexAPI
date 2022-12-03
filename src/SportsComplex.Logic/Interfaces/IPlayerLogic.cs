using SportsComplex.Logic.Models;

namespace SportsComplex.Logic.Interfaces;

public interface IPlayerLogic
{
    Task<List<Player>> GetPlayersAsync(PlayerQuery filters);
    Task<Player> GetPlayerByIdAsync(int id);
    Task<Player> AddPlayerAsync(Player player);
    Task<Player> UpdatePlayerAsync(Player player);
    Task DeletePlayerAsync(int id);
}