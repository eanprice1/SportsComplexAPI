using SportsComplex.Logic.Models;

namespace SportsComplex.Logic.Interfaces;

public interface ISportLogic
{
    Task<List<Sport>> GetSportsAsync(SportQuery filters);
    Task<Sport> GetSportByIdAsync(int sportId);
    Task<Sport> AddSportAsync(Sport sport);
    Task<Sport> UpdateSportAsync(Sport sport);
    Task DeleteSportAsync(int sportId);
}