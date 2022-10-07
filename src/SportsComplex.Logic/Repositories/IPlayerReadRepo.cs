using SportsComplex.Logic.Models;

namespace SportsComplex.Logic.Repositories;

public interface IPlayerReadRepo
{
    Task<Player> GetPlayerByIdAsync(int playerId);
}