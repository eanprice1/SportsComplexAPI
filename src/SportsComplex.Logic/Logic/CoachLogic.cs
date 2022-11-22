using SportsComplex.Logic.Exceptions;
using SportsComplex.Logic.Interfaces;
using SportsComplex.Logic.Models;
using SportsComplex.Logic.Repositories;
using SportsComplex.Logic.Validators;

namespace SportsComplex.Logic.Logic;

public class CoachLogic : ICoachLogic
{
    private readonly IdValidator _idValidator;
    private readonly CoachValidator _coachValidator;
    private readonly ICoachReadRepo _readRepo;
    private readonly ICoachWriteRepo _writeRepo;
    private readonly ITeamReadRepo _teamReadRepo;

    public CoachLogic(IdValidator idValidator, CoachValidator coachValidator, ICoachReadRepo readRepo, ICoachWriteRepo writeRepo, ITeamReadRepo teamReadRepo)
    {
        _idValidator = idValidator;
        _coachValidator = coachValidator;
        _readRepo = readRepo;
        _writeRepo = writeRepo;
        _teamReadRepo = teamReadRepo;
    }

    public async Task<List<Coach>> GetCoachesAsync(CoachQuery filters)
    {
        return await _readRepo.GetCoachesAsync(filters);
    }

    public async Task<Coach> GetCoachById(int coachId)
    {
        if (coachId <= 0)
            throw new InvalidRequestException("'CoachId' must be greater than 0.");

        return await _readRepo.GetCoachByIdAsync(coachId);
    }

    public async Task<Coach> AddCoachAsync(Coach coach)
    {
        await ValidateAsync(coach);
        coach.Id = await _writeRepo.InsertCoachAsync(coach);
        return coach;
    }

    public async Task<Coach> UpdateCoachAsync(Coach coach)
    {
        await ValidateAsync(coach, true);
        return await _writeRepo.UpdateCoachAsync(coach);
    }

    public async Task DeleteCoachAsync(int coachId)
    {
        await _writeRepo.DeleteCoachAsync(coachId);
    }

    private async Task ValidateAsync(Coach coach, bool checkId = false)
    {
        if (coach == null)
            throw new ArgumentNullException(nameof(coach));

        var result = await _coachValidator.ValidateAsync(coach);

        if (!result.IsValid)
            throw new InvalidRequestException(result.ToString());

        if (checkId)
        {
            result = await _idValidator.ValidateAsync(coach);

            if (!result.IsValid)
                throw new InvalidRequestException(result.ToString());
        }

        try
        {
            if (coach.TeamId != null)
            {
                await _teamReadRepo.GetTeamByIdAsync((int)coach.TeamId);

                if (coach.IsHeadCoach)
                {
                    var existingHeadCoach = await _readRepo.GetCoachesAsync(new CoachQuery
                    {
                        TeamIds = new List<int> { (int)coach.TeamId },
                        IsHeadCoach = true
                    });

                    if (existingHeadCoach.Any())
                        throw new InvalidRequestException(
                            $"'TeamId={coach.TeamId}' already has a coach assigned with 'IsHeadCoach=true'. A team can only have one head coach assigned.");
                }
            }
        }
        catch (EntityNotFoundException ex)
        {
            throw new InvalidRequestException(
                "'TeamId' (if provided) must exist in the database when adding a new coach. See inner exception for details.", ex);
        }
    }
}