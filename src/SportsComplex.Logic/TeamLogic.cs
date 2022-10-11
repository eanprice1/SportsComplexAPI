using SportsComplex.Logic.Exceptions;
using SportsComplex.Logic.Interfaces;
using SportsComplex.Logic.Models;
using SportsComplex.Logic.Repositories;
using SportsComplex.Logic.Validators;

namespace SportsComplex.Logic;

public class TeamLogic : ITeamLogic
{
    private readonly IdValidator _idValidator;
    private readonly TeamValidator _teamValidator;
    private readonly ITeamReadRepo _teamReadRepo;
    private readonly ITeamWriteRepo _writeRepo;
    private readonly ISportReadRepo _sportReadRepo;

    public TeamLogic(IdValidator idValidator,TeamValidator teamValidator, ITeamReadRepo teamReadRepo, ITeamWriteRepo writeRepo, ISportReadRepo sportReadRepo)
    {
        _idValidator = idValidator;
        _teamValidator = teamValidator;
        _teamReadRepo = teamReadRepo;
        _writeRepo = writeRepo;
        _sportReadRepo = sportReadRepo;
    }

    public async Task<List<Team>> GetTeamsAsync(TeamQuery filters)
    {
        return await _teamReadRepo.GetTeamsAsync(filters);
    }

    public async Task<Team> GetTeamByIdAsync(int teamId)
    {
        if (teamId <= 0)
            throw new InvalidRequestException("'TeamId' must be greater than 0.");

        return await _teamReadRepo.GetTeamByIdAsync(teamId);
    }

    public async Task<Team> AddTeamAsync(Team team)
    {
        await ValidateAsync(team);
        team.Id = await _writeRepo.InsertTeamAsync(team);
        return team;
    }

    public async Task<Team> UpdateTeamAsync(Team team)
    {
        await ValidateAsync(team, true);
        return await _writeRepo.UpdateTeamAsync(team);
    }

    public async Task DeleteTeamAsync(int teamId)
    {
        await _writeRepo.DeleteTeamAsync(teamId);
    }

    private async Task ValidateAsync(Team team, bool checkId = false)
    {
        if (team == null)
            throw new ArgumentNullException(nameof(team));

        var result = await _teamValidator.ValidateAsync(team);

        if (!result.IsValid)
            throw new InvalidRequestException(result.ToString());

        if (checkId)
        {
            result = await _idValidator.ValidateAsync(team);

            if (!result.IsValid)
                throw new InvalidRequestException(result.ToString());
        }

        try
        {
            await _sportReadRepo.GetSportByIdAsync(team.SportId);
        }
        catch (EntityNotFoundException ex)
        {
            throw new InvalidRequestException(
                "'SportId' must exist in the database when adding a new team. See inner exception for details.", ex);
        }
    }
}