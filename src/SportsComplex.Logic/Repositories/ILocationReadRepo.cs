using SportsComplex.Logic.Models;

namespace SportsComplex.Logic.Repositories;

public interface ILocationReadRepo
{
    Task<List<Location>> GetLocationsAsync(LocationQuery filters);
    Task<Location> GetLocationByIdAsync(int id);
}