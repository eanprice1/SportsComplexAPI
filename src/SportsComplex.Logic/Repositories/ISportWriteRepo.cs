using SportsComplex.Logic.Models;

namespace SportsComplex.Logic.Repositories;

public interface ISportWriteRepo
{
    Task<int> InsertSportAsync(Sport sport);
    Task<Sport> UpdateSportAsync(Sport sport);
    Task DeleteSportAsync(int sportId);
}