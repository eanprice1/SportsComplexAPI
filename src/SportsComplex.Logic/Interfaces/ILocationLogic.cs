using SportsComplex.Logic.Models;

namespace SportsComplex.Logic.Interfaces;

public interface ILocationLogic
{
    Task<List<Location>> GetLocationsAsync(LocationQuery filters);
    Task<Location> GetLocationByIdAsync(int locationId);
    Task<Location> AddLocationAsync(Location location);
    Task<Location> UpdateLocationAsync(Location location);
    Task DeleteLocationAsync(int locationId);
}