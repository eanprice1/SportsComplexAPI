using SportsComplex.Logic.Exceptions;
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

    public MatchLogic(IdValidator idValidator, MatchValidator matchValidator, IMatchReadRepo matchReadRepo, IMatchWriteRepo matchWriteRepo, ITeamReadRepo teamReadRepo, ILocationReadRepo locationReadRepo)
    {
        _idValidator = idValidator;
        _matchValidator = matchValidator;
        _matchReadRepo = matchReadRepo;
        _matchWriteRepo = matchWriteRepo;
        _teamReadRepo = teamReadRepo;
        _locationReadRepo = locationReadRepo;
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

        // Verify there are no conflicting matches with the provided StartDateTime, EndDateTime, and LocationId
        await ValidateLocationConflicts(match);

        // Verify that both teams are not already scheduled for a match
        await ValidateTeamConflicts(match);
    }

    private async Task ValidateLocationConflicts(Match match)
    {
        if (match.LocationId != null)
        {
            var filters = new MatchQuery
            {
                LocationIds = new List<int> { (int)match.LocationId },
                StartRange = match.StartDateTime,
                EndRange = match.EndDateTime,
                Descending = false
            };
            var matches = await _matchReadRepo.GetMatchesAsync(filters);

            var conflictingMatches = match.Id == 0
                ? matches.Select(x => x.Id).ToList()
                : matches.Select(x => x.Id).Where(id => id != match.Id).ToList();

            if (conflictingMatches.Any())
                throw new InvalidRequestException(
                    $"'StartDateTime' and 'EndDateTime' must not conflict with other matches at the same location. Conflicting Match Ids: {string.Join(", ", conflictingMatches)}");
        }
    }

    private async Task ValidateTeamConflicts(Match match)
    {
        var filters = new MatchQuery
        {
            TeamIds = new List<int> { match.AwayTeamId, match.HomeTeamId },
            StartRange = match.StartDateTime,
            EndRange = match.EndDateTime,
            Descending = false
        };
        var matches = await _matchReadRepo.GetMatchesAsync(filters);
        var conflictingMatches = matches.Select(x => x.Id).ToList();

        if (conflictingMatches != null && conflictingMatches.Any())
            throw new InvalidRequestException(
                $"'HomeTeamId' and 'AwayTeamId' must not already be scheduled for a match at another location. Conflicting Match Ids: {string.Join(", ", conflictingMatches)}");

    }
}