using SportsComplex.Logic.Models;

namespace SportsComplex.Logic.Repositories;

public interface IEmergencyContactReadRepo
{
    Task<List<EmergencyContact>> GetEmergencyContactsAsync(EmergencyContactQuery filters);
    Task<EmergencyContact> GetEmergencyContactByIdAsync(int id);
}