using SportsComplex.Logic.Exceptions;
using SportsComplex.Logic.Interfaces;
using SportsComplex.Logic.Models;
using SportsComplex.Logic.Repositories;
using SportsComplex.Logic.Validators;

namespace SportsComplex.Logic.Logic;

public class LocationLogic : ILocationLogic
{
    private readonly IdValidator _idValidator;
    private readonly LocationValidator _locationValidator;
    private readonly ILocationReadRepo _locationReadRepo;
    private readonly ILocationWriteRepo _locationWriteRepo;
    private readonly ISportReadRepo _sportReadRepo;

    public LocationLogic(IdValidator idValidator, LocationValidator locationValidator, ILocationReadRepo locationReadRepo, ILocationWriteRepo locationWriteRepo, ISportReadRepo sportReadRepo)
    {
        _idValidator = idValidator;
        _locationValidator = locationValidator;
        _locationReadRepo = locationReadRepo;
        _locationWriteRepo = locationWriteRepo;
        _sportReadRepo = sportReadRepo;
    }

    public async Task<List<Location>> GetLocationsAsync(LocationQuery filters)
    {
        return await _locationReadRepo.GetLocationsAsync(filters);
    }

    public async Task<Location> GetLocationByIdAsync(int id)
    {
        if (id <= 0)
            throw new InvalidRequestException("'LocationId' must be greater than 0.");

        return await _locationReadRepo.GetLocationByIdAsync(id);
    }

    public async Task<Location> AddLocationAsync(Location location)
    {
        await ValidateAsync(location);
        location.Id = await _locationWriteRepo.InsertLocationAsync(location);
        return location;
    }

    public async Task<Location> UpdateLocationAsync(Location location)
    {
        await ValidateAsync(location, true);
        return await _locationWriteRepo.UpdateLocationAsync(location);
    }

    public async Task DeleteLocationAsync(int id)
    {
        await _locationWriteRepo.DeleteLocationAsync(id);
    }

    private async Task ValidateAsync(Location location, bool checkId = false)
    {
        if (location == null)
            throw new ArgumentNullException(nameof(location));

        var result = await _locationValidator.ValidateAsync(location);

        if (!result.IsValid)
            throw new InvalidRequestException(result.ToString());

        if (checkId)
        {
            result = await _idValidator.ValidateAsync(location);

            if (!result.IsValid)
                throw new InvalidRequestException(result.ToString());
        }

        try
        {
            await _sportReadRepo.GetSportByIdAsync(location.SportId);
        }
        catch (EntityNotFoundException ex)
        {
            throw new InvalidRequestException(
                "'SportId' must exist in the database when adding a new location. See inner exception for details.", ex);
        }
    }
}