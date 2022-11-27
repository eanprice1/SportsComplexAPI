using FluentValidation;
using SportsComplex.Logic.Models;

namespace SportsComplex.Logic.Validators;

public class SportValidator : AbstractValidator<Sport>
{
    public SportValidator()
    {
        CascadeMode = CascadeMode.Stop;

        RuleFor(x => x.Name)
            .NotEmpty().MinimumLength(1).MaximumLength(30)
            .WithMessage("'Name' must be between 1 and 30 characters long.");
        RuleFor(x => x.Description)
            .NotEmpty().MinimumLength(1).MaximumLength(200)
            .WithMessage("'Description' must be between 1 and 200 characters long.");
        RuleFor(x => x.MaxTeamSize)
            .GreaterThan(0)
            .WithMessage("'MaxTeamSize' must be greater than 0");
        RuleFor(x => x.StartDate)
            .NotEmpty()
            .WithMessage("'StartDate' must not be empty.");
        RuleFor(x => x.EndDate)
            .NotEmpty()
            .WithMessage("'EndDate' must not be empty")
            .GreaterThan(x => x.StartDate)
            .WithMessage("'EndDate' must be greater than 'StartDate'");
    }
}