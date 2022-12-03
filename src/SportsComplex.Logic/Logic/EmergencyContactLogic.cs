using SportsComplex.Logic.Exceptions;
using SportsComplex.Logic.Interfaces;
using SportsComplex.Logic.Models;
using SportsComplex.Logic.Repositories;
using SportsComplex.Logic.Validators;

namespace SportsComplex.Logic.Logic;

public class EmergencyContactLogic : IEmergencyContactLogic
{
    private readonly IdValidator _idValidator;
    private readonly EmergencyContactValidator _emergencyContactValidator;
    private readonly IEmergencyContactReadRepo _emergencyContactReadRepo;
    private readonly IEmergencyContactWriteRepo _emergencyContactWriteRepo;
    private readonly IGuardianReadRepo _guardianReadRepo;

    public EmergencyContactLogic(IdValidator idValidator, EmergencyContactValidator emergencyContactValidator, IEmergencyContactReadRepo emergencyContactReadRepo, IEmergencyContactWriteRepo emergencyContactWriteRepo, IGuardianReadRepo guardianReadRepo)
    {
        _idValidator = idValidator;
        _emergencyContactValidator = emergencyContactValidator;
        _emergencyContactReadRepo = emergencyContactReadRepo;
        _emergencyContactWriteRepo = emergencyContactWriteRepo;
        _guardianReadRepo = guardianReadRepo;
    }

    public async Task<List<EmergencyContact>> GetEmergencyContactsAsync(EmergencyContactQuery filters)
    {
        return await _emergencyContactReadRepo.GetEmergencyContactsAsync(filters);
    }

    public async Task<EmergencyContact> GetEmergencyContactByIdAsync(int id)
    {
        if (id <= 0)
            throw new InvalidRequestException("'EmergencyContactId' must be greater than 0.");

        return await _emergencyContactReadRepo.GetEmergencyContactByIdAsync(id);
    }

    public async Task<EmergencyContact> AddEmergencyContactAsync(EmergencyContact emergencyContact)
    {
        await ValidateAsync(emergencyContact);
        emergencyContact.Id = await _emergencyContactWriteRepo.InsertEmergencyContactAsync(emergencyContact);
        return emergencyContact;
    }

    public async Task<EmergencyContact> UpdateEmergencyContactAsync(EmergencyContact emergencyContact)
    {
        await ValidateAsync(emergencyContact, true);
        return await _emergencyContactWriteRepo.UpdateEmergencyContactAsync(emergencyContact);
    }

    public async Task DeleteEmergencyContactAsync(int id)
    {
        await _emergencyContactWriteRepo.DeleteEmergencyContactAsync(id);
    }

    private async Task ValidateAsync(EmergencyContact emergencyContact, bool checkId = false)
    {
        if (emergencyContact == null)
            throw new ArgumentNullException(nameof(emergencyContact));

        var result = await _emergencyContactValidator.ValidateAsync(emergencyContact);

        if (!result.IsValid)
            throw new InvalidRequestException(result.ToString());

        if (checkId)
        {
            result = await _idValidator.ValidateAsync(emergencyContact);

            if (!result.IsValid)
                throw new InvalidRequestException(result.ToString());
        }

        try
        {
            await _guardianReadRepo.GetGuardianByIdAsync(emergencyContact.GuardianId);
        }
        catch (EntityNotFoundException ex)
        {
            throw new InvalidRequestException(
                "'GuardianId' must exist in the database when adding a new emergency contact. See inner exception for details.", ex);
        }
    }
}