using Dapper.API.Dtos.Hotels;
using FluentValidation;

namespace Dapper.API.Validators
{
    /// <summary>
    /// Validator for AddHotelWithRooms DTO
    /// </summary>
    public class HotelWithRoomsValidator : AbstractValidator<AddHotelWithRooms>
    {
        public HotelWithRoomsValidator(HotelValidator hotelValidator, RoomForHotelWithRoomsValidator roomValidator)
        {
            RuleFor(x => x.Hotel)
                .NotNull().WithMessage("Hotel information is required.")
                .SetValidator(hotelValidator);

            RuleFor(x => x.Rooms)
                .NotNull().WithMessage("At least one room must be provided.")
                .NotEmpty().WithMessage("At least one room must be provided.");

            RuleForEach(x => x.Rooms)
                .SetValidator(roomValidator);

            // Validate that room numbers are unique within the hotel
            RuleFor(x => x.Rooms)
                .Must(rooms => rooms.Select(r => r.RoomNumber).Distinct().Count() == rooms.Count())
                .WithMessage("Room numbers must be unique within the hotel.");
        }
    }
}
