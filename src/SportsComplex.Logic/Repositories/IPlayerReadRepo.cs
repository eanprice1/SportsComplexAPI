using SportsComplex.Logic.Models;

namespace SportsComplex.Logic.Repositories;

public interface IPlayerReadRepo
{
    Task<List<Player>> GetPlayersAsync(PlayerQuery filters);
    Task<Player> GetPlayerByIdAsync(int playerId);
}