using SportsComplex.Logic.Exceptions;
using SportsComplex.Logic.Interfaces;
using SportsComplex.Logic.Models;
using SportsComplex.Logic.Repositories;
using SportsComplex.Logic.Validators;

namespace SportsComplex.Logic.Logic;

public class SportLogic : ISportLogic
{
    private readonly IdValidator _idValidator;
    private readonly SportValidator _sportValidator;
    private readonly ISportReadRepo _readRepo;
    private readonly ISportWriteRepo _writeRepo;

    public SportLogic(IdValidator idValidator, SportValidator sportValidator, ISportReadRepo readRepo, ISportWriteRepo writeRepo)
    {
        _idValidator = idValidator;
        _sportValidator = sportValidator;
        _readRepo = readRepo;
        _writeRepo = writeRepo;
    }

    public async Task<List<Sport>> GetSportsAsync(SportQuery filters)
    {
        if (filters.StartRange == null && filters.EndRange != null)
            throw new InvalidRequestException(
                "'StartRange' and 'EndRange' must either both be null or both contain a DateTime");

        if (filters.StartRange != null && filters.EndRange == null)
            throw new InvalidRequestException(
                "'StartRange' and 'EndRange' must either both be null or both contain a DateTime");

        return await _readRepo.GetSportsAsync(filters);
    }

    public async Task<Sport> GetSportByIdAsync(int sportId)
    {
        if (sportId <= 0)
            throw new InvalidRequestException("'SportId' must be greater than 0.");

        return await _readRepo.GetSportByIdAsync(sportId);
    }

    public async Task<Sport> AddSportAsync(Sport sport)
    {
        await ValidateAsync(sport);
        sport.Id = await _writeRepo.InsertSportAsync(sport);
        return sport;
    }

    public async Task<Sport> UpdateSportAsync(Sport sport)
    {
        await ValidateAsync(sport, true);
        return await _writeRepo.UpdateSportAsync(sport);
    }

    public async Task DeleteSportAsync(int sportId)
    {
        await _writeRepo.DeleteSportAsync(sportId);
    }

    private async Task ValidateAsync(Sport sport, bool checkId = false)
    {
        if (sport == null)
            throw new ArgumentNullException(nameof(sport));

        var result = await _sportValidator.ValidateAsync(sport);

        if (!result.IsValid)
            throw new InvalidRequestException(result.ToString());

        if (checkId)
        {
            result = await _idValidator.ValidateAsync(sport);

            if (!result.IsValid)
                throw new InvalidRequestException(result.ToString());
        }
    }
}