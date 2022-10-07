using FluentValidation;
using SportsComplex.Logic.Models;

namespace SportsComplex.Logic.Validators
{
    public class PlayerValidator : AbstractValidator<Player>
    {
        public PlayerValidator()
        {
            CascadeMode = CascadeMode.Stop;
            RuleFor(x => x.TeamId)
                .GreaterThan(0).When(x => x.TeamId != null)
                .WithMessage("'TeamId' must be greater than 0.");
            RuleFor(x => x.GuardianId)
                .GreaterThan(0)
                .WithMessage("'GuardianId' must be greater than 0.");
            RuleFor(x => x.FirstName)
                .NotEmpty().MinimumLength(1).MaximumLength(30)
                .WithMessage("'FirstName' must be between 1 and 30 characters long.");
            RuleFor(x => x.LastName)
                .NotEmpty().MinimumLength(1).MaximumLength(30)
                .WithMessage("'LastName' must be between 1 and 30 characters long.");
            RuleFor(x => x.BirthDate)
                .NotEmpty()
                .WithMessage("'BirthDate' must not be empty.")
                .LessThan(x => DateTime.Today.Date)
                .WithMessage("'BirthDate' must be less than than today's date.");
        }
    }
}
