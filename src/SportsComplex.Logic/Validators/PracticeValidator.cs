using FluentValidation;
using SportsComplex.Logic.Models;

namespace SportsComplex.Logic.Validators;

public class PracticeValidator : AbstractValidator<Practice>
{
    public PracticeValidator()
    {
        CascadeMode = CascadeMode.Stop;

        RuleFor(x => x.TeamId)
            .GreaterThan(0)
            .WithMessage("'TeamId' must be greater than 0.");
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