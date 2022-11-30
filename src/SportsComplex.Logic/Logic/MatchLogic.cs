using SportsComplex.Logic.Exceptions;
using SportsComplex.Logic.Interfaces;
using SportsComplex.Logic.Models;
using SportsComplex.Logic.Repositories;
using SportsComplex.Logic.Validators;

namespace SportsComplex.Logic.Logic;

public class MatchLogic : IMatchLogic
{
    private readonly IdValidator _idValidator;
    private readonly MatchValidator _matchValidator;
    private readonly IMatchReadRepo _matchReadRepo;
    private readonly IMatchWriteRepo _matchWriteRepo;
    private readonly ITeamReadRepo _teamReadRepo;
    private readonly ILocationReadRepo _locationReadRepo;
    private readonly IPracticeReadRepo _practiceReadRepo;

    public MatchLogic(IdValidator idValidator, MatchValidator matchValidator, IMatchReadRepo matchReadRepo, IMatchWriteRepo matchWriteRepo, ITeamReadRepo teamReadRepo, ILocationReadRepo locationReadRepo, IPracticeReadRepo practiceReadRepo)
    {
        _idValidator = idValidator;
        _matchValidator = matchValidator;
        _matchReadRepo = matchReadRepo;
        _matchWriteRepo = matchWriteRepo;
        _teamReadRepo = teamReadRepo;
        _locationReadRepo = locationReadRepo;
        _practiceReadRepo = practiceReadRepo;
    }

    public async Task<List<Match>> GetMatchesAsync(MatchQuery filters)
    {
        if (filters.StartRange == null && filters.EndRange != null)
            throw new InvalidRequestException(
                "'StartRange' and 'EndRange' must either both be null or both contain a DateTime");

        if (filters.StartRange != null && filters.EndRange == null)
            throw new InvalidRequestException(
                "'StartRange' and 'EndRange' must either both be null or both contain a DateTime");

        return await _matchReadRepo.GetMatchesAsync(filters);
    }

    public async Task<Match> GetMatchById(int matchId)
    {
        if(matchId <= 0)
            throw new InvalidRequestException("'MatchId' must be greater than 0.");

        return await _matchReadRepo.GetMatchByIdAsync(matchId);
    }

    public async Task<Match> AddMatchAsync(Match match)
    {
        await ValidateAsync(match);
        match.Id = await _matchWriteRepo.InsertMatchAsync(match);
        return match;
    }

    public async Task<Match> UpdateMatchAsync(Match match)
    {
        await ValidateAsync(match, true);
        return await _matchWriteRepo.UpdateMatchAsync(match);
    }

    public async Task DeleteMatchAsync(int matchId)
    {
        await _matchWriteRepo.DeleteMatchAsync(matchId);
    }

    private async Task ValidateAsync(Match match, bool checkId = false)
    {
        if (match == null)
            throw new ArgumentNullException(nameof(match));

        var result = await _matchValidator.ValidateAsync(match);

        if (!result.IsValid)
            throw new InvalidRequestException(result.ToString());

        if (checkId)
        {
            result = await _idValidator.ValidateAsync(match);

            if (!result.IsValid)
                throw new InvalidRequestException(result.ToString());
        }

        try
        {
            var homeTeam = await _teamReadRepo.GetTeamByIdAsync(match.HomeTeamId);
            var awayTeam = await _teamReadRepo.GetTeamByIdAsync(match.AwayTeamId);

            if (homeTeam.SportId != awayTeam.SportId)
                throw new InvalidRequestException(
                    "'HomeTeamId' and 'AwayTeamId' must have the same 'SportId' in order to play a match against each other.");

            if (match.LocationId != null)
            {
                var location = await _locationReadRepo.GetLocationByIdAsync((int)match.LocationId);
                if (location.SportId != homeTeam.SportId)
                    throw new InvalidRequestException(
                        "'LocationId' must have the same 'SportId' as the 'HomeTeamId' and 'AwayTeamId' in order to schedule a match at that location.");
            }
        }
        catch (EntityNotFoundException ex)
        {
            throw new InvalidRequestException(
                "'HomeTeamId', 'AwayTeamId', and 'LocationId' (if provided) must exist in the database when adding a new match. See inner exception for details.", ex);
        }

        await ValidateMatchConflictsAsync(match);
        await ValidatePracticeConflictsAsync(match);
    }

    private async Task ValidateMatchConflictsAsync(Match match)
    {
        var conflictingMatches = await _matchReadRepo.GetConflictingMatchesAsync(match.StartDateTime, match.EndDateTime);
        if (!conflictingMatches.Any())
            return;

        var teams = new List<int> { match.HomeTeamId, match.AwayTeamId };
        var teamConflicts = match.Id == 0
            ? conflictingMatches.Where(x => teams.Contains(x.AwayTeamId) || teams.Contains(x.HomeTeamId))
                .Select(x => x.Id).ToList()
            : conflictingMatches.Where(x => x.Id != match.Id && (teams.Contains(x.AwayTeamId) || teams.Contains(x.HomeTeamId)))
                .Select(x => x.Id).ToList();

        if (teamConflicts.Any())
            throw new InvalidRequestException(
                $"'HomeTeamId' and 'AwayTeamId' must not already be scheduled for a match that conflicts with the provided 'StartDateTime' and 'EndDateTime'. Conflicting Match Ids: {string.Join(", ", teamConflicts)}");

        if (match.LocationId != null)
        {
            var locationConflicts = match.Id == 0
                ? conflictingMatches.Where(x => x.LocationId == match.LocationId).Select(x => x.Id).ToList()
                : conflictingMatches.Where(x => x.LocationId == match.LocationId && x.Id != match.Id).Select(x => x.Id).ToList();

            if (locationConflicts.Any())
                throw new InvalidRequestException(
                    $"'StartDateTime' and 'EndDateTime' must not conflict with other matches at the same location. Conflicting Match Ids: {string.Join(", ", locationConflicts)}");
        }
    }

    private async Task ValidatePracticeConflictsAsync(Match match)
    {
        var conflictingPractices = await _practiceReadRepo.GetConflictingPracticesAsync(match.StartDateTime, match.EndDateTime);
        if (!conflictingPractices.Any())
            return;

        var teams = new List<int> { match.HomeTeamId, match.AwayTeamId };
        var teamConflicts = conflictingPractices.Where(x => teams.Contains(x.TeamId)).Select(x => x.Id).ToList();

        if (teamConflicts.Any())
            throw new InvalidRequestException(
                $"'HomeTeamId' and 'AwayTeamId' must not already be scheduled for a practice that conflicts with the provided 'StartDateTime' and 'EndDateTime' of the match. Conflicting Practice Ids: {string.Join(", ", teamConflicts)}");

        if (match.LocationId != null)
        {
            var locationConflicts = conflictingPractices.Where(x => x.LocationId == match.LocationId).Select(x => x.Id).ToList();
            if (locationConflicts.Any())
                throw new InvalidRequestException(
                    $"'StartDateTime' and 'EndDateTime' of match must not conflict with practices at the same location. Conflicting Practice Ids: {string.Join(", ", locationConflicts)}");
        }
    }
}