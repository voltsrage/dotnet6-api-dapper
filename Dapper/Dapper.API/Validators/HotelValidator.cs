using Dapper.API.Dtos.Hotels;
using FluentValidation;

namespace Dapper.API.Validators
{
    public class HotelValidator : AbstractValidator<AddEditHotel>
    {
        public HotelValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().Must(x => !x.Equals("string")).WithMessage($"Email is required.")
                .MaximumLength(100);

            RuleFor(x => x.Address)
                .NotEmpty().Must(x => !x.Equals("string")).WithMessage($"Email is required.")
                .MaximumLength(200);

            RuleFor(x => x.City)
                .NotEmpty().Must(x => !x.Equals("string")).WithMessage($"Email is required.")
                .MaximumLength(100);

            RuleFor(x => x.Country)
                .NotEmpty().Must(x => !x.Equals("string")).WithMessage($"Email is required.")
                .MaximumLength(100);

            RuleFor(x => x.PhoneNumber)
                .MaximumLength(15);

            RuleFor(x => x.Email)
                .NotEmpty().Must(x => !x.Equals("string")).WithMessage($"Email is required.")
                .EmailAddress()
                .MaximumLength(100);


        }
    }
}
