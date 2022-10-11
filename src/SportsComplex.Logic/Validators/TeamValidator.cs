using FluentValidation;
using SportsComplex.Logic.Models;

namespace SportsComplex.Logic.Validators;

public class TeamValidator : AbstractValidator<Team>
{
    public TeamValidator()
    {
        CascadeMode = CascadeMode.Stop;

        RuleFor(x => x.SportId)
            .GreaterThan(0)
            .WithMessage("'SportId' must be greater than 0.");
        RuleFor(x => x.Name)
            .NotEmpty().MinimumLength(1).MaximumLength(30)
            .WithMessage("'FirstName' must be between 1 and 30 characters long.");
        RuleFor(x => x.Motto)
            .NotEmpty().MinimumLength(1).MaximumLength(50)
            .WithMessage("'LastName' must be between 1 and 50 characters long.");
    }
}