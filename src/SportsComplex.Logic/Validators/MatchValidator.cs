using FluentValidation;
using SportsComplex.Logic.Models;

namespace SportsComplex.Logic.Validators;

public class MatchValidator : AbstractValidator<Match>
{
    public MatchValidator()
    {
        CascadeMode = CascadeMode.Stop;

        RuleFor(x => x.HomeTeamId)
            .GreaterThan(0)
            .WithMessage("'HomeTeamId' must be greater than 0.");
        RuleFor(x => x.AwayTeamId)
            .GreaterThan(0)
            .WithMessage("'AwayTeamId' must be greater than 0.")
            .NotEqual(x => x.HomeTeamId)
            .WithMessage("'AwayTeamId' must not be equal to 'HomeTeamId'.");
        RuleFor(x => x.LocationId)
            .GreaterThan(0).When(x => x.LocationId != null)
            .WithMessage("'LocationId' must be greater than 0.");
        RuleFor(x => x.StartDateTime)
            .NotEmpty()
            .WithMessage("'StartDateTime' must not be empty.");
        RuleFor(x => x.EndDateTime)
            .NotEmpty()
            .WithMessage("'EndDateTime' must not be empty.")
            .GreaterThan(x => x.StartDateTime)
            .WithMessage("'EndDateTime' must be greater than 'StartDateTime'.");
        RuleFor(x => x.EndDateTime.Date)
            .Equal(x => x.StartDateTime.Date)
            .WithMessage("'EndDateTime' date must be equal to 'StartDateTime' date.");
    }
    
}