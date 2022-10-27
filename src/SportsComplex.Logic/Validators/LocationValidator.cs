using FluentValidation;
using SportsComplex.Logic.Models;

namespace SportsComplex.Logic.Validators;

public class LocationValidator : AbstractValidator<Location>
{
    public LocationValidator()
    {
        CascadeMode = CascadeMode.Stop;

        RuleFor(x => x.SportId)
            .GreaterThan(0)
            .WithMessage("'SportId' must be greater than 0.");
        RuleFor(x => x.Name)
            .NotEmpty().MinimumLength(1).MaximumLength(30)
            .WithMessage("'Name' must be between 1 and 30 characters long.");
        RuleFor(x => x.Address)
            .NotEmpty().MinimumLength(5).MaximumLength(50)
            .WithMessage("'Address' must be between 5 and 50 characters long.");
        RuleFor(x => x.Description)
            .NotEmpty().MinimumLength(1).MaximumLength(200)
            .WithMessage("'Description' must be between 1 and 200 characters long.");
    }
}