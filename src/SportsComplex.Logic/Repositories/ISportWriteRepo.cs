using SportsComplex.Logic.Models;

namespace SportsComplex.Logic.Repositories;

public interface ISportWriteRepo
{
    Task<int> InsertSportAsync(Sport model);
    Task<Sport> UpdateSportAsync(Sport model);
    Task DeleteSportAsync(int id);
}