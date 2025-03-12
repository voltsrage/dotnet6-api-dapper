using Dapper.API.Dtos.Hotels;
using FluentValidation;

namespace Dapper.API.Validators
{
    public class HotelValidator : AbstractValidator<AddEditHotel>
    {
        public HotelValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().Must(x => !x.Equals("string")).WithMessage("Hotel name is required.")
                .MaximumLength(100).WithMessage("Hotel name cannot exceed 100 characters.");

            RuleFor(x => x.Address)
                .NotEmpty().Must(x => !x.Equals("string")).WithMessage("Address is required.")
                .MaximumLength(200).WithMessage("Address cannot exceed 200 characters.");

            RuleFor(x => x.City)
                .NotEmpty().Must(x => !x.Equals("string")).WithMessage("City is required.")
                .MaximumLength(100).WithMessage("City name cannot exceed 100 characters.");

            RuleFor(x => x.Country)
                .NotEmpty().Must(x => !x.Equals("string")).WithMessage("Country is required.")
                .MaximumLength(100).WithMessage("Country name cannot exceed 100 characters.");

            RuleFor(x => x.PhoneNumber)
                .MaximumLength(20).WithMessage("Phone number cannot exceed 20 characters.")
                .Matches(@"^[0-9\+\-\(\)\s]*$").WithMessage("Phone number contains invalid characters.");

            RuleFor(x => x.Email)
                .NotEmpty().Must(x => !x.Equals("string")).WithMessage("Email is required.")
                .EmailAddress().WithMessage("A valid email address is required.")
                .MaximumLength(100).WithMessage("Email cannot exceed 100 characters.");

        }
    }
}
