using SportsComplex.Logic.Models;

namespace SportsComplex.Logic.Interfaces;

public interface IEmergencyContactLogic
{
    Task<List<EmergencyContact>> GetEmergencyContactsAsync(EmergencyContactQuery filters);
    Task<EmergencyContact> GetEmergencyContactByIdAsync(int id);
    Task<EmergencyContact> AddEmergencyContactAsync(EmergencyContact emergencyContact);
    Task<EmergencyContact> UpdateEmergencyContactAsync(EmergencyContact emergencyContact);
    Task DeleteEmergencyContactAsync(int id);
}