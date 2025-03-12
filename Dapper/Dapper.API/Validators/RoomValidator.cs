using Dapper.API.Dtos.Rooms;
using FluentValidation;

namespace Dapper.API.Validators
{
    public class RoomForHotelWithRoomsValidator : AbstractValidator<AddEditRoom>
    {
        public RoomForHotelWithRoomsValidator()
        {

            RuleFor(r => r.RoomNumber)
                .NotEmpty().WithMessage("Room number is required.")
                .MaximumLength(10).WithMessage("Room number cannot exceed 10 characters.")
                .Matches(@"^[A-Za-z0-9\-]+$").WithMessage("Room number can only contain letters, numbers, and hyphens.");

            RuleFor(r => r.RoomTypeId)
                .NotEmpty().WithMessage("Room type is required.")
                .GreaterThan(0).WithMessage("Room type must be valid.");

            RuleFor(r => r.PricePerNight)
                .NotEmpty().WithMessage("Price per night is required.")
                .GreaterThan(0).WithMessage("Price per night must be greater than zero.")
                .LessThan(10000).WithMessage("Price per night seems unusually high.");

            RuleFor(r => r.MaxOccupancy)
                .NotEmpty().WithMessage("Maximum occupancy is required.")
                .InclusiveBetween(1, 10).WithMessage("Maximum occupancy must be between 1 and 10 people.");
        }
    }
}
