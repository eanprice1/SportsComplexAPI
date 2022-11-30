using SportsComplex.Logic.Exceptions;
using SportsComplex.Logic.Interfaces;
using SportsComplex.Logic.Models;
using SportsComplex.Logic.Repositories;
using SportsComplex.Logic.Validators;

namespace SportsComplex.Logic.Logic;

public class PracticeLogic : IPracticeLogic
{
    private readonly IdValidator _idValidator;
    private readonly PracticeValidator _practiceValidator;
    private readonly IPracticeReadRepo _practiceReadRepo;
    private readonly IPracticeWriteRepo _practiceWriteRepo;
    private readonly ITeamReadRepo _teamReadRepo;
    private readonly ILocationReadRepo _locationReadRepo;
    private readonly IMatchReadRepo _matchReadRepo;

    public PracticeLogic(IdValidator idValidator, PracticeValidator practiceValidator, IPracticeReadRepo practiceReadRepo, IPracticeWriteRepo practiceWriteRepo, ITeamReadRepo teamReadRepo, ILocationReadRepo locationReadRepo, IMatchReadRepo matchReadRepo)
    {
        _idValidator = idValidator;
        _practiceValidator = practiceValidator;
        _practiceReadRepo = practiceReadRepo;
        _practiceWriteRepo = practiceWriteRepo;
        _teamReadRepo = teamReadRepo;
        _locationReadRepo = locationReadRepo;
        _matchReadRepo = matchReadRepo;
    }

    public async Task<List<Practice>> GetPracticesAsync(PracticeQuery filters)
    {
        if (filters.StartRange == null && filters.EndRange != null)
            throw new InvalidRequestException(
                "'StartRange' and 'EndRange' must either both be null or both contain a DateTime");

        if (filters.StartRange != null && filters.EndRange == null)
            throw new InvalidRequestException(
                "'StartRange' and 'EndRange' must either both be null or both contain a DateTime");

        return await _practiceReadRepo.GetPracticesAsync(filters);
    }

    public async Task<Practice> GetPracticeById(int practiceId)
    {
        if (practiceId <= 0)
            throw new InvalidRequestException("'PracticeId' must be greater than 0.");

        return await _practiceReadRepo.GetPracticeByIdAsync(practiceId);
    }

    public async Task<Practice> AddPracticeAsync(Practice practice)
    {
        await ValidateAsync(practice);
        practice.Id = await _practiceWriteRepo.InsertPracticeAsync(practice);
        return practice;
    }

    public async Task<Practice> UpdatePracticeAsync(Practice practice)
    {
        await ValidateAsync(practice, true);
        return await _practiceWriteRepo.UpdatePracticeAsync(practice);
    }

    public async Task DeletePracticeAsync(int practiceId)
    {
        await _practiceWriteRepo.DeletePracticeAsync(practiceId);
    }

    private async Task ValidateAsync(Practice practice, bool checkId = false)
    {
        if (practice == null)
            throw new ArgumentNullException(nameof(practice));

        var result = await _practiceValidator.ValidateAsync(practice);

        if (!result.IsValid)
            throw new InvalidRequestException(result.ToString());

        if (checkId)
        {
            result = await _idValidator.ValidateAsync(practice);

            if (!result.IsValid)
                throw new InvalidRequestException(result.ToString());
        }

        try
        {
            var team = await _teamReadRepo.GetTeamByIdAsync(practice.TeamId);

            if (practice.LocationId != null)
            {
                var location = await _locationReadRepo.GetLocationByIdAsync((int)practice.LocationId);
                if (location.SportId != team.SportId)
                    throw new InvalidRequestException(
                        "'LocationId' must have the same 'SportId' as the 'TeamId' in order to schedule a practice at that location.");
            }
        }
        catch (EntityNotFoundException ex)
        {
            throw new InvalidRequestException(
                "'TeamId' and 'LocationId' (if provided) must exist in the database when adding a new practice. See inner exception for details.", ex);
        }

        await ValidatePracticeConflictsAsync(practice);
        await ValidateMatchConflictsAsync(practice);
    }

    private async Task ValidatePracticeConflictsAsync(Practice practice)
    {
        var conflictingPractices = await _practiceReadRepo.GetConflictingPracticesAsync(practice.StartDateTime, practice.EndDateTime);
        if (!conflictingPractices.Any())
            return;

        var teamConflicts = practice.Id == 0
            ? conflictingPractices.Where(x => x.TeamId == practice.TeamId)
                .Select(x => x.Id).ToList()
            : conflictingPractices.Where(x => x.Id != practice.Id && x.TeamId == practice.TeamId)
                .Select(x => x.Id).ToList();

        if (teamConflicts.Any())
            throw new InvalidRequestException(
                $"'TeamId' must not already be scheduled for a practice that conflicts with the provided 'StartDateTime' and 'EndDateTime'. Conflicting Practice Ids: {string.Join(", ", teamConflicts)}");

        if (practice.LocationId != null)
        {
            var locationConflicts = practice.Id == 0
                ? conflictingPractices.Where(x => x.LocationId == practice.LocationId).Select(x => x.Id).ToList()
                : conflictingPractices.Where(x => x.LocationId == practice.LocationId && x.Id != practice.Id).Select(x => x.Id).ToList();

            if (locationConflicts.Any())
                throw new InvalidRequestException(
                    $"'StartDateTime' and 'EndDateTime' must not conflict with other practices at the same location. Conflicting Practice Ids: {string.Join(", ", locationConflicts)}");
        }
    }

    private async Task ValidateMatchConflictsAsync(Practice practice)
    {
        var conflictingMatches =
            await _matchReadRepo.GetConflictingMatchesAsync(practice.StartDateTime, practice.EndDateTime);
        if (!conflictingMatches.Any())
            return;

        var teamConflicts = conflictingMatches
            .Where(x => x.AwayTeamId == practice.TeamId || x.HomeTeamId == practice.TeamId).Select(x => x.Id).ToList();

        if (teamConflicts.Any())
            throw new InvalidRequestException(
                $"'TeamId' must not already be scheduled for a match that conflicts with the provided 'StartDateTime' and 'EndDateTime'. Conflicting Match Ids: {string.Join(", ", teamConflicts)}");

        if (practice.LocationId != null)
        {
            var locationConflicts = conflictingMatches.Where(x => x.LocationId == practice.LocationId).Select(x => x.Id)
                .ToList();
            if (locationConflicts.Any())
                throw new InvalidRequestException(
                    $"'StartDateTime' and 'EndDateTime' of practice must not conflict with matches at the same location. Conflicting Match Ids: {string.Join(", ", locationConflicts)}");
        }
    }
}