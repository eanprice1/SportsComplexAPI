using FluentValidation;
using SportsComplex.Logic.Models;

namespace SportsComplex.Logic.Validators
{
    public class EmergencyContactValidator : AbstractValidator<EmergencyContact>
    {
        public EmergencyContactValidator()
        {
            CascadeMode = CascadeMode.Stop;

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
            RuleFor(x => x.PhoneNumber)
                .NotEmpty().Length(10).WithMessage("'PhoneNumber' must be exactly 10 characters long.");
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("'Email' must not be empty.")
                .EmailAddress().WithMessage("'Email' must contain the @ symbol.");
            RuleFor(x => x.Address)
                .NotEmpty().MinimumLength(5).MaximumLength(50)
                .WithMessage("'Address' must be between  5 and 50 characters long.");
            RuleFor(x => x.OtherAddress)
                .MaximumLength(50).WithMessage("'OtherAddress' cannot be greater than 50 characters long.");
            RuleFor(x => x.OtherPhoneNumber)
                .Length(10).When(x => x.OtherPhoneNumber != null)
                .WithMessage("'OtherPhoneNumber' must be exactly 10 characters long or empty.");
        }
    }
}
