using SportsComplex.Logic.Models;

namespace SportsComplex.Logic.Repositories;

public interface ILocationWriteRepo
{
    Task<int> InsertLocationAsync(Location model);
    Task<Location> UpdateLocationAsync(Location model);
    Task DeleteLocationAsync(int id);
}