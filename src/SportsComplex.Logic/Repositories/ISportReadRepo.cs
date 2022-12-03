using SportsComplex.Logic.Models;

namespace SportsComplex.Logic.Repositories;

public interface ISportReadRepo
{
    Task<List<Sport>> GetSportsAsync(SportQuery filters);
    Task<Sport> GetSportByIdAsync(int id);
}