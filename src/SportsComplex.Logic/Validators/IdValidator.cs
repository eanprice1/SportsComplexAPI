using FluentValidation;
using SportsComplex.Logic.Interfaces;

namespace SportsComplex.Logic.Validators
{
    public class IdValidator : AbstractValidator<IModel>
    {
        public IdValidator()
        {
            CascadeMode = CascadeMode.Stop;

            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage("'Id' must be greater than 0.");
        }
    }
}
