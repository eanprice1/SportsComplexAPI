using SportsComplex.Logic.Exceptions;
using SportsComplex.Logic.Interfaces;
using SportsComplex.Logic.Models;
using SportsComplex.Logic.Repositories;
using SportsComplex.Logic.Validators;

namespace SportsComplex.Logic;

public class SportLogic : ISportLogic
{
    private readonly IdValidator _idValidator;
    private readonly SportValidator _sportValidator;
    private readonly ISportReadRepo _sportReadRepo;
    private readonly ISportWriteRepo _sportWriteRepo;

    public SportLogic(IdValidator idValidator, SportValidator sportValidator, ISportReadRepo sportReadRepo, ISportWriteRepo sportWriteRepo)
    {
        _idValidator = idValidator;
        _sportValidator = sportValidator;
        _sportReadRepo = sportReadRepo;
        _sportWriteRepo = sportWriteRepo;
    }

    public async Task<List<Sport>> GetSportsAsync(SportQuery filters)
    {
        if (filters.StartRange == null && filters.EndRange != null)
            throw new InvalidRequestException(
                "'StartRange' and 'EndRange' must either both be null or both contain a DateTime");

        if (filters.StartRange != null && filters.EndRange == null)
            throw new InvalidRequestException(
                "'StartRange' and 'EndRange' must either both be null or both contain a DateTime");

        return await _sportReadRepo.GetSportsAsync(filters);
    }

    public async Task<Sport> GetSportByIdAsync(int sportId)
    {
        if (sportId <= 0)
            throw new InvalidRequestException("'SportId' must be greater than 0.");

        return await _sportReadRepo.GetSportByIdAsync(sportId);
    }

    public async Task<Sport> AddSportAsync(Sport sport)
    {
        await Validate(sport);
        sport.Id = await _sportWriteRepo.InsertSportAsync(sport);
        return sport;
    }

    public async Task<Sport> UpdateSportAsync(Sport sport)
    {
        await Validate(sport, true);
        return await _sportWriteRepo.UpdateSportAsync(sport);
    }

    public async Task DeleteSportAsync(int sportId)
    {
        await _sportWriteRepo.DeleteSportAsync(sportId);
    }

    private async Task Validate(Sport sport, bool checkId = false)
    {
        if(sport == null)
            throw new ArgumentNullException(nameof(sport));

        var result = await _sportValidator.ValidateAsync(sport);

        if(!result.IsValid)
            throw new InvalidRequestException(result.ToString());

        if (checkId)
        {
            result = await _idValidator.ValidateAsync(sport);

            if(!result.IsValid)
                throw new InvalidRequestException(result.ToString());
        }
    }
}