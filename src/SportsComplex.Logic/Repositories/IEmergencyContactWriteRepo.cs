using SportsComplex.Logic.Models;

namespace SportsComplex.Logic.Repositories;

public interface IEmergencyContactWriteRepo
{
    Task<int> InsertEmergencyContactAsync(EmergencyContact model);
    Task<EmergencyContact> UpdateEmergencyContactAsync(EmergencyContact model);
    Task DeleteEmergencyContactAsync(int id);
}