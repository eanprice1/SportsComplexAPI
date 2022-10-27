using SportsComplex.Logic.Exceptions;
using SportsComplex.Logic.Interfaces;
using SportsComplex.Logic.Models;
using SportsComplex.Logic.Repositories;
using SportsComplex.Logic.Validators;

namespace SportsComplex.Logic;

public class LocationLogic : ILocationLogic
{
    private readonly IdValidator _idValidator;
    private readonly LocationValidator _locationValidator;
    private readonly ILocationReadRepo _readRepo;
    private readonly ILocationWriteRepo _writeRepo;
    private readonly ISportReadRepo _sportReadRepo;

    public LocationLogic(IdValidator idValidator, LocationValidator locationValidator, ILocationReadRepo readRepo, ILocationWriteRepo writeRepo, ISportReadRepo sportReadRepo)
    {
        _idValidator = idValidator;
        _locationValidator = locationValidator;
        _readRepo = readRepo;
        _writeRepo = writeRepo;
        _sportReadRepo = sportReadRepo;
    }

    public async Task<List<Location>> GetLocationsAsync(LocationQuery filters)
    {
        return await _readRepo.GetLocationsAsync(filters);
    }

    public async Task<Location> GetLocationByIdAsync(int locationId)
    {
        if (locationId <= 0)
            throw new InvalidRequestException("'LocationId' must be greater than 0.");

        return await _readRepo.GetLocationByIdAsync(locationId);
    }

    public async Task<Location> AddLocationAsync(Location location)
    {
        await ValidateAsync(location);
        location.Id = await _writeRepo.InsertLocationAsync(location);
        return location;
    }

    public async Task<Location> UpdateLocationAsync(Location location)
    {
        await ValidateAsync(location, true);
        return await _writeRepo.UpdateLocationAsync(location);
    }

    public async Task DeleteLocationAsync(int locationId)
    {
        await _writeRepo.DeleteLocationAsync(locationId);
    }

    private async Task ValidateAsync(Location location, bool checkId = false)
    {
        if(location == null) 
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