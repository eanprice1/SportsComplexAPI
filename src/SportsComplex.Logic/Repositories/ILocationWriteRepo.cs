using SportsComplex.Logic.Models;

namespace SportsComplex.Logic.Repositories;

public interface ILocationWriteRepo
{
    Task<int> InsertLocationAsync(Location location);
    Task<Location> UpdateLocationAsync(Location location);
    Task DeleteLocationAsync(int locationId);
}